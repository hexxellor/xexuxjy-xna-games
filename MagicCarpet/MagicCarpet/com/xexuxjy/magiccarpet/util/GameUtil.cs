﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;
using BulletXNA.LinearMath;

namespace com.xexuxjy.magiccarpet.util
{
    public static class GameUtil
    {
        public static bool InRange(GameObject obj1, GameObject obj2, float dist)
        {
            //return (obj1.Position - obj2.Position).Length() <= dist;
            // can't do a simple point distance check so do it based on bounding boxes?
            BoundingBox obj1BB = obj1.BoundingBox;
            BoundingBox obj2BB = obj2.BoundingBox;

            // grow one of the boxes by dist and check for intersection?
            obj2BB.Min -= new IndexedVector3(dist);
            obj2BB.Max += new IndexedVector3(dist);

            return obj2BB.Contains(obj1BB) != ContainmentType.Disjoint;
        }

        public static IndexedVector3 DirectionToTarget(GameObject source, GameObject target)
        {
            return IndexedVector3.Normalize(target.Position - source.Position);
        }

        public static IndexedVector3 DirectionToTarget(IndexedVector3 source, GameObject target)
        {
            return IndexedVector3.Normalize(target.Position - source);
        }

        public static IndexedVector3 DirectionToTarget(GameObject source, IndexedVector3 target)
        {
            return IndexedVector3.Normalize(target - source.Position);
        }


    }
}
