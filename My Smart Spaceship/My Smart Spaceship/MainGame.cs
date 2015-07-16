using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using SbsSW.SwiPlCs;
using System.Collections.Generic;

namespace My_Smart_Spaceship
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        public static int screenHeight = 600;
        public static int screenWidth = 800;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Background background;
        Player player;
        COM com;
        List<Meteors> meteor = new List<Meteors>();
        Random random = new Random();
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;
            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        /// 

        float spawn = 0;
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = new Background(Content.Load<Texture2D>("purple.png"), new Vector2(100, 100), true);
            player = new Player(Content.Load<Texture2D>("player.png"), new Vector2(300,300));
            com = new COM(Content.Load<Texture2D>("com.png"), new Vector2(280, 280));

           
            
            // TODO: use this.Content to load your game content here
        }

         

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // TODO: Add your update logic here
            background.Update(gameTime);
            player.Update(gameTime);
            com.Update(gameTime);
            foreach (Meteors m in meteor)
            {
                m.Update(graphics.GraphicsDevice);
            }
            //LoadMeteors();

            //meteor.Update(gameTime);
            base.Update(gameTime);
        }

        /*public void LoadMeteors()
        {
            int rand = random.Next(100, 700);
            if (spawn > 1)
            {
                spawn = 0;
                if (meteor.Capacity < 10)
                    meteor.Add(new Meteors(Content.Load<Texture2D>("bigMeteor.png"), new Vector2(250, 250)));
            }

        }*/

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            background.Draw(gameTime,spriteBatch);
            player.Draw(gameTime,spriteBatch);
            com.Draw(gameTime, spriteBatch);
            foreach(Meteors iterator in meteor)
                    iterator.Draw(gameTime, spriteBatch);
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
