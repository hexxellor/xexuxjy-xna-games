﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using BulletXNA.BulletCollision;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.interfaces;
using com.xexuxjy.magiccarpet.terrain;
using com.xexuxjy.magiccarpet.combat;

namespace com.xexuxjy.magiccarpet.spells
{
    public class SpellFireball : MovingSpell
    {
        public SpellFireball(GameObject owner)
            : base(owner)
        {

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public override CollisionFilterGroups GetCollisionFlags()
        {
            return (CollisionFilterGroups)GameObjectType.spell;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override CollisionFilterGroups GetCollisionMask()
        {
            return (CollisionFilterGroups)(GameObjectType.terrain | GameObjectType.magician | GameObjectType.balloon | GameObjectType.monster | GameObjectType.castle);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override float Mass
        {
            get
            {
                return 1f;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override bool ShouldCollideWith(ICollideable partner)
        {
            // stop on terrain
            if (partner is Terrain)
            {
                return true;
            }
            // of if we hit something that doesn't belong to us.
            if (partner.GetGameObject().Owner != Owner)
            {
                return true;
            }
            return false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override bool ProcessCollision(ICollideable partner, ManifoldPoint manifoldPoint)
        {
            // Double check?
            if (ShouldCollideWith(partner))
            {
                if (!(partner is Terrain))
                {
                    // do damge based on owner of spell.
                    partner.GetGameObject().Damaged(new DamageData(Owner,m_damage));

                }
                Cleanup();
            }
            return true;

        }


        private float m_damage = 20;
    }
}
