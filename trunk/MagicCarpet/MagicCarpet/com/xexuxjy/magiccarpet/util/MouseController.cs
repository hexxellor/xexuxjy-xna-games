﻿using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;

namespace com.xexuxjy.magiccarpet.util
{
    public class MouseController 
    {
        public MouseController()
        {

        }

        //----------------------------------------------------------------------------------------------

        public void HandleInput(InputState inputState)
        {
            bool leftReleased = WasReleased(ref inputState.LastMouseState, ref inputState.CurrentMouseState, 0);
            bool rightReleased = WasReleased(ref inputState.LastMouseState, ref inputState.CurrentMouseState, 2);


            if ( leftReleased || rightReleased)
            {
                Vector3 startPos = Globals.Camera.Position;
                Vector3 direction = Globals.Camera.ViewDirection;
                if(leftReleased)
                {
                    if (Globals.Player != null)
                    {
                        Globals.Player.CastSpell1(startPos, direction);
                    }
                }
                if(rightReleased)
                {
                    if (Globals.Player != null)
                    {
                        Globals.Player.CastSpell2(startPos, direction);
                    }
                }
            }

            // add something to draw and test collision?
            if (true)
            {
                if (Globals.CollisionManager != null)
                {

                    int rayLength = 100;
                    int normalLength = 10;
                    Vector3 startPos = Globals.Camera.Position;
                    Vector3 direction = Globals.Camera.ViewDirection;
                    Vector3 endPos = startPos + (direction * rayLength);

                    Vector3 collisionPoint = Vector3.Zero;
                    Vector3 collisionNormal = Vector3.Zero;

                    if (Globals.DebugDraw != null)
                    {
                        Vector3 rayColor = new Vector3(1, 1, 1);
                        Vector3 normalColor = new Vector3(1, 0, 0);
                        Globals.DebugDraw.DrawLine(ref startPos, ref endPos, ref rayColor);

                        Vector3 location = Globals.DebugTextCamera;
                        Vector3 colour = new Vector3(1, 1, 1);

                        if (Globals.CollisionManager.CastRay(startPos, endPos, ref collisionPoint, ref collisionNormal))
                        {
                            Globals.cameraHasGroundContact = true;
                            Globals.cameraGroundContactPoint = collisionPoint;
                            Globals.cameraGroundContactNormal = collisionNormal;

                            
                            Vector3 normalStart = collisionPoint;
                            Vector3 normalEnd = collisionPoint + (collisionNormal * normalLength);
                            Globals.DebugDraw.DrawLine(ref normalStart, ref normalEnd, ref normalColor);
                            Globals.DebugDraw.DrawText(String.Format("Camera Pos[{0} Forward[{1}] Collide[{2}] Normal[{3}].", startPos, direction,collisionPoint,collisionNormal), location, colour);

                        }
                        else
                        {
                            Globals.cameraHasGroundContact = false;
                            Globals.DebugDraw.DrawText(String.Format("Camera Pos[{0} Forward[{1}].", startPos, direction), location, colour);
                        }

                    }
                }

            }
        }


        //----------------------------------------------------------------------------------------------

        private bool WasReleased(ref MouseState old, ref MouseState current, int buttonIndex)
        {
            if (buttonIndex == 0)
            {
                return old.LeftButton == ButtonState.Pressed && current.LeftButton == ButtonState.Released;
            }
            if (buttonIndex == 1)
            {
                return old.MiddleButton == ButtonState.Pressed && current.MiddleButton == ButtonState.Released;
            }
            if (buttonIndex == 2)
            {
                return old.RightButton == ButtonState.Pressed && current.RightButton == ButtonState.Released;
            }
            return false;
        }

        //----------------------------------------------------------------------------------------------

        //public void GenerateMouseEvents(ref MouseState oldState, ref MouseState newState)
        //{
        //    MouseFunc(ref oldState, ref newState);
        //    MouseMotionFunc(ref newState);
        //}


        bool m_invertY = false;
    }
}