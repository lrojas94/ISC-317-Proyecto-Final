﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SbsSW.SwiPlCs;


namespace My_Smart_Spaceship
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 
    public class MainGame : Game
    {
        

        public int ScreenHeight = 600;
        public int ScreenWidth = 800;
        public SpriteSheetHandler SpriteSheetHandler;
        

        private static MainGame instance;
        public static MainGame Instance
        {
            get{
                if (instance == null)
                    instance = new MainGame();
                return instance;
            }

        }


        static Dictionary<string, SoundEffectInstance> soundfx;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Background background;
        Player player;
        COM com;
        //Meteors List
        MeteorController meteorController;

        Random random = new Random();
        public MainGame()
        {
            soundfx = new Dictionary<string, SoundEffectInstance>();
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.PreferredBackBufferWidth = ScreenWidth;
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
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.

            SpriteSheetHandler = new SpriteSheetHandler(@"Content\spriteSheet.sprites",@"Content\Animations.anim");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = new Background(Content.Load<Texture2D>("purple.png"), new Vector2(100, 100), true);
            player = new Player(this.SpriteSheetHandler,@"Players/playerA_Blue", new Vector2(500,500));
            player.GenerateBullets(SpriteSheetHandler);
            com = new COM(this.SpriteSheetHandler, @"Players/playerC_Mix", new Vector2(250, 250));
            com.GenerateBullets(SpriteSheetHandler);
            meteorController = new MeteorController(100, SpriteSheetHandler, @"Meteors/",60f,
                new Vector2(300, 100),new Vector2(100,50), new Point(3, 9), new Point(13, 19));


            // SOUND FX !!!
            soundfx.Add("Bullet", Content.Load<SoundEffect>("SoundFX/Bullet").CreateInstance());
            soundfx.Add("SuperBullet", Content.Load<SoundEffect>("SoundFX/SuperBullet").CreateInstance());
            soundfx.Add("Shield", Content.Load<SoundEffect>("SoundFX/Shield").CreateInstance());
            soundfx.Add("Explosion", Content.Load<SoundEffect>("SoundFX/Explosion").CreateInstance());

            if (!PlEngine.IsInitialized) {
                PlEngine.Initialize(new string[] { "-q", "AI.pl" });
            }
            com.LoadFromFile("Content/conocimiento.txt");
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

            #region Collisions
            //PlayerBullets and Asteroids:
            List<Bullet> playerBullets = player.Bullets;
            List<Bullet> comBullets = com.Bullets;
            List<Meteors> meteors = meteorController.Meteors;
            Rectangle comRange = com.ActionRange();
            Rectangle comFireRange = com.FireRange();
            Vector2 direction = Vector2.Zero;
            List<Tuple<Rectangle, string>> actions = new List<Tuple<Rectangle,string>>();
            PlQuery query;
            bool comShouldShoot = false;
            query = new PlQuery("mover(disparo(humano), Veredicto).");
            string objectVeredict = query.SolutionVariables.First()["Veredicto"].ToString();
            query.Dispose();

            foreach (Bullet b in playerBullets){
                if (b.CanCollide){

                    if(b.Rectangle.Intersects(comFireRange))
                        comShouldShoot = PlQuery.PlCall("disparar(disparo(humano)).") | comShouldShoot;

                    if (comRange.Intersects(b.Rectangle))
                        actions.Add(new Tuple<Rectangle, string>(b.Rectangle, objectVeredict));

                    foreach (Meteors m in meteors){
                        if (m.CanCollide){
                            if (b.Rectangle.Intersects(m.Rectangle)){
                                b.Explode();
                                if (!m.IsUndestructible){
                                    m.Explode();
                                }
                                soundfx["Explosion"].Stop(true);
                                soundfx["Explosion"].Play();
                            }
                        }
                    }
                }
                if (com.CanCollide && b.Rectangle.Intersects(com.Rectangle))
                {
                    com.AddEvent(new COM.Cause {
                        PossibleCause = COM.PossibleCauses.Impacts,
                        Stimulus = "disparo(humano)",
                        TargetObject = "ia"
                    }, new COM.Consecuence {
                        PossibleConsecuence = COM.PossibleConsecuences.Damages,
                        Stimulus = "disparo(humano)",
                        TargetObject = "ia"
                    });
                    com.KillPlayer();
                }
                
            }

            foreach (Bullet b in comBullets)
            {
                if (b.CanCollide)

                    foreach (Bullet hb in playerBullets)
                    {
                        if (hb.CanCollide)
                        {
                            if (b.Rectangle.Intersects(hb.Rectangle))
                            {
                                hb.Explode();
                                b.Explode();
                                com.AddEvent(new COM.Cause
                                {
                                    PossibleCause = COM.PossibleCauses.Impacts,
                                    Stimulus = "disparo(ia)",
                                    TargetObject = "disparo(humano)"
                                }, new COM.Consecuence
                                {
                                    PossibleConsecuence = COM.PossibleConsecuences.Damages,
                                    Stimulus = "disparo(ia)",
                                    TargetObject = "disparo(humano)"
                                });
                                break;
                            }
                        }
                    }

                    foreach (Meteors m in meteors)
                        if (m.CanCollide)
                            if (b.Rectangle.Intersects(m.Rectangle))
                            {
                                b.Explode();
                                if (!m.IsUndestructible)
                                {
                                    m.Explode();
                                    com.AddEvent(
                                        new COM.Cause
                                        {
                                            PossibleCause = COM.PossibleCauses.Impacts,
                                            Stimulus = "disparo(ia)",
                                            TargetObject = "asteroide"
                                        },
                                        new COM.Consecuence
                                        {
                                            PossibleConsecuence = COM.PossibleConsecuences.Damages,
                                            Stimulus = "disparo(ia)",
                                            TargetObject = "asteroide"
                                        });
                                }
                                else
                                    com.AddEvent(
                                        new COM.Cause
                                        {
                                            PossibleCause = COM.PossibleCauses.Impacts,
                                            Stimulus = "disparo(ia)",
                                            TargetObject = "asteroide_gris"
                                        },
                                        new COM.Consecuence
                                        {
                                            PossibleConsecuence = COM.PossibleConsecuences.Ignores,
                                            Stimulus = "disparo(ia)",
                                            TargetObject = "asteroide_gris"
                                        });
                            }

                if (player.CanCollide && b.Rectangle.Intersects(player.Rectangle))
                {
                    b.Explode();
                    player.KillPlayer();
                    com.AddEvent(new COM.Cause
                    {
                        PossibleCause = COM.PossibleCauses.Impacts,
                        Stimulus = "disparo(ia)",
                        TargetObject = "humano"
                    }, new COM.Consecuence
                    {
                        PossibleConsecuence = COM.PossibleConsecuences.Damages,
                        Stimulus = "disparo(ia)",
                        TargetObject = "humano"
                    });
                }
            }

            

            foreach (Meteors m in meteors)
            {
                if (!m.CanCollide)
                    continue;

                if(m.Rectangle.Intersects(comFireRange))
                    comShouldShoot = PlQuery.PlCall(String.Format("disparar({0}).", m.IsUndestructible ? "asteroide_gris" : "asteroide"))
                                    | comShouldShoot;
                query = new PlQuery(String.Format("mover({0}, Veredicto).",m.IsUndestructible ? "asteroide_gris" : "asteroide"));
                objectVeredict = query.SolutionVariables.First()["Veredicto"].ToString();
                query.Dispose();

                if (m.Rectangle.Intersects(comRange))
                {
                    actions.Add(new Tuple<Rectangle, string>(m.Rectangle, objectVeredict));
                }

                if (player.CanCollide && m.Rectangle.Intersects(player.Rectangle)) {
                    player.KillPlayer();
                }

                if (com.CanCollide && m.Rectangle.Intersects(com.Rectangle)) {
                    com.KillPlayer();
                    com.AddEvent(
                        new COM.Cause
                        {
                            PossibleCause = COM.PossibleCauses.Impacts,
                            Stimulus = m.IsUndestructible ? "asteroide_gris" : "asteroide",
                            TargetObject = "ia"
                        },
                        new COM.Consecuence
                        {
                            PossibleConsecuence = COM.PossibleConsecuences.Damages,
                            Stimulus = m.IsUndestructible ? "asteroide_gris" : "asteroide",
                            TargetObject = "ia"
                        }
                        );
                }
                
            }

            if(player.CanCollide && player.Rectangle.Intersects(comFireRange))
                comShouldShoot = PlQuery.PlCall("disparar(humano).") | comShouldShoot;


            #endregion

            // TODO: Add your update logic here

            com.Update(gameTime, player.Position, actions);
            background.Update(gameTime);
            meteorController.Update(gameTime);
            player.Update(gameTime);
            com.Update(gameTime, player.Position,actions);
            if (comShouldShoot)
                com.Shoot();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 


        public void DrawColor(Color c, Rectangle r) {
            Texture2D t = new Texture2D(GraphicsDevice, 1, 1,false,SurfaceFormat.Color);
            t.SetData<Color>(new Color[] { c });
            spriteBatch.Draw(t, r, Color.White);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            background.Draw(gameTime,spriteBatch);

            //DrawColor(Color.Blue, com.FireRange());
            meteorController.Draw(spriteBatch);
            player.Draw(spriteBatch);
            com.Draw(spriteBatch);
          
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            com.DumpToFile("Content/conocimiento.txt");
            base.OnExiting(sender, args);
        }

    }
}
