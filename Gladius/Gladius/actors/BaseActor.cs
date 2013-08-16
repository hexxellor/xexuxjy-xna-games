﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xexuxjy.Gladius.util;
using Microsoft.Xna.Framework;
using Gladius.combat;
using Gladius.actions;
using Gladius.renderer;
using Microsoft.Xna.Framework.Graphics;
using Dhpoware;
using Gladius.renderer.animation;
using Microsoft.Xna.Framework.Content;
using Gladius.util;

namespace Gladius.actors
{
    public class BaseActor : DrawableGameComponent
    {
        public BaseActor(Game game) : base(game)
        {
            m_animatedModel = new AnimatedModel(this);
        }


        public String Name
        {
            get;
            set;
        }

        public String DebugName
        {
            get;
            set;
        }

        public String ModelName
        {
            get;
            set;
        }

        public Arena Arena
        {
            get;
            set;
        }

        public float GetAttributeValue(GameObjectAttributeType attributeType)
        {
            return m_attributeDictionary[attributeType].CurrentValue;
        }

        public void SetAttributeValue(GameObjectAttributeType attributeType,float val)
        {
            m_attributeDictionary[attributeType].CurrentValue = val;
        }

        public void PlayAnimation(String name)
        {
            if (m_animatedModel != null)
            {
                m_animatedModel.PlayAnimation(name);
            }
        }

        public void LoadContent(ContentManager contentManager)
        {
            if (m_animatedModel != null)
            {
                m_animatedModel.LoadContent(contentManager);
                m_animatedModel.PlayAnimation("Walk");
            }
        }


        public Point CurrentPoint
        {
            get;
            set;
        }


        public Vector3 Position
        {
            get
            {
                if (Arena != null)
                {
                    return Arena.ArenaToWorld(CurrentPoint);
                }
                else
                {
                    return new Vector3(CurrentPoint.X, 0, CurrentPoint.Y);
                }
            }
        }

        public Matrix World
        {
            get;
            set;
        }

        public BoundingBox BoundingBox
        {
            get;
            set;
        }

        public String Team
        {
            get;
            set;
        }

        public ActorClass ActorClass
        {
            get;
            set;
        }

        public void TakeDamage(AttackResult attackResult)
        {
            if(attackResult.resultType != AttackResultType.Miss)
            {
                m_attributeDictionary[GameObjectAttributeType.Health].CurrentValue -= attackResult.damageDone;
            }
        }


        public override void Update(GameTime gameTime)
        {
            if (m_animatedModel != null)
            {
                m_animatedModel.Update(gameTime);
            }

        }

        public virtual void CheckState()
        {
            if(m_attributeDictionary[GameObjectAttributeType.Health].CurrentValue <= 0f)
            {
                StartAction(ActionTypes.Death);
            }
        }

        public virtual void StartAction(ActionTypes actionType)
        {
            Globals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] [{1}] started.", DebugName, actionType));
        }

        public virtual void StopAction(ActionTypes actionType)
        {
            Globals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] [{1}] stopped.", DebugName, actionType));
        }

        public override void Draw(GameTime gameTime)
        {
            Draw(Game.GraphicsDevice, Globals.Camera, gameTime);
        }

        public virtual void Draw(GraphicsDevice device, ICamera camera, GameTime gameTime)
        {
            if (m_animatedModel != null)
            {
                m_animatedModel.Draw(device, camera, gameTime);
            }
        }



        public List<AttackSkill> m_knownAttacks;
        public Dictionary<GameObjectAttributeType,BoundedAttribute> m_attributeDictionary;
        public AnimatedModel m_animatedModel;

    }

    public enum ActorClass
    {
        Light,
        Medium,
        Heavy
    }

}