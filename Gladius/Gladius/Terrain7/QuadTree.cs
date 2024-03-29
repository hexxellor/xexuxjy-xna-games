﻿// Sample taken from http://www.dustinhorne.com/page/XNA-Terrain-Tutorial-Table-of-Contents.aspx - many thanks

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gladius.Terrain7.Culling;
using Gladius.util;

namespace Gladius.Terrain7
{
	public class QuadTree
	{
		private QuadNode _rootNode;
		private QuadNode _activeNode;
		private TreeVertexCollection _vertices;
		private BufferManager _buffers;
		private Vector3 _position;
		private int _topNodeSize;

		public ViewClipShape ClipShape;
		internal Vector3[] VFCorners = new Vector3[8];

		private Vector3 _cameraPosition;
		private Vector3 _lastCameraPosition;
		public int[] Indices;

		public Matrix View;
		public Matrix Projection;

		public GraphicsDevice Device;

		public BasicEffect Effect;

		public int MinimumDepth;

        RasterizerState _rsDefault = new RasterizerState();
        RasterizerState _rsWire = new RasterizerState();

		public int TopNodeSize { get { return _topNodeSize; } }
		public QuadNode RootNode { get { return _rootNode; } }
		public TreeVertexCollection Vertices { get { return _vertices; } }
		public Vector3 CameraPosition
		{
			get { return _cameraPosition; }
			set 
            {
                // move camera to local space?
                _cameraPosition = value-_position; 
            }
		}
		public BoundingFrustum ViewFrustrum { get; private set; }
		public int IndexCount { get; private set; }

		public bool Cull { get; set; }

        private ObjectArray<VertexPositionNormalTexture> SkirtVertexOA;
        private ObjectArray<int> SkirtIndexOA;


		public QuadTree(Vector3 position, Texture2D heightMap, Matrix viewMatrix, Matrix projectionMatrix, GraphicsDevice device, int scale,float heightScale)
		{
            //position = new Vector3(0, -10, 0);


			Device = device;
			_position = position;

            

            int hmWidth = heightMap.Width;
            //hmWidth = 2;

            _topNodeSize = hmWidth - 1;

			_vertices = new TreeVertexCollection(heightMap, scale,heightScale,hmWidth);
			_buffers = new BufferManager(_vertices.Vertices, device);
			_rootNode = new QuadNode(NodeType.FullNode, _topNodeSize, 1, null, this, 0);

            _rsDefault.CullMode = CullMode.CullCounterClockwiseFace;
            _rsDefault.FillMode = FillMode.Solid;

            _rsWire.CullMode = CullMode.CullCounterClockwiseFace;
            _rsWire.FillMode = FillMode.WireFrame;


			View = viewMatrix;
			Projection = projectionMatrix;

			//Initialize the bounding frustrum to be used for culling later.
			ViewFrustrum = new BoundingFrustum(viewMatrix * projectionMatrix);

			//Construct an array large enough to hold all of the indices we'll need
			Indices = new int[((heightMap.Width + 1) * (heightMap.Height + 1)) * 3];

			Effect = new BasicEffect(Device);
			Effect.EnableDefaultLighting();
			Effect.FogEnabled = false;
			Effect.FogStart = 300f;
			Effect.FogEnd = 1000f;
			Effect.FogColor = Color.Black.ToVector3();
			Effect.TextureEnabled = true;
			Effect.Texture = new Texture2D(device, 100, 100);
			Effect.Projection = projectionMatrix;
			Effect.View = viewMatrix;
            Effect.World = Matrix.CreateTranslation(_position) ;

            int defaultSize = 100;
            SkirtVertexOA = new ObjectArray<VertexPositionNormalTexture>(defaultSize);
            SkirtIndexOA = new ObjectArray<int>(defaultSize * 24);

            MinimumDepth = 6;
        
        }

        public Texture2D Texture
        {
            set
            {
                Effect.Texture = value;
            }
        }

		public void Update(GameTime gameTime)
		{
			ViewFrustrum.Matrix = View*Projection;
			Effect.View = View;
			Effect.Projection = Projection;

			//Corners 0-3 = near plane clockwise, Corners 4-7 = far plane clockwise
			ViewFrustrum.GetCorners(VFCorners);

			var clip = ClippingFrustrum.FromFrustrumCorners(VFCorners, CameraPosition);
			ClipShape = clip.ProjectToTargetY(_position.Y);

			_lastCameraPosition = CameraPosition;
			IndexCount = 0;

			_rootNode.Merge();
			_rootNode.EnforceMinimumDepth();

			_activeNode = _rootNode.DeepestNodeWithPoint(ClipShape.ViewPoint);

			
			if (_activeNode != null)
			{
				_activeNode.Split();
			}

			_rootNode.SetActiveVertices();

			_buffers.UpdateIndexBuffer(Indices, IndexCount);
			_buffers.SwapBuffer();
		}

		public void Draw(GameTime gameTime)
		{

            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;

			Device.SetVertexBuffer(_buffers.VertexBuffer);
			Device.Indices = _buffers.IndexBuffer;
            //Device.RasterizerState = _rsWire;

            Effect.World = Matrix.CreateTranslation(_position);

            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.Vertices.Length, 0, IndexCount / 3);
            }
            if (SkirtsEnabled)
            {
                SkirtVertexOA.Clear();
                SkirtIndexOA.Clear();
                _rootNode.BuildSkirt(SkirtVertexOA, SkirtIndexOA);
                _buffers.SetSkirtData(SkirtVertexOA, SkirtIndexOA);
                if (SkirtVertexOA.Count > 0)
                {
                    Device.SetVertexBuffer(_buffers.SkirtVertexBuffer);
                    Device.Indices = _buffers.SkirtIndexBuffer;
                    foreach (var pass in Effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _buffers.SkirtVertexBuffer.VertexCount, 0, _buffers.SkirtIndexBuffer.IndexCount / 3);
                    }
                }
            }
            //Device.RasterizerState = _rsDefault;

		}


        public void DrawEffect(GameTime gameTime,Effect effect)
        {
            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;

            Device.SetVertexBuffer(_buffers.VertexBuffer);
            Device.Indices = _buffers.IndexBuffer;
            //Device.RasterizerState = _rsWire;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.Vertices.Length, 0, IndexCount / 3);
            }

            if (SkirtsEnabled)
            {
                SkirtVertexOA.Clear();
                SkirtIndexOA.Clear();
                _rootNode.BuildSkirt(SkirtVertexOA, SkirtIndexOA);
                _buffers.SetSkirtData(SkirtVertexOA, SkirtIndexOA);
                if (SkirtVertexOA.Count > 0)
                {
                    Device.SetVertexBuffer(_buffers.SkirtVertexBuffer);
                    Device.Indices = _buffers.SkirtIndexBuffer;
                    foreach (var pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _buffers.SkirtVertexBuffer.VertexCount, 0, _buffers.SkirtIndexBuffer.IndexCount / 3);
                    }
                }
            }
            Device.RasterizerState = _rsDefault;
        }

        public bool SkirtsEnabled
        {
            get;
            set;
        }


		internal void UpdateBuffer(int vIndex)
		{
			Indices[IndexCount] = vIndex;
			IndexCount++;
		}
	}
}
