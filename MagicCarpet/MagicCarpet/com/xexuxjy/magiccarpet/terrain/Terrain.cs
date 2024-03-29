/*
* Created on 11-Jan-2006
*
* To change the template for this generated file go to
* Window - Preferences - Java - Code Generation - Code and Comments
*/
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.collision;
using System.Collections.Generic;
using com.xexuxjy.magiccarpet.util;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework.Graphics;
using BulletXNA.BulletCollision;
using com.xexuxjy.magiccarpet.interfaces;
using BulletXNA.LinearMath;
using com.xexuxjy.magiccarpet.renderer;
using BulletXNA;
using com.xexuxjy.magiccarpet.manager;

namespace com.xexuxjy.magiccarpet.terrain
{
    public class Terrain : GameObject
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////

        public Terrain(Vector3 position)
            : base(position, GameObjectType.terrain)
        {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        virtual public int Width
        {
            get
            {
                return (int)(BoundingBox.Max.X - BoundingBox.Min.X);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        virtual public int Breadth
        {
            get
            {
                return (int)(BoundingBox.Max.Z - BoundingBox.Min.Z);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public override Vector3 Position
        {
            set
            {
                Matrix m = WorldTransform;
                m.Translation = value;
                WorldTransform = m;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public float OneOverTextureWidth
        {
            get
            {
                return m_oneOverTextureWidth;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public Texture2D HeightMapTexture
        {
            get
            {
                return m_heightMapTexture;
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void LoadOrCreateHeightMap(String textureName)
        {
            m_heightMap = new float[m_textureWidth * m_textureWidth];
            m_heightMapTexture = Globals.MCContentManager.GetTexture("TerrainHeightMap");
            //m_normalMapTexture = new Texture2D(Globals.GraphicsDevice, m_textureWidth, m_textureWidth, false, SurfaceFormat.Color);
            m_normalMapRenderTarget = (RenderTarget2D)Globals.MCContentManager.GetTexture("TerrainNormalMap");
            UpdateHeightMap();

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override CollisionFilterGroups GetCollisionFlags()
        {
            return (CollisionFilterGroups)GameObjectType.terrain;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override CollisionFilterGroups GetCollisionMask()
        {
            return (CollisionFilterGroups)(GameObjectType.spell | GameObjectType.manaball | GameObjectType.camera | GameObjectType.magician | GameObjectType.monster);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////

        public override CollisionShape BuildCollisionShape()
        {
            return new HeightfieldTerrainShape(m_textureWidth, m_textureWidth, m_heightMap, 1f, -Globals.WorldHeight, Globals.WorldHeight, 1, true);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize()
        {
            m_terrainEffect = Globals.MCContentManager.GetEffect("Terrain");
            m_normalsEffect = Globals.MCContentManager.GetEffect("TerrainNormal");
            m_baseTexture = Globals.MCContentManager.GetTexture("TerrainBase");
            m_noiseTexture = Globals.MCContentManager.GetTexture("TerrainNoise");

            m_terrainEffect.Parameters["BaseTexture"].SetValue(m_baseTexture);
            m_terrainEffect.Parameters["NoiseTexture"].SetValue(m_noiseTexture);

            LightManager.ApplyLightToEffect(m_terrainEffect);

            m_helperScreenQuad = new ScreenQuad(Game);
            m_helperScreenQuad.Initialize();

            BuildVertexBuffers();

            InitialiseWorldGrid();

            LoadOrCreateHeightMap(null);


            base.Initialize();

            // after init so we get the right draw order.
            DrawOrder = Globals.NORMAL_DRAW_ORDER;

            m_noCullState = new RasterizerState();
            m_noCullState.CullMode = CullMode.None;

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void BuildVertexBuffers()
        {
            int adjustedVertices = (m_blockSize * m_multiplier) + 1;
            int adjustedIndices = (m_blockSize * m_multiplier);


            PosOnlyVertex[] blockVertices = new PosOnlyVertex[adjustedVertices * adjustedVertices];
            int[] blockIndices = new int[adjustedIndices * adjustedIndices * 6];
            int indexCounter = 0;
            int vertexCounter = 0;
            int stride = adjustedVertices;


            for (int y = 0; y < adjustedVertices; ++y)
            {
                for (int x = 0; x < adjustedVertices; ++x)
                {
                    Vector2 v = new Vector2(x, y) * m_oneOver;
                    PosOnlyVertex vpnt = new PosOnlyVertex(v);

                    blockVertices[vertexCounter++] = vpnt;
                    if (x < adjustedIndices && y < adjustedIndices)
                    {
                        blockIndices[indexCounter++] = (x + (y * stride));
                        blockIndices[indexCounter++] = (x + 1 + (y * stride));
                        blockIndices[indexCounter++] = (x + 1 + ((y + 1) * stride));

                        blockIndices[indexCounter++] = (x + 1 + ((y + 1) * stride));
                        blockIndices[indexCounter++] = (x + ((y + 1) * stride));
                        blockIndices[indexCounter++] = (x + (y * stride));
                    }
                }
            }

            m_blockVertexBuffer = new VertexBuffer(Globals.GraphicsDevice, PosOnlyVertex.VertexDeclaration, blockVertices.Length, BufferUsage.None);
            m_blockVertexBuffer.SetData<PosOnlyVertex>(blockVertices, 0, blockVertices.Length);
            m_blockIndexBuffer = new IndexBuffer(Globals.GraphicsDevice, IndexElementSize.ThirtyTwoBits, blockIndices.Length, BufferUsage.None);
            m_blockIndexBuffer.SetData<int>(blockIndices);

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateHeightMapTexture()
        {
            if (HasTerrainChanged())
            {
                m_normalsEffect.Parameters["HeightMapTexture"].SetValue((Texture)null);
                m_terrainEffect.Parameters["HeightMapTexture"].SetValue((Texture)null);

                // need these here to unset the textures so I can change them.
                foreach (EffectPass pass in m_terrainEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }

                foreach (EffectPass pass in m_normalsEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }

                m_heightMapTexture.SetData<Single>(m_heightMap);
                //m_terrainEffect.Parameters["HeightMapTexture"].SetValue(m_heightMapTexture);

                UpdateNormalMap();


                ClearTerrainChanged();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateNormalMap()
        {
            //m_terrainEffect.Parameters["NormalMapTexture"].SetValue((Texture)null);

            int width = m_heightMapTexture.Width;

            Globals.GraphicsDevice.SetRenderTarget(m_normalMapRenderTarget);
            Globals.GraphicsDevice.Clear(Color.White);

            m_normalsEffect.CurrentTechnique = m_normalsEffect.Techniques["ComputeNormals"];
            m_normalsEffect.Parameters["HeightMapTexture"].SetValue(m_heightMapTexture);
            float texelWidth = 1f / (float)width;
            m_normalsEffect.Parameters["TexelWidth"].SetValue(texelWidth);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, width, width, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            m_normalsEffect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);

            foreach (EffectPass pass in m_normalsEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                m_helperScreenQuad.Draw();
            }

            m_normalsEffect.Parameters["HeightMapTexture"].SetValue((Texture)null);

            Globals.GraphicsDevice.SetRenderTarget(null);
            m_terrainEffect.Parameters["NormalMapTexture"].SetValue(m_normalMapRenderTarget);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public override void Draw(GameTime gameTime)
        {
            
            UpdateHeightMapTexture();

            Vector3 lastStartPosition = Vector3.Zero;
            
            Vector4 scaleFactor = new Vector4(m_oneOver);
            m_terrainEffect.Parameters["ScaleFactor"].SetValue(scaleFactor);
            Globals.MCContentManager.ApplyCommonEffectParameters(m_terrainEffect);
            DrawTerrainBlocks();
            //DrawTrees();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void DrawTerrainBlocks()
        {
            int numSpans = Globals.WorldWidth / m_blockSize;
            Vector3 blockSize = new Vector3(m_blockSize, 0, m_blockSize);
            Vector3 startPosition = new Vector3(-Globals.WorldWidth * 0.5f, 0, -Globals.WorldWidth * 0.5f);

            // draw in the wall
            m_terrainEffect.CurrentTechnique = m_terrainEffect.Techniques["TerrainWall"];
            Model m = Globals.MCContentManager.GetModelForName("TerrainWalls");

            Matrix wallTransform = Matrix.CreateScale(new Vector3(Globals.WorldWidth / 2, 100, Globals.WorldWidth / 2));
            //Matrix wallTransform = Matrix.Identity;
            //wallTransform.Translation = startPosition;
            m_terrainEffect.Parameters["HeightMapTexture"].SetValue(m_heightMapTexture);
            m_terrainEffect.Parameters["WorldMatrix"].SetValue(wallTransform);
            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (EffectPass pass in m_terrainEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    mesh.Draw();

                }
            }


            m_terrainEffect.CurrentTechnique = m_terrainEffect.Techniques["TileTerrain"];
            Globals.GraphicsDevice.Indices = m_blockIndexBuffer;
            Globals.GraphicsDevice.SetVertexBuffer(m_blockVertexBuffer);

            foreach (EffectPass pass in m_terrainEffect.CurrentTechnique.Passes)
            {
                for (int j = 0; j < numSpans; ++j)
                {
                    for (int i = 0; i < numSpans; ++i)
                    {
                        Vector3 localPosition = new Vector3((m_blockSize) * i, 0, (m_blockSize) * j);

                        Vector3 worldPosition = localPosition + startPosition;

                        Vector3 minbb = new Vector3(worldPosition.X, -Globals.WorldHeight, worldPosition.Z);
                        Vector3 maxbb = minbb + blockSize;
                        maxbb.Y = Globals.WorldHeight;

                        BoundingBox bb = new BoundingBox(minbb, maxbb);

                        if (Globals.s_currentCameraFrustrum.Contains(bb) != ContainmentType.Disjoint)
                        {

                            Matrix transform = Matrix.CreateTranslation(startPosition);

                            m_terrainEffect.Parameters["WorldMatrix"].SetValue(transform);
                            m_terrainEffect.Parameters["FineTextureBlockOrigin"].SetValue(new Vector4(m_oneOverTextureWidth, m_oneOverTextureWidth, localPosition.X, localPosition.Z));

                            // need apply on inner level to make sure latest vals copied across
                            pass.Apply();
                            Globals.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_blockVertexBuffer.VertexCount, 0, m_blockIndexBuffer.IndexCount / 3);

                        }
                        else
                        {
                            int ibreak = 0;
                        }

                    }
                }
            }
            m_terrainEffect.Parameters["HeightMapTexture"].SetValue((Texture)null);

        }


        ///////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void InitialiseWorldGrid()
        {
            m_terrainSquareGrid = new TerrainSquare[Globals.WorldWidth * Globals.WorldWidth];
            for (int i = 0; i < m_terrainSquareGrid.Length; ++i)
            {
                m_terrainSquareGrid[i] = new TerrainSquare();
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void AddPeak(Vector3 point, float height)
        {
            point = WorldToLocal(point);
            AddPeak(point.X, point.Z, height);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////

        public void AddPeak(float x, float y, float height)
        {
            float defaultRadius = 15.0f;
            AddPeak(x, y, defaultRadius, height);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void AddPeak(float x, float z, float radius, float height)
        {
            //TerrainUpdater terrainUpdate = new TerrainUpdater(new Vector3(x, 0, z), radius, s_terrainMoveTime, height, this);
            //m_terrainUpdaters.Add(terrainUpdate);
            TerrainUpdater.ApplyImmediate(new Vector3(x, 0, z), radius, height, this);
            UpdateHeightMap();

        }
        ///////////////////////////////////////////////////////////////////////////////////////////////

        public virtual float GetHeightAtPointWorld(Vector3 point)
        {
            // straight down
            float result = GetHeightAtPointWorld(point.X, point.Z);
            return result;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////

        public virtual float GetHeightAtPointWorldSmooth(Vector3 point)
        {
            // straight down
            float result = GetHeightAtPointWorldSmooth(point.X, point.Z);
            return result;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////
        public void SetHeightAtPointWorld(ref Vector3 worldPoint)
        {
            Vector3 local = WorldToLocal(worldPoint);
            int localX = (int)local.X;
            int localZ = (int)local.Z;
            SetHeightAtPointLocal(localX, localZ, worldPoint.Y);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void SetHeightAtPointLocal(int localX, int localZ, float height)
        {
            Debug.Assert(localX >= 0 && localX <= Globals.WorldWidth);
            Debug.Assert(localZ >= 0 && localZ <= Globals.WorldWidth);
            //m_heightMap[(localZ * Globals.WorldWidth) + localX] = height;
            m_heightMap[(localZ * m_textureWidth) + localX] = height;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public virtual float GetHeightAtPointWorldSmooth(float x, float z)
        {
            // Use a similar technique to the shader and sample around
            float result = 0.0f;
            float tl = GetHeightAtPointWorld(x - 1, z - 1);
            float tc = GetHeightAtPointWorld(x - 1, z - 1);
            float tr = GetHeightAtPointWorld(x + 1, z - 1);

            float ml = GetHeightAtPointWorld(x - 1, z);
            float mc = GetHeightAtPointWorld(x - 1, z);
            float mr = GetHeightAtPointWorld(x + 1, z);

            float bl = GetHeightAtPointWorld(x - 1, z + 1);
            float bc = GetHeightAtPointWorld(x - 1, z + 1);
            float br = GetHeightAtPointWorld(x + 1, z + 1);

            result = (tl + tc + tr + ml + mc + mr + bl + bc + br) / 9.0f;

            //float tl = GetHeightAtPointWorld(x, z);
            //float tr = GetHeightAtPointWorld(x + 1, z);
            //float bl = GetHeightAtPointWorld(x, z + 1);
            //float br = GetHeightAtPointWorld(x + 1, z + 1);

            //float txdiff = MathHelper.Lerp(tl, tr, (x - (float)Math.Truncate(x)));
            //float bxdiff = MathHelper.Lerp(bl, br, (x - (float)Math.Truncate(x)));
            //float result = MathHelper.Lerp(txdiff, bxdiff, (z - (float)Math.Truncate(z)));
            return result;

        }


        public virtual float GetHeightAtPointWorld(float x, float z)
        {
            Vector3 local = WorldToLocal(new Vector3(x, 0, z));
            int localX = (int)local.X;
            int localZ = (int)local.Z;
            return GetHeightAtPointLocal(localX, localZ);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual float GetHeightAtPointLocal(int localX, int localZ)
        {
            Debug.Assert(localX >= 0 && localX <= Globals.WorldWidth);
            Debug.Assert(localZ >= 0 && localZ <= Globals.WorldWidth);
            //return m_heightMap[(localZ * Globals.WorldWidth) + localX];
            return m_heightMap[(localZ * m_textureWidth) + localX];
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual void BuildRandomLandscape()
        {
            int counter = 0;
            int increment = 1;
            int maxHills = Width;
            int maxInstanceHeight = 10;
            int maxRadius = 50;
            int currentHills = 0;
            while (currentHills++ < maxHills)
            {
                if (counter == 5)
                {
                    increment = -1;
                }
                else if (counter == 0)
                {
                    increment = 1;
                }
                counter += increment;
                int xpos = (int)((float)Globals.s_random.NextDouble() * Width);
                int ypos = (int)((float)Globals.s_random.NextDouble() * Breadth);
                float radius = ((float)Globals.s_random.NextDouble() * maxRadius);
                // don't want too small a radius
                if (radius < 5f)
                {
                    radius = 5f;
                }


                float height = ((float)Globals.s_random.NextDouble() * maxInstanceHeight);
                bool up = (float)Globals.s_random.NextDouble() > 0.5;
                if (!up)
                {
                    height = -height;
                }


                TerrainUpdater.ApplyImmediate(new Vector3(xpos, 0, ypos), radius, height, this);
            }

            UpdateHeightMap();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void ToggleHeightMethod()
        {
            m_defaultHeightMethod = !m_defaultHeightMethod;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void SetHeightForArea(Vector3 startPosition,int width,int breadth,float height)
        {
            Vector3 baseVec = new Vector3(startPosition.X, height, startPosition.Z);
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < breadth; ++j)
                {
                    Vector3 vec = baseVec + new Vector3(i, 0, j);
                    SetHeightAtPointWorld(ref vec);
                }
            }
            UpdateHeightMap();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public TerrainSquare GetTerrainSquareAtPointWorld(ref Vector3 worldPoint)
        {
            Vector3 local = worldPoint - BoundingBox.Min;
            int localX = (int)local.X;
            int localZ = (int)local.Z;
            return GetTerrainSquareAtPointLocal(localX, localZ);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public TerrainSquare GetTerrainSquareAtPointLocal(int localX, int localZ)
        {
            Debug.Assert(localX >= 0 && localX < Globals.WorldWidth);
            Debug.Assert(localZ >= 0 && localZ < Globals.WorldWidth);
            return m_terrainSquareGrid[(localZ * Globals.WorldWidth) + localX];
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public bool HasTerrainChanged()
        {
            return m_terrainHasChanged;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void ClearTerrainChanged()
        {
            m_terrainHasChanged = false;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void SetTerrainTypeAndOccupier(Vector3 position, TerrainType terrainType, GameObject occupier)
        {
            TerrainSquare square = GetTerrainSquareAtPointWorld(ref position);
            square.Type = terrainType;
            square.Occupier = occupier;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void SetTerrainTypeOld(TerrainSquare terrainSquare, float val)
        {
            TerrainType result = TerrainType.water;
            if (val <= -5.0f)
            {
                result = TerrainType.deepwater;
            }
            else if (val > -5.0f && val <= -2.0f)
            {
                result = TerrainType.water;
            }
            else if (val > -2.0f && val <= 0.0f)
            {
                result = TerrainType.shallowwater;
            }
            else
                if (val > 0.0f && val <= 1.0f)
                {
                    result = TerrainType.beach;
                }
                else
                    if (val > 1.0f && val <= 3.0f)
                    {
                        result = TerrainType.grass;
                    }
                    else
                        if (val > 3.0f && val <= 5.0f)
                        {
                            result = TerrainType.grass2;
                        }
                        else
                            if (val > 5.0f && val <= 8.0f)
                            {
                                result = TerrainType.rock;
                            }
                            else
                                if (val > 8.0f)
                                {
                                    result = TerrainType.ice;
                                }
            terrainSquare.Type = result;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public bool IsPointInTerrain(ref Vector3 point)
        {
            return ((point.X >= 0.0f && point.X <= Width) && (point.Z >= 0.0f && point.Z <= Breadth));

        }
        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void AssertPointInTerrain(ref Vector3 point)
        {
            //Debug.Assert(isPointInTerrain(ref point), "Point not in terrain");
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void Update(GameTime gameTime)
        {
            // go through and adjust our current and target heights to make the landscape move nicely.
            base.Update(gameTime);
            bool terrainChanged = false; ;
            foreach (TerrainUpdater terrainUpdate in m_terrainUpdaters)
            {
                terrainUpdate.Update(gameTime);
                if (!terrainUpdate.Complete())
                {
                    terrainUpdate.ApplyToTerrain();
                    terrainChanged = true;
                }
                else
                {
                    m_terrainUpdatersRemove.Add(terrainUpdate);
                }
            }

            foreach (TerrainUpdater terrainUpdate in m_terrainUpdatersRemove)
            {
                m_terrainUpdaters.Remove(terrainUpdate);
            }

            if (terrainChanged)
            {
                UpdateHeightMap();
            }

            m_terrainUpdatersRemove.Clear();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public TerrainSquare[] TerrainSquares
        {
            get { return m_terrainSquareGrid; }
            set { m_terrainSquareGrid = value; }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public Vector3 WorldToLocal(Vector3 worldPoint)
        {
            return (worldPoint - BoundingBox.Min);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public Vector3 LocalToWorld(Vector3 localPoint)
        {
            return (localPoint + BoundingBox.Min);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public Vector3 GetRandomWorldPositionXZ()
        {
            Vector3 result = new Vector3();
            result.X = ((float)Globals.s_random.NextDouble() * Width);
            result.Z = ((float)Globals.s_random.NextDouble() * Breadth);
            result.Y = GetHeightAtPointLocal((int)result.X, (int)result.Z);
            return LocalToWorld(result);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public Vector3 GetRandomWorldPositionXZWithRange(Vector3 position, float distance)
        {
            Vector3 result = new Vector3();
            float sign = Globals.s_random.NextDouble() > 0.5f ? 1.0f : -1.0f;
            result.X = position.X + (sign * ((float)Globals.s_random.NextDouble() * distance));
            result.Z = position.Z + (sign * ((float)Globals.s_random.NextDouble() * distance));
            return result;
            //return LocalToWorld(result);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public Vector3 GetRandomWorldPositionWithRange(Vector3 position, float distance)
        {
            Vector3 result = new Vector3();
            float sign = Globals.s_random.NextDouble() > 0.5f ? 1.0f : -1.0f;
            result.X = position.X + (sign * ((float)Globals.s_random.NextDouble() * distance));
            result.Z = position.Z + (sign * ((float)Globals.s_random.NextDouble() * distance));

            result.Y = GetHeightAtPointWorld((int)result.X, (int)result.Z);
            
            return result;
            //return LocalToWorld(result);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	
        public void ClampToTerrain(ref Vector3 position)
        {
            Vector3 local = WorldToLocal(position);


            if (Globals.TerrainWrapEnabled)
            {
                // wrap the position
                if (local.X < 0)
                {
                    local.X += Width;
                }
                if (local.X > Width)
                {
                    local.X -= Width;
                }
                if (local.Z < 0)
                {
                    local.Z += Width;
                }
                if (local.Z > Width)
                {
                    local.Z -= Width;
                }
            }
            else
            {
                local.X = MathHelper.Clamp(local.X, 0, Width);
                local.Z = MathHelper.Clamp(local.Z, 0, Width);
            }

            position = LocalToWorld(local);
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////	

        // simple terrain with two levels
        public void BuildTestTerrain1()
        {
            for (int z = 0; z < m_textureWidth; ++z)
            {
                for (int x = 0; x < m_textureWidth / 2; ++x)
                {
                    m_heightMap[(z * m_textureWidth) + x] = 3.0f;
                }
            }
            UpdateHeightMap();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void UpdateHeightMap()
        {
            m_terrainHasChanged = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public float TerrainMoveTime
        {
            get { return s_terrainMoveTime; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void BuildFractalLandscape()
        {
            m_heightMap = new float[m_textureWidth * m_textureWidth];
            float height = 10.0f;
            float smoothness = 0.7f;
            FractalUtil.Fill2DFractArray(m_heightMap, m_textureWidth - 1, 1, height, smoothness);
            UpdateHeightMap();
        }






        ///////////////////////////////////////////////////////////////////////////////////////////////	

        private TerrainSquare[] m_terrainSquareGrid;
        private float[] m_heightMap;

        private List<TerrainUpdater> m_terrainUpdaters = new List<TerrainUpdater>();
        private List<TerrainUpdater> m_terrainUpdatersRemove = new List<TerrainUpdater>();




        private bool m_defaultHeightMethod = true;
        private bool m_terrainHasChanged;
        private float m_maxCoverage = 0.65f;
        private float m_minIslandSize = 5.0f;
        private float m_maxIslandSize = 15.0f;

        // the time taken for the complete terrain move.
        private float s_terrainMoveTime = 5.0f;

        const int m_blockSize = 64;
        const int m_textureWidth = Globals.WorldWidth + 1;
        const float m_oneOverTextureWidth = 1f / (m_textureWidth - 1);


        const int m_multiplier = 1;
        const float m_oneOver = 1f / (float)(m_multiplier);


        VertexBuffer m_blockVertexBuffer;
        IndexBuffer m_blockIndexBuffer;

        Effect m_terrainEffect;
        Effect m_normalsEffect;

        RasterizerState m_rasterizerState;
        Texture2D m_heightMapTexture;

        RenderTarget2D m_normalMapRenderTarget;

        ScreenQuad m_helperScreenQuad;

        Texture2D m_baseTexture;
        Texture2D m_noiseTexture;
        Texture2D m_portalTexture;

        RasterizerState m_noCullState;

        public bool ShouldCollideWith(ICollideable partner)
        {
            return true;
        }


        public Texture2D BaseTexture
        {
            get { return m_baseTexture; }
        }
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////	

    public struct TerrainUpdater
    {
        public TerrainUpdater(Vector3 position, float radius, float totalTime, float totalDeflection, Terrain terrain)
        {
            m_terrain = terrain;
            m_positionLocal = position;
            m_updateDeflection = 0f;

            BoundingBox terrainBB = m_terrain.BoundingBox;

            // need to adjust position based on midpoint of terrain
            //m_position -= new Vector3(CommonSettings.worldWidth / 2, 0, CommonSettings.worldBreadth / 2);
            m_radius = radius;
            m_totalTime = totalTime;
            m_totalDeflection = totalDeflection;
            m_currentTime = 0f;


            m_minX = (int)System.Math.Max(0, position.X - radius);
            m_maxX = (int)System.Math.Min(Globals.WorldWidth, position.X + radius);
            m_minZ = (int)System.Math.Max(0, position.Z - radius);
            m_maxZ = (int)System.Math.Min(Globals.WorldWidth, position.Z + radius);
        }


        public static void ApplyImmediate(Vector3 position, float radius, float totalDeflection, Terrain terrain)
        {
            int minX = (int)System.Math.Max(0, position.X - radius);
            int maxX = (int)System.Math.Min(Globals.WorldWidth, position.X + radius);
            int minZ = (int)System.Math.Max(0, position.Z - radius);
            int maxZ = (int)System.Math.Min(Globals.WorldWidth, position.Z + radius);

            float floatRadius2 = radius * radius;
            for (int j = minZ; j < maxZ; j++)
            {
                for (int i = minX; i < maxX; i++)
                {
                    Vector3 worldPoint = new Vector3(i, 0, j);
                    Vector3 diff = worldPoint - position;
                    float diffLength2 = diff.LengthSquared();
                    if (diffLength2 < floatRadius2)
                    {
                        float lerpValue = (floatRadius2 - diffLength2) / floatRadius2;
                        lerpValue = (float)Math.Pow(lerpValue, 4f);
                        lerpValue = (float)Math.Sin(MathUtil.SIMD_HALF_PI * lerpValue);
                        //lerpValue += 1.0f;

                        // play with lerp value to smooth the terrain?
                        //                          lerpValue = (float)Math.Sqrt(lerpValue);
                        //lerpValue *= lerpValue;
                        //lerpValue = (float)Math.Pow(lerpValue, 4f);
                        // ToDo - fractal hill generation.
                        float currentHeight = terrain.GetHeightAtPointLocal(i, j);
                        //float oldHeight = getHeightAtPoint(i, j);
                        float newHeight = currentHeight + (totalDeflection * lerpValue);

                        newHeight = MathHelper.Clamp(newHeight, -Globals.WorldHeight, Globals.WorldHeight);
                        terrain.SetHeightAtPointLocal(i, j, newHeight);
                    }
                }
            }
        }


        public void ApplyToTerrain()
        {
            if (m_currentTime < m_totalTime)
            {
                float floatRadius2 = m_radius * m_radius;
                for (int j = m_minZ; j < m_maxZ; j++)
                {
                    for (int i = m_minX; i < m_maxX; i++)
                    {
                        Vector3 worldPoint = new Vector3(i, 0, j);
                        Vector3 diff = worldPoint - m_positionLocal;
                        float diffLength2 = diff.LengthSquared();
                        if (diffLength2 < floatRadius2)
                        {
                            float lerpValue = (floatRadius2 - diffLength2) / floatRadius2;
                            lerpValue = (float)Math.Pow(lerpValue, 1.5f);
                            float currentHeight = m_terrain.GetHeightAtPointLocal(i, j);
                            float newHeight = currentHeight + (m_updateDeflection * lerpValue);

                            newHeight = MathHelper.Clamp(newHeight, -Globals.WorldHeight, Globals.WorldHeight);
                            m_terrain.SetHeightAtPointLocal(i, j, newHeight);
                        }
                    }
                }
            }
        }


        public void Update(GameTime gameTime)
        {
            float timeStep = (float)gameTime.ElapsedGameTime.TotalSeconds;
            m_currentTime += timeStep;
            m_updateDeflection = (timeStep / m_totalTime) * m_totalDeflection;
        }

        public bool Complete()
        {
            return m_currentTime > m_totalTime;
        }



        private Terrain m_terrain;
        private Vector3 m_positionLocal;
 
        private float m_radius;
        private float m_totalTime;
        private float m_currentTime;
        private float m_totalDeflection;
        private float m_updateDeflection;
        int m_minX;
        int m_maxX;
        int m_minZ;
        int m_maxZ;
    }






}