﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Gladius.events;
using Microsoft.Xna.Framework.Graphics;
using Gladius.gamestatemanagement.screens;
using Microsoft.Xna.Framework.Content;
using Gladius.actors;
using Gladius.combat;
using Gladius.modes.arena;
using Gladius.renderer;
using Gladius.util;

namespace Gladius.control
{
    public class PlayerChoiceBar : BaseUIElement
    {
        const int numSkillSlots = 5;

        public override void LoadContent(ThreadSafeContentManager manager, GraphicsDevice device)
        {
            m_shieldBarBitMap = manager.Load<Texture2D>("UI/arena/ShieldSkilllbar");
            m_attackBarBitMap = manager.Load<Texture2D>("UI/arena/AttackSkillBar");

            m_skillBar2Bitmap = manager.Load<Texture2D>("UI/arena/SkillbarPart2");
            m_skillsBitmap = manager.Load<Texture2D>("UI/arena/SkillIcons");
            m_smallFont = manager.Load<SpriteFont>("UI/fonts/UIFontSmall");
            m_largeFont = manager.Load<SpriteFont>("UI/fonts/UIFontLarge");

            m_damageTypeTextures[DamageType.Physical] = manager.GetColourTexture(Color.Green);
            m_damageTypeTextures[DamageType.Light] = manager.GetColourTexture(Color.White); ;
            m_damageTypeTextures[DamageType.Dark] = manager.GetColourTexture(Color.Black);
            m_damageTypeTextures[DamageType.Fire] = manager.Load<Texture2D>("UI/arena/FireOpal");
            m_damageTypeTextures[DamageType.Water] = manager.Load<Texture2D>("UI/arena/AzureWaters");
            m_damageTypeTextures[DamageType.Earth] = manager.Load<Texture2D>("UI/arena/DarkAle");
            m_damageTypeTextures[DamageType.Air] = manager.Load<Texture2D>("UI/arena/LightMarble");

            m_attackSkills = new List<List<AttackSkill>>();
            for (int i = 0; i < numSkillSlots; ++i)
            {
                m_attackSkills.Add( new List<AttackSkill>());
            }
            m_currentAttackSkillLine = new List<AttackSkill>(numSkillSlots);
            EventManager.BaseActorChanged += new EventManager.BaseActorSelectionChanged(EventManager_BaseActorChanged);
        }

        public override void DrawElement(GameTime gameTime,GraphicsDevice graphiceDevice,SpriteBatch spriteBatch)
        {
            Rectangle barRect = new Rectangle(Rectangle.X,Rectangle.Y,m_shieldBarBitMap.Width,m_shieldBarBitMap.Height);

            Vector2 textDims = m_largeFont.MeasureString(CurrentActor.Name);
            Vector2 textPos = new Vector2(Rectangle.X+5,Rectangle.Y-textDims.Y-3);

            GraphicsHelper.DrawShadowedText(spriteBatch, m_largeFont, CurrentActor.Name, textPos);

            DrawSkillBar1(spriteBatch, m_shieldBarBitMap,barRect, CurrentActor.ArmourAffinityType, CurrentActor.Health, CurrentActor.MaxHealth, CurrentActor.Affinity, CurrentActor.MaxAffinity);
            barRect.X += m_shieldBarBitMap.Width;
            DrawSkillBar1(spriteBatch, m_attackBarBitMap, barRect, CurrentActor.WeaponAffinityType, CurrentActor.ArenaSkillPoints,CurrentActor.MaxArenaSkillPoints,1,1);

            barRect.X += m_shieldBarBitMap.Width;
            barRect.X += 20;

            DrawSkillBar2(spriteBatch, barRect, m_currentAttackSkillLine, null,null);
        }

        public override void RegisterListeners()
        {
            EventManager.ActionPressed += new EventManager.ActionButtonPressed(EventManager_ActionPressed);
        }


        public override void UnregisterListeners()
        {
            //EventManager.ActionPressed -= new event ActionButtonPressed();
            EventManager.ActionPressed -= new EventManager.ActionButtonPressed(EventManager_ActionPressed);

        }


        void EventManager_BaseActorChanged(object sender, BaseActorChangedArgs e)
        {
            CurrentActor = e.New;
        }

        public void BuildDataForActor()
        {
            foreach (List<AttackSkill> list in m_attackSkills)
            {
                list.Clear();
            }
            m_currentAttackSkillLine.Clear();


            foreach (AttackSkill attackSkill in CurrentActor.AttackSkills)
            {
                m_attackSkills[attackSkill.SkillRow].Add(attackSkill);
            }

            for (int i = 0; i < numSkillSlots; ++i)
            {
                m_currentAttackSkillLine.Add(m_attackSkills[i][0]);
            }
            m_actionCursor = new Point();
        }


        void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
        {
            if (!InputAllowed)
            {
                return;
            }

            if(TurnManager.CurrentControlState== Gladius.control.TurnManager.ControlState.ChoosingSkill)
            {
                HandleSkillChoiceAction(e);
            }
            else if (TurnManager.CurrentControlState == Gladius.control.TurnManager.ControlState.UsingGrid)
            {
                HandleMovementGridAction(e);
            }
        }



        private void HandleSkillChoiceAction(ActionButtonPressedArgs e)
        {
            switch (e.ActionButton)
            {

                case (ActionButton.ActionLeft):
                    {
                        CursorLeft();
                        break;
                    }
                case (ActionButton.ActionRight):
                    {
                        CursorRight();
                        break;
                    }
                case (ActionButton.ActionUp):
                    {
                        CursorUp();
                        break;
                    }
                case (ActionButton.ActionDown):
                    {
                        CursorDown();
                        break;
                    }
                case (ActionButton.ActionButton1):
                    {
                        if (CurrentlySelectedSkill.NeedsGrid)
                        {
                            MovementGrid.CurrentPosition = CurrentActor.CurrentPosition;
                            CurrentActor.CurrentAttackSkill = CurrentlySelectedSkill;
                            TurnManager.CurrentControlState = Gladius.control.TurnManager.ControlState.UsingGrid;                            
                        }
                        else
                        {
                            //ConfirmAction();
                        }
                        break;
                    }
                // cancel
                case (ActionButton.ActionButton2):
                    {
                        CancelAction();
                        break;
                    }

            }
        }


        private void UpdateCursor(Point delta)
        {
            m_actionCursor += delta;

            m_actionCursor.X = (m_actionCursor.X + m_attackSkills.Count) % m_attackSkills.Count;

            if (delta.X != 0)
            {
                m_actionCursor.Y = 0;
            }
            m_actionCursor.Y = (m_actionCursor.Y + m_attackSkills[m_actionCursor.X].Count) % m_attackSkills[m_actionCursor.X].Count;
            m_currentAttackSkillLine[m_actionCursor.X] = m_attackSkills[m_actionCursor.X][m_actionCursor.Y];
            CurrentActor.CurrentAttackSkill = CurrentlySelectedSkill;
        }


        public void CursorLeft()
        {
            UpdateCursor(new Point(-1,0));
        }

        public void CursorRight()
        {
            UpdateCursor(new Point(1, 0));
        }

        public void CursorUp()
        {
            UpdateCursor(new Point(0, 1));
        }

        public void CursorDown()
        {
            UpdateCursor(new Point(0, -1));
        }


        private void DrawSkillBar1(SpriteBatch spriteBatch, Texture2D background,Rectangle rect, DamageType damageType, float bar1Value, float bar1MaxValue, float bar2Value, float bar2MaxValue)
        {
            int smallCircleDiameter = 16;
            int smallircleYOffset = 0;

            Rectangle skillRect1Dims = new Rectangle(33, 15, 107, 16);
            Rectangle skillRect2Dims = new Rectangle(33, 38, 107 , 16);

            Rectangle affinityRect = skillRect2Dims;
            GraphicsHelper.OffsetRect(ref rect, ref affinityRect);

            affinityRect.X -= (int)((float)smallCircleDiameter*1.0f);
            affinityRect.Y -= smallCircleDiameter/2;
            affinityRect.Width = affinityRect.Height = smallCircleDiameter;

            // draw 
            spriteBatch.Draw(m_damageTypeTextures[damageType], affinityRect, Color.White);

            Rectangle rect1 = new Rectangle(rect.X + skillRect1Dims.X,rect.Y + skillRect1Dims.Y + skillRect1Dims.Height,skillRect1Dims.Width,skillRect1Dims.Height);
            Rectangle rect2 = new Rectangle(rect.X + skillRect2Dims.X, rect.Y + skillRect2Dims.Y + skillRect2Dims.Height, skillRect2Dims.Width, skillRect2Dims.Height);

            spriteBatch.Draw(background, rect, Color.White);

            DrawMiniBar(spriteBatch,rect1, bar1Value, bar1MaxValue, Color.Green, Color.Black);
            DrawMiniBar(spriteBatch,rect2, bar2Value, bar2MaxValue, Color.Yellow, Color.Black);

        }


        private void DrawMiniBar(SpriteBatch spriteBatch,Rectangle baseRectangle, float val, float maxVal, Color color1, Color color2)
        {
            float fillPercentage = val / maxVal;

            int height = baseRectangle.Height;
            int ypos = baseRectangle.Y;
            int start = baseRectangle.X;

            int width = (int)(fillPercentage * baseRectangle.Width);
            spriteBatch.Draw(ArenaScreen.ContentManager.GetColourTexture(color1), new Rectangle(start, ypos, width, height), Color.White);
            start += width;
            width = baseRectangle.Width - width;
            spriteBatch.Draw(ArenaScreen.ContentManager.GetColourTexture(color2), new Rectangle(start, ypos, width, height), Color.White);
        }

        private void DrawSkillBar2(SpriteBatch spriteBatch, Rectangle rect, List<AttackSkill> skills,StringBuilder bar1Text,StringBuilder bar2Text)
        {
            rect.Width = m_skillBar2Bitmap.Width;
            rect.Height = m_skillBar2Bitmap.Height;
            spriteBatch.Draw(m_skillBar2Bitmap, rect, Color.White);

            int circleRadius = 28;
            Rectangle skillRect1Dims = new Rectangle(161, 32, 115, 16);
            Rectangle skillRect2Dims = new Rectangle(26, 17, 250, 16);

            int xpad = 2;
            rect.X += xpad;
            rect.Y -= 1;

            for (int i = 0; i < skills.Count(); ++i)
            {
                AttackSkill skill = skills[i];
                Point dims;
                Point uv;

                SkillIconState iconState = (i == m_actionCursor.X) ? SkillIconState.Selected : SkillIconState.Available;
                if (!skill.Available(CurrentActor))
                {
                    iconState = SkillIconState.Unavailable;
                }

                GetUVForIcon(skill.SkillIcon,iconState,out uv,out dims);
                Rectangle dest = new Rectangle(rect.X + (i * (circleRadius+xpad)), rect.Y + 2, circleRadius, circleRadius);
                Rectangle src = new Rectangle(uv.X,uv.Y,dims.X,dims.Y);

                spriteBatch.Draw(m_skillsBitmap, dest, src, Color.White);
            }
            Vector2 pos = new Vector2(rect.X,rect.Y);
            pos += new Vector2(skillRect1Dims.X,skillRect1Dims.Y);
            GraphicsHelper.DrawShadowedText(spriteBatch, m_smallFont, skills[m_actionCursor.X].Name, pos);
            //spriteBatch.DrawString(m_smallFont, skills[m_actionCursor.X].Name, pos, Color.Black);

        }

        public void GetUVForIcon(SkillIcon icon, SkillIconState state, out Point uv, out Point dims)
        {
            int width = 8;
            int height  = 3;
            dims = new Point(32,32);

            uv = new Point(((int)icon) * dims.X,0);
            if(state == SkillIconState.Selected)
            {
                uv.Y = dims.Y;
            }
            if(state == SkillIconState.Unavailable)
            {
                uv.Y = dims.Y*2;
            }

        }

        private BaseActor m_currentActor;
        public BaseActor CurrentActor
        {
            get
            {
                return m_currentActor;
            }
            set
            {
                m_currentActor = value;
                TurnManager.CurrentControlState = Gladius.control.TurnManager.ControlState.ChoosingSkill;
                BuildDataForActor();
                CurrentActor.CurrentAttackSkill = CurrentlySelectedSkill;
                InputAllowed = true;
            }
        }

        public AttackSkill CurrentlySelectedSkill
        {
            get
            {
                return m_currentAttackSkillLine[m_actionCursor.X];
            }
        }

        public void CancelAction()
        {
            if (TurnManager.CurrentControlState == Gladius.control.TurnManager.ControlState.UsingGrid)
            {
                CurrentActor.WayPointList.Clear();
                TurnManager.CurrentControlState = Gladius.control.TurnManager.ControlState.ChoosingSkill;
            }
        }

        public TurnManager TurnManager
        {
            get;
            set;
        }

        public void ConfirmAction()
        {
            if (CurrentActor.CurrentAttackSkill.Available(CurrentActor))
            {
                CurrentActor.ConfirmAttackSkill();
                InputAllowed = false;
            }
            else
            {
                // output some 'not available' text?
                ArenaScreen.CombatEngineUI.DrawFloatingText(CurrentActor.Position, Color.Red, "Skill Not Available.", 2f);
            }
        }

        public MovementGrid MovementGrid
        {
            get { return ArenaScreen.MovementGrid; }
        }

        public Arena Arena
        {
            get { return ArenaScreen.Arena; }
        }


        public Point ApplyMoveToGrid(ActionButton button)
        {
            Vector3 fwd = Globals.Camera.Forward;
            Vector3 right = Vector3.Cross(fwd, Vector3.Up);
            Vector3 v = Vector3.Zero;

            Point p = MovementGrid.CurrentPosition;
            if (button == ActionButton.ActionLeft)
            {
                v = -right;
            }
            else if (button == ActionButton.ActionRight)
            {
                v = right;
            }
            else if (button == ActionButton.ActionUp)
            {
                v = fwd;
            }
            else if (button == ActionButton.ActionDown)
            {
                v = -fwd;
            }
            if (v.LengthSquared() > 0)
            {
                v.Y = 0;
                v.Normalize();

                //v = v * vd;
                //v = result;

                if (Math.Abs(v.X) > Math.Abs(v.Z))
                {
                    if (v.X < 0)
                    {
                        p.X--;
                    }
                    if (v.X > 0)
                    {
                        p.X++;
                    }
                }
                else
                {
                    if (v.Z < 0)
                    {
                        p.Y--;
                    }
                    if (v.Z > 0)
                    {
                        p.Y++;
                    }
                }

            }
            return p;
        }




        private void HandleMovementGridAction(ActionButtonPressedArgs e)
        {
            switch (e.ActionButton)
            {
                case (ActionButton.ActionButton1):
                    int pathLength = CurrentActor.WayPointList.Count;
                    if (pathLength == 0)
                    {
                        pathLength = 1;
                    }
                    if (CurrentActor.CurrentAttackSkill.InRange(pathLength))
                    {
                        if (MovementGrid.CursorOnTarget(CurrentActor))
                        {
                            BaseActor target = Arena.GetActorAtPosition(MovementGrid.CurrentPosition);
                            if (ArenaScreen.CombatEngine.IsValidTarget(CurrentActor, target, CurrentActor.CurrentAttackSkill))
                            {
                                CurrentActor.Target = target;
                                ConfirmAction();
                                
                            }
                        }
                        else
                        {
                            ConfirmAction();
                        }
                    }
                    break;
                case (ActionButton.ActionButton2):
                    {
                        CancelAction();
                        break;
                    }
                case (ActionButton.ActionLeft):
                case (ActionButton.ActionRight):
                case (ActionButton.ActionUp):
                case (ActionButton.ActionDown):
                    {
                        Point p = ApplyMoveToGrid(e.ActionButton);
                        if (Arena.InLevel(p))
                        {
                            Point lastPoint = MovementGrid.CurrentPosition;
                            MovementGrid.CurrentPosition = p;
                            SquareType st = Arena.GetSquareTypeAtLocation(MovementGrid.CurrentPosition);
                            BaseActor target = Arena.GetActorAtPosition(MovementGrid.CurrentPosition);
                            //int pathLength = 
                            CurrentActor.WayPointList.Clear();

                            Point adjustedPoint = MovementGrid.CurrentPosition;
                            if (target != null && target != CurrentActor)
                            {
                                adjustedPoint = lastPoint;
                            }

                            Arena.FindPath(CurrentActor.CurrentPosition, adjustedPoint, CurrentActor.WayPointList);
                        }
                        break;
                    }
            }

        }

        public bool InputAllowed
        {
            get;
            set;
        }

        public enum SkillIconState
        {
            Available,
            Selected,
            Unavailable
        }

        List<List<AttackSkill>> m_attackSkills;
        List<AttackSkill> m_currentAttackSkillLine;

        Point m_actionCursor = new Point();        



        // this may need to be somewhere more common...
        Dictionary<DamageType, Texture2D> m_damageTypeTextures = new Dictionary<DamageType, Texture2D>();
        


        Texture2D m_shieldBarBitMap;
        Texture2D m_attackBarBitMap;
        Texture2D m_skillBar2Bitmap;
        Texture2D m_skillsBitmap;

        Point m_topLeft = new Point(20, 500);
        SpriteFont m_smallFont;
        SpriteFont m_largeFont;
    }
}
