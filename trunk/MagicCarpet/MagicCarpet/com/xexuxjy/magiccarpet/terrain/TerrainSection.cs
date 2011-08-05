using System;
using System.Collections.Generic;
using System.Text;
using com.xexuxjy.magiccarpet.renderer;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.collision;
using System.Diagnostics;
using com.xexuxjy.magiccarpet.terrain;
using com.xexuxjy.magiccarpet;
using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace com.xexuxjy.magiccarpet.terrain
{
    public class TerrainSection : WorldObject
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public TerrainSection(Terrain terrain, int sectorX, int sectorZ, int worldSpanX, int worldSpanZ, Game game)
            : base(new Vector3(), game)
        {
            // cheat and build once as they should all be uniform.
            BuildSectionIndices(worldSpanX, worldSpanZ);
            
            m_terrain = terrain;
            m_sectorX = sectorX;
            m_sectorZ = sectorZ;
            m_worldSpanX = worldSpanX;
            m_worldSpanZ = worldSpanZ;
            m_xVerts = m_worldSpanX;
            m_zVerts = m_worldSpanZ;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize()
        {
            // do this before base initialise otherwise we'll be added to the collision system.
            //m_collider = false;

            int stepSizeX = m_worldSpanX / m_xVerts;
            int stepSizeZ = m_worldSpanZ / m_zVerts;

            float sectionWidth = m_terrain.Width / m_terrain.NumSectionsX;
            float sectionBreadth = m_terrain.Breadth / m_terrain.NumSectionsZ;

            Vector3 transformVector = new Vector3();
            transformVector.X = m_sectorX * sectionWidth;
            transformVector.Z = m_sectorZ * sectionBreadth;
            // up the bounds of the object.
            Vector3 minBound = transformVector;
            minBound.Y = Globals.containmentMinHeight;
            Vector3 maxBound = transformVector;
            maxBound.X += sectionWidth;
            maxBound.Z += sectionBreadth;
            maxBound.Y = Globals.containmentMaxHeight;

            // need position to be the middle of it
            transformVector.X += sectionWidth / 2.0f;
            transformVector.Z += sectionBreadth / 2.0f;


            Position = transformVector;
            BuildRenderer();
            BuildCollisionObject();

            base.Initialize();

            Id = String.Format("TerrainSection([{0}],[{1}])", m_sectorX, m_sectorZ);
            SetDirty();


        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public DefaultRenderer Renderer
        {
            get { return m_terrainSectionRenderer; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public void BuildRenderer()
        {
            m_terrainSectionRenderer = new TerrainSectionRenderer((MagicCarpet)Game, this, m_terrain);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetDirty()
        {
            m_isDirty = true;
            // reset this here - though may cause oddness with multiple height events.
            m_terrainMoveTime = 0.0f;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ClearDirty()
        {
            m_isDirty = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool IsDirty()
        {
            return m_isDirty;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // FIXME - Sort out this god awful system of percentages and world points!
        public Vector3 GetNormalAtPoint(int x, int z)
        {
            if (null != m_terrainSectionRenderer)
            {
                return m_terrainSectionRenderer.GetNormalAtPoint(x, z, m_worldSpanX, m_worldSpanZ);
            }
            return Vector3.Zero;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds * m_terrain.TerrainMoveTime;


            m_terrainMoveTime += delta;
            m_terrainMoveTime = Math.Min(m_terrainMoveTime, 1.0f);
            base.Update(gameTime);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public float TerrainMoveTime { get { return m_terrainMoveTime; } }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool ContainsPoint(Vector3 point)
        {
            // FIXME - Do a proper check.
            return true;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void BuildCollisionObject()
        {
            TerrainSectionRenderer.MorphingTerrainVertexFormatStruct[] morphingVertices = m_terrainSectionRenderer.Vertices;
            m_plainVertices = new ObjectArray<Vector3>(morphingVertices.Length);
            for (int i = 0; i < morphingVertices.Length; ++i)
            {
                m_plainVertices[i] = morphingVertices[i].Position;
            }
            int totalTriangles = (m_xVerts-1) * (m_zVerts-1) * 2;

            TriangleIndexVertexArray indexVertexArrays = new TriangleIndexVertexArray(totalTriangles,
                s_indices, 1, m_plainVertices.Count, m_plainVertices, 1);

            CollisionShape heightFieldTerrain = new BvhTriangleMeshShape(indexVertexArrays, true, true);
            m_rigidBody = Globals.CollisionManager.LocalCreateRigidBody(0f, Matrix.CreateTranslation(m_position), heightFieldTerrain, true);

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ObjectArray<int> GetSectionIndices()
        {
            return s_indices;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void BuildSectionIndices(int numx, int numz)
        {
            int numberOfQuadsX = numx - 1;
            int numberOfQuadsZ = numz - 1;
            int stepSize = 6;

            if (s_indices == null)
            {
                s_indices = new ObjectArray<int>(numberOfQuadsX * numberOfQuadsZ * stepSize);
                for (int x = 0; x < numberOfQuadsX; x++)
                {
                    for (int y = 0; y < numberOfQuadsZ; y++)
                    {
                        int index = (x + y * (numberOfQuadsX)) * stepSize;
                        s_indices[index] = (x + y * numx);
                        s_indices[index + 1] = ((x + 1) + y * numx);
                        s_indices[index + 2] = ((x + 1) + (y + 1) * numx);

                        s_indices[index + 3] = (x + (y + 1) * numx);
                        s_indices[index + 4] = (x + y * numx);
                        s_indices[index + 5] = ((x + 1) + (y + 1) * numx);
                    }
                }
            }
        }

        protected Terrain m_terrain;
        protected TerrainSectionRenderer m_terrainSectionRenderer;
        protected TerrainSquare[][] m_terrainSquares;
        public int m_sectorX;
        public int m_sectorZ;
        public int m_xVerts;
        public int m_zVerts;
        public int m_worldSpanX;
        public int m_worldSpanZ;
        protected float m_terrainMoveTime; // counter used when adjusting terrain heights
        protected bool m_isDirty = false;
        protected ObjectArray<Vector3> m_plainVertices;
        protected static ObjectArray<int> s_indices;



    }
}
