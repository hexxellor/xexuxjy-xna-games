﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.control;
using Dhpoware;
using Gladius.combat;
using Gladius.util;
using Gladius.actors;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gladius
{
    public class Globals
    {
        public const float MovementStepTime = 2f;

        public static UserControl UserControl;
        public static CameraComponent Camera;
        public static CombatEngine CombatEngine;
        public static EventLogger EventLogger;
        public static MovementGrid MovementGrid;
        public static AttackBar AttackBar;
        public static PlayerChoiceBar PlayerChoiceBar;
        public static SoundManager SoundManager;


        public static BoundingFrustum BoundingFrustum;


        public static AttackSkillDictionary AttackSkillDictionary;


        public static ThreadSafeContentManager GlobalContentManager;


        public static bool NextToTarget(BaseActor from, BaseActor to)
        {
            Debug.Assert(from != null && to != null);
            if (from != null && to!= null)
            {
                Point fp = from.CurrentPosition;
                Point tp = to.CurrentPosition;
                Vector3 diff = new Vector3(tp.X, 0, tp.Y) - new Vector3(fp.X, 0, fp.Y);
                return diff.LengthSquared() == 1f;
            }
            return false;
        }



        public static void DrawCameraDebugText(SpriteBatch spriteBatch,SpriteFont spriteFont,int fps)
        {
            string text = null;
            StringBuilder buffer = new StringBuilder();
            Vector2 fontPos = new Vector2(1.0f, 1.0f);
            buffer.AppendFormat("FPS: {0} \n", fps);
            buffer.Append("Camera:\n");
            buffer.AppendFormat("  Behavior: {0}\n", Globals.Camera.CurrentBehavior);
            buffer.AppendFormat("  Position: x:{0} y:{1} z:{2}\n",
                Globals.Camera.Position.X.ToString("#0.00"),
                Globals.Camera.Position.Y.ToString("#0.00"),
                Globals.Camera.Position.Z.ToString("#0.00"));
            buffer.AppendFormat("  Velocity: x:{0} y:{1} z:{2}\n",
                Globals.Camera.CurrentVelocity.X.ToString("#0.00"),
                Globals.Camera.CurrentVelocity.Y.ToString("#0.00"),
                Globals.Camera.CurrentVelocity.Z.ToString("#0.00"));

            buffer.AppendFormat("  Forward: x:{0} y:{1} z:{2}\n",
                Globals.Camera.ViewDirection.X.ToString("#0.00"),
                Globals.Camera.ViewDirection.Y.ToString("#0.00"),
                Globals.Camera.ViewDirection.Z.ToString("#0.00"));

            buffer.AppendFormat("  Rotation speed: {0}\n",
                Globals.Camera.RotationSpeed.ToString("#0.00"));

            if (Globals.Camera.PreferTargetYAxisOrbiting)
                buffer.Append("  Target Y axis orbiting\n\n");
            else
                buffer.Append("  Free orbiting\n\n");
            //if(Globals.MovementGrid != null)
            //{
            //    buffer.AppendFormat("Cursor Pos : [{0},{1}]", Globals.MovementGrid.CurrentPosition.X, Globals.MovementGrid.CurrentPosition.Y);
            //}

            text = buffer.ToString();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.DrawString(spriteFont, text, fontPos, Color.Yellow);
            spriteBatch.End();
        }

        public static void RemapModel(Model model, Effect effect)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
            }
        }



    }
}
