﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.interfaces;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.spells
{
    public class Convert : MovingSpell , ICollideable
    {
        public bool ShouldCollideWith(ICollideable partner)
        {
            bool shouldCollide = false;
            GameObject target = partner.GetGameObject();
            if (target is ManaBall)
            {
                // if it's owned by someone else or is non-aligned
                if(target.Owner != Owner)
                {
                    shouldCollide = true;
                }
            }
            return shouldCollide;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public int  GetCollisionMask()
        {
            return 0;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ProcessCollision(ICollideable partner)
        {
            // Double check?
            if (ShouldCollideWith(partner))
            {

                GameObject target = partner.GetGameObject();
                target.Owner = Owner;
                Cleanup();
            }
        
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////

    }
}