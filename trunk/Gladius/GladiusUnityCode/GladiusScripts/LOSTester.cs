﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;
namespace Gladius
{

    public class LOSTester : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            int initPoolSize = 20;
            for (int i = 0; i < initPoolSize; ++i)
            {
                FreeBox(GetBox());
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetStartAndEnd(Vector3 start, Vector3 end)
        {
            Point startp = new Point();//GladiusGlobals.Arena.Width
            Point endp = new Point();//
            //GladiusGlobals.Arena.GetLOSPoints(startp, endp, m_arenaPoints);
            ResetBoxes();
            BuildBoxes();
        }

        private void ResetBoxes()
        {
            int n = m_activeLines.Count;
            for (int i = 0; i < n; ++i)
            {
                FreeBox(m_activeLines[i]);
            }
            m_activeLines.Clear();
        }

        private void BuildBoxes()
        {
            foreach (Point p in m_arenaPoints)
            {
                Vector3 worldPos = GladiusGlobals.Arena.ArenaToWorld(p);
                VectorLine vl = GetBox();
                vl.drawTransform.position = worldPos;
                m_activeLines.Add(vl);
            }
        }



        public VectorLine BuildDefaultBox()
        {
            Vector3 dims = new Vector3(1, 2, 1);
            Vector3 halfDims = dims / 2f;
            Vector3[] corners = new Vector3[8];
            int count = 0;
            Vector3 position = Vector3.zero;
            corners[count++] = position + new Vector3(-halfDims.x, -halfDims.y, -halfDims.z);
            corners[count++] = position + new Vector3(halfDims.x, -halfDims.y, -halfDims.z);
            corners[count++] = position + new Vector3(halfDims.x, -halfDims.y, halfDims.z);
            corners[count++] = position + new Vector3(-halfDims.x, -halfDims.y, halfDims.z);
            corners[count++] = position + new Vector3(-halfDims.x, halfDims.y, -halfDims.z);
            corners[count++] = position + new Vector3(halfDims.x, halfDims.y, -halfDims.z);
            corners[count++] = position + new Vector3(halfDims.x, halfDims.y, halfDims.z);
            corners[count++] = position + new Vector3(-halfDims.x, halfDims.y, halfDims.z);

            VectorLine vectorLine = new VectorLine("3DLine", new Vector3[] { corners[0], corners[1], 
                corners[1], corners[2], 
                corners[2], corners[3], 
                corners[3], corners[0],
                corners[4], corners[5], 
                corners[5], corners[6], 
                corners[6], corners[7], 
                corners[7], corners[4],
                corners[0],corners[4],
                corners[1],corners[5],
                corners[2],corners[6],
                corners[3],corners[7]
        
        }, null, 4.0f);
            vectorLine.Draw3DAuto();
            return vectorLine;
        }

        public VectorLine GetBox()
        {
            VectorLine vl = null;
            if (m_boxPool.Count == 0)
            {
                vl = BuildDefaultBox();
                m_boxPool.Push(vl);
            }
            vl = m_boxPool.Pop();
            vl.active = true;
            return vl;
        }

        public void FreeBox(VectorLine vl)
        {
            vl.active = false;
            m_boxPool.Push(vl);
        }


        private Stack<VectorLine> m_boxPool = new Stack<VectorLine>();
        private List<Point> m_arenaPoints = new List<Point>();
        private List<VectorLine> m_activeLines = new List<VectorLine>();



    }
}