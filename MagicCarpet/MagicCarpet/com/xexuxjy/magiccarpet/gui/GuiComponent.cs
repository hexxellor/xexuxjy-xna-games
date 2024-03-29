﻿using Microsoft.Xna.Framework;
using GameStateManagement;
using System;
using Microsoft.Xna.Framework.Graphics;
using BulletXNA;

namespace com.xexuxjy.magiccarpet.gui
{
    public abstract class GuiComponent : DrawableGameComponent
    {
        public GuiComponent(Point topLeft,int width)
            : base(Globals.Game)
        {
            m_componentTopCorner = topLeft;
            m_width = width;
            m_rectangle = new Rectangle(topLeft.X, topLeft.Y, width, width);
            m_textureUpdateNeeded = true;
            DrawOrder = Globals.GUI_DRAW_ORDER;
        }

        public virtual void HandleInput(InputState inputState)
        {
        }

        public override void Update(GameTime gameTime)
        {
            // empty on purpose.
        }

        public bool HasGuiControl
        {
            get{return m_hasGuiControl;}
            set{m_hasGuiControl = value;}
        }

        public override void Draw(GameTime gameTime)
        {
            CheckAndUpdateTexture();
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(m_texture, m_rectangle, Color.White);
            m_spriteBatch.End();
        }


        public virtual void CheckAndUpdateTexture()
        {
        }


        public static void DrawFilledCircle(Color[] texture, int step,int xPos, int yPos, int radius, Color foregroundColor, Color backgroundColor, bool clearFirst)
        {
            if (clearFirst)
            {
                for (int i = 0; i < texture.Length; ++i)
                {
                    texture[i] = backgroundColor;
                }
            }
            // ensure we can draw within the bounds??
            // Work out the minimum step necessary using trigonometry + sine approximation.
            int startX = xPos - radius;
            startX = Math.Max(0,startX);
            int endX = xPos + radius;
            endX = Math.Min(endX,step);
            int startY = yPos - radius;
            startY = Math.Max(0,startY);
            int endY = yPos + radius;
            endY = Math.Min(endY,step);

            int radiusSq = radius*radius;
            for(int j=startY;j<endY;++j)
            {

                int offset = j * step;
                int yoff = j - yPos;
                yoff *= yoff;

                for(int i=startX;i<endX;++i)
                {
                    int xoff = i - xPos;
                    if ((xoff * xoff) + yoff < radiusSq)
                    {
                        texture[offset + i] = foregroundColor;
                    }
                }
            }
        }


        public static void DrawFilledArc(Color[] texture, int step, int xPos, int yPos, int radius, float startAngle, float arcWidth,Color foregroundColor, Color backgroundColor, bool clearFirst)
        {
            if (clearFirst)
            {
                for (int i = 0; i < texture.Length; ++i)
                {
                    texture[i] = backgroundColor;
                }
            }
            // ensure we can draw within the bounds??
            // Work out the minimum step necessary using trigonometry + sine approximation.
            int startX = xPos - radius;
            startX = Math.Max(0, startX);
            int endX = xPos + radius;
            endX = Math.Min(endX, step);
            int startY = yPos - radius;
            startY = Math.Max(0, startY);
            int endY = yPos + radius;
            endY = Math.Min(endY, step);

            int radiusSq = radius * radius;
            for (int j = startY; j < endY; ++j)
            {

                int offset = j * step;
                int yoff = j - yPos;
                yoff *= yoff;

                for (int i = startX; i < endX; ++i)
                {
                    int xoff = i - xPos;
                    if ((xoff * xoff) + yoff < radiusSq)
                    {
                        int a = i - xPos;
                        int b = j - yPos;
                        float tan = (float)Math.Atan2(b,a);
                        // adjust range;
                        if (tan < 0)
                        {
                            tan += MathUtil.SIMD_2_PI;
                        }
                        if (tan >= startAngle && tan <= (startAngle + arcWidth))
                        {
                            texture[offset + i] = foregroundColor;
                        }
                    }
                }
            }
        }



        public static void DrawFilledRectangle(Color[] texture, int step, int xPos, int yPos, int width,int height, Color foregroundColor, Color backgroundColor, bool clearFirst)
        {
            if (clearFirst)
            {
                for (int i = 0; i < texture.Length; ++i)
                {
                    texture[i] = backgroundColor;
                }
            }
            // ensure we can draw within the bounds??
            // Work out the minimum step necessary using trigonometry + sine approximation.
            int startX = xPos;
            startX = Math.Max(0, startX);
            int endX = xPos + (width);
            endX = Math.Min(endX, step);
            int startY = yPos;
            startY = Math.Max(0, startY);
            int endY = yPos + (height);
            endY = Math.Min(endY, step);

            for (int j = startY; j < endY; ++j)
            {

                int offset = j * step;
                int yoff = j - yPos;
                yoff *= yoff;

                for (int i = startX; i < endX; ++i)
                {
                    int xoff = i - xPos;
                    texture[offset + i] = foregroundColor;

                }
            }
        }

        protected static void DrawBar(float currentVal, float maxVal, Color[] texture, int step, int xPos, int yPos, int width, int height, Color foregroundColor, Color backgroundColor, bool clearFirst)
        {
            // draw outline.
            DrawFilledRectangle(texture, step, xPos, yPos, width, height, foregroundColor, backgroundColor, false);
            // fill center
            int borderWidth = 2;
            DrawFilledRectangle(texture, step, xPos + borderWidth, yPos +borderWidth, width - 2*borderWidth, height - 2*borderWidth, backgroundColor, backgroundColor, false);
            // draw 'amount;

            float fillAmount = currentVal / maxVal;
            DrawFilledRectangle(texture, step, xPos, yPos, (int)(width * fillAmount), height, foregroundColor, backgroundColor, false);
        }


        
        protected Point m_componentTopCorner;
        protected Rectangle m_rectangle;
        protected SpriteBatch m_spriteBatch;
        protected int m_width;
        protected Texture2D m_texture;
        protected Color[] m_colorData;

        protected bool m_enabled;
        protected bool m_hasGuiControl;
        protected bool m_textureUpdateNeeded;

    }
}
