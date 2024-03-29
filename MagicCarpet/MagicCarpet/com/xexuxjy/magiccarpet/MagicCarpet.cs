using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using com.xexuxjy.magiccarpet.terrain;
using com.xexuxjy.magiccarpet.util;
using com.xexuxjy.magiccarpet.collision;
using BulletXNADemos.Demos;
using Dhpoware;
using BulletXNA.LinearMath;
using com.xexuxjy.magiccarpet.gameobjects;
using com.xexuxjy.magiccarpet.manager;
using GameStateManagement;
using com.xexuxjy.magiccarpet.util.debug;


namespace com.xexuxjy.magiccarpet
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MagicCarpet : Microsoft.Xna.Framework.Game
    {

        public MagicCarpet()
        {
            Globals.GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Globals.EventLogger = new EventLogger();
            Globals.GraphicsDeviceManager.PreferredBackBufferWidth = 1024;
            Globals.GraphicsDeviceManager.PreferredBackBufferHeight = 768;
            Globals.GraphicsDeviceManager.PreparingDeviceSettings += graphics_PreparingDeviceSettings;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            AppDomain.CurrentDomain.FirstChanceException += new EventHandler<System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs>(CurrentDomain_FirstChanceException);
        
            
            Globals.ScreenManager = new ScreenManager(this);
            Globals.ScreenManager.AddScreen(new BackgroundScreen(), null);
            Globals.ScreenManager.AddScreen(new MainMenuScreen(), null);
            Globals.ScreenManager.AddScreen(new GameplayScreen(), null);
            Components.Add(Globals.ScreenManager);


            base.Initialize();
        }

        void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            System.Console.Error.WriteLine(ex.StackTrace);
            System.Console.Error.Flush();
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            System.Console.Error.WriteLine(ex.StackTrace);
            System.Console.Error.Flush();
        }
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }
            // TODO: Add your update logic here

            base.Update(gameTime);


        }

        protected override void EndRun()
        {
            Globals.EventLogger.Dispose();
            base.EndRun();
        }

   }
}
