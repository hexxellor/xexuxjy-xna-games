#region File Description
//-----------------------------------------------------------------------------
// ScreenManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Gladius.control;
using Gladius;
using Gladius.util;
#endregion

namespace Gladius.gamestatemanagement.screenmanager
{
    /// <summary>
    /// The screen manager is a component which manages one or more GameScreen
    /// instances. It maintains a stack of screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active screen.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        #region Fields

        List<GameScreen> screens = new List<GameScreen>();
        List<GameScreen> screensToAdd = new List<GameScreen>();
        List<GameScreen> screensToRemove = new List<GameScreen>();
        List<GameScreen> screensToUpdate = new List<GameScreen>();

        public InputState input = new InputState();

        SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D blankTexture;

        //ContentManager contentManager = new ThreadSafeCo
        ThreadSafeContentManager contentManager;
        DateTime lastUpdate = DateTime.Now;
        int fpsCounter = 0;
        int fps;

        bool isInitialized;

        bool traceEnabled;

        #endregion

        #region Properties


        public ThreadSafeContentManager ContentManager
        {
            get
            {
                return contentManager;
            }
        }

        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }


        /// <summary>
        /// A default font shared by all the screens. This saves
        /// each screen having to bother loading their own local copy.
        /// </summary>
        public SpriteFont Font
        {
            get { return font; }
        }


        /// <summary>
        /// If true, the manager prints out a list of all the screens
        /// each time it is updated. This can be useful for making sure
        /// everything is being added and removed at the right times.
        /// </summary>
        public bool TraceEnabled
        {
            get { return traceEnabled; }
            set { traceEnabled = value; }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public ScreenManager(Game game)
            : base(game)
        {
            // we must set EnabledGestures before we can query for them, but
            // we don't assume the game wants to read them.
            TouchPanel.EnabledGestures = GestureType.None;
            contentManager = new ThreadSafeContentManager(game, game.Services);
            contentManager.RootDirectory = "Content";
        }


        /// <summary>
        /// Initializes the screen manager component.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
            Globals.UserControl = new UserControl(Game, input);
            Game.Components.Add(Globals.UserControl);

        }


        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.

            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = ContentManager.Load<SpriteFont>("UI/fonts/BattleOverFont");
            blankTexture = ContentManager.GetColourTexture(Color.Black);

            // Tell each of the screens to load their content.
            foreach (GameScreen screen in screens)
            {
                screen.LoadContent();
            }
            foreach (GameScreen screen in screensToAdd)
            {
                screen.LoadContent();
            }
        }


        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in screens)
            {
                screen.UnloadContent();
            }
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            try
            {
                // Read the keyboard and gamepad.
                input.Update();
                Globals.CameraManager.Update(gameTime);
                Globals.CameraManager.UpdateInput(input);


                // add/remove screens
                foreach (GameScreen screen in screensToRemove)
                {
                    screens.Remove(screen);
                    screen.UnregisterListeners();
                }
                foreach (GameScreen screen in screensToAdd)
                {
                    screens.Add(screen);
                    screen.RegisterListeners();
                }
                screensToAdd.Clear();
                screensToRemove.Clear(); 

                // Make a copy of the master screen list, to avoid confusion if
                // the process of updating one screen adds or removes others.
                screensToUpdate.Clear();

                foreach (GameScreen screen in screens)
                    screensToUpdate.Add(screen);

                bool otherScreenHasFocus = !Game.IsActive;
                bool coveredByOtherScreen = false;

                if ((DateTime.Now - lastUpdate).TotalMilliseconds >= 1000)
                {
                    fps = fpsCounter;
                    fpsCounter = 0;
                    lastUpdate = DateTime.Now;
                }
                else
                {
                    fpsCounter++;
                }



                // Loop as long as there are screens waiting to be updated.
                while (screensToUpdate.Count > 0)
                {
                    // Pop the topmost screen off the waiting list.
                    GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];

                    screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                    // Update the screen.
                    screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                    if (screen.ScreenState == ScreenState.TransitionOn ||
                        screen.ScreenState == ScreenState.Active)
                    {
                        // If this is the first active screen we came across,
                        // give it a chance to handle input.
                        //if (!otherScreenHasFocus)
                        {
                            screen.HandleInput(input, gameTime);

                            otherScreenHasFocus = true;
                        }

                        // If this is an active non-popup, inform any subsequent
                        // screens that they are covered by it.
                        if (!screen.IsPopup)
                            coveredByOtherScreen = true;
                    }
                }

                // Print debug trace?
                if (traceEnabled)
                    TraceScreens();
            }
            catch (System.Exception ex)
            {
                int ibreak = 0;
            }
        }


        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary>
        void TraceScreens()
        {
            List<string> screenNames = new List<string>();

            foreach (GameScreen screen in screens)
                screenNames.Add(screen.GetType().Name);

            Debug.WriteLine(string.Join(", ", screenNames.ToArray()));
        }


        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            lock (GraphicsDevice)
            {
                foreach (GameScreen screen in screens)
                {
                    if (screen.ScreenState == ScreenState.Hidden)
                        continue;

                    screen.Draw(gameTime);
                }
            }
        }


        #endregion

        #region Public Methods


        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
        {
            screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (isInitialized)
            {
                screen.LoadContent();
            }

            screensToAdd.Add(screen);
            //screens.Add(screen);

            // update the TouchPanel to respond to gestures this screen is interested in
            TouchPanel.EnabledGestures = screen.EnabledGestures;
        }


        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if (isInitialized)
            {
                screen.UnloadContent();
            }


            //screens.Remove(screen);
            screensToRemove.Add(screen);
            //screensToUpdate.Remove(screen);

            // if there is a screen still in the manager, update TouchPanel
            // to respond to gestures that screen is interested in.
            if (screens.Count > 0)
            {
                TouchPanel.EnabledGestures = screens[screens.Count - 1].EnabledGestures;
            }
        }


        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }

        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        public void FadeBackBufferToBlack(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            spriteBatch.Begin();

            spriteBatch.Draw(blankTexture,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.Black * alpha);

            spriteBatch.End();
        }

        public int FPS
        {
            get { return fps; }
        }


        #endregion
    }
}
