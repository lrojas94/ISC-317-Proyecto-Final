using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Background background;
        Player player;
        COM com;
        //Meteors List
        MeteorController meteorController;
        PowerUpGenerator powerUpGenerator;
        Random random = new Random();
        public MainGame()
        {
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
            meteorController = new MeteorController(100, SpriteSheetHandler, @"Meteors/",120f,
                new Vector2(300, 100),new Vector2(100,50), new Point(0, 9), new Point(10, 19));
            powerUpGenerator = new PowerUpGenerator(SpriteSheetHandler, @"PowerUps/", new Point(0, 3));

            if (!PlEngine.IsInitialized)
            {
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
            string objectVeredict;



            foreach (Bullet b in playerBullets){
                if (b.CanCollide){

                    if(b.Rectangle.Intersects(comFireRange))
                        comShouldShoot = PlQuery.PlCall(String.Format("disparar({0}).",b.Name)) | comShouldShoot;

                    if (comRange.Intersects(b.Rectangle))
                    {
                        query = new PlQuery(String.Format("mover({0}, Veredicto).",b.Name));
                        objectVeredict = query.SolutionVariables.First()["Veredicto"].ToString();
                        query.Dispose();
                        actions.Add(new Tuple<Rectangle, string>(b.Rectangle, objectVeredict));
                    }

                    foreach (Meteors m in meteors){
                        if (m.CanCollide){
                            if (b.Rectangle.Intersects(m.Rectangle)){
                                b.Explode();
                                if (!m.IsUndestructible)
                                    m.Explode();
                            }
                        }
                    }
                }
                if (com.CanCollide && b.Rectangle.Intersects(com.Rectangle))
                {
                    com.AddEvent(new COM.Cause {
                        PossibleCause = COM.PossibleCauses.Impacts,
                        Stimulus = b.Name,
                        TargetObject = com.Name
                    }, new COM.Consecuence {
                        PossibleConsecuence = COM.PossibleConsecuences.Damages,
                        Stimulus = b.Name,
                        TargetObject = com.Name
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
                                    Stimulus = b.Name,
                                    TargetObject = hb.Name
                                }, new COM.Consecuence
                                {
                                    PossibleConsecuence = COM.PossibleConsecuences.Damages,
                                    Stimulus = b.Name,
                                    TargetObject = hb.Name
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
                                            Stimulus = b.Name,
                                            TargetObject = m.Name
                                        },
                                        new COM.Consecuence
                                        {
                                            PossibleConsecuence = COM.PossibleConsecuences.Damages,
                                            Stimulus = b.Name,
                                            TargetObject = m.Name
                                        });
                                }
                                else
                                    com.AddEvent(
                                        new COM.Cause
                                        {
                                            PossibleCause = COM.PossibleCauses.Impacts,
                                            Stimulus = b.Name,
                                            TargetObject = m.Name
                                        },
                                        new COM.Consecuence
                                        {
                                            PossibleConsecuence = COM.PossibleConsecuences.Ignores,
                                            Stimulus = b.Name,
                                            TargetObject = m.Name
                                        });
                            }

                if (player.CanCollide && b.Rectangle.Intersects(player.Rectangle))
                {
                    b.Explode();
                    player.KillPlayer();
                    com.AddEvent(new COM.Cause
                    {
                        PossibleCause = COM.PossibleCauses.Impacts,
                        Stimulus = b.Name,
                        TargetObject = player.Name
                    }, new COM.Consecuence
                    {
                        PossibleConsecuence = COM.PossibleConsecuences.Damages,
                        Stimulus = b.Name,
                        TargetObject = player.Name
                    });
                }
            }

            
            foreach (Meteors m in meteors)
            {
                if (!m.CanCollide)
                    continue;

                if(m.Rectangle.Intersects(comFireRange))
                    comShouldShoot = PlQuery.PlCall(String.Format("disparar({0}).", m.Name))
                                    | comShouldShoot;
                query = new PlQuery(String.Format("mover({0}, Veredicto).",m.Name));
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
                            Stimulus = m.Name,
                            TargetObject = com.Name
                        },
                        new COM.Consecuence
                        {
                            PossibleConsecuence = COM.PossibleConsecuences.Damages,
                            Stimulus = m.Name,
                            TargetObject = com.Name
                        }
                        );
                }
                
            }

            if(player.CanCollide && player.Rectangle.Intersects(comFireRange))
                comShouldShoot = PlQuery.PlCall(String.Format("disparar({0}).",player.Name)) | comShouldShoot;

            if (player.CanCollide && powerUpGenerator.PowerUp.IsActive && player.Rectangle.Intersects(powerUpGenerator.PowerUp.Rectangle))
            {
                player.PowerUp = powerUpGenerator.PowerUp.Power;
                com.AddEvent(new COM.Cause
                {
                    PossibleCause = COM.PossibleCauses.Impacts,
                    Stimulus = powerUpGenerator.PowerUp.Name,
                    TargetObject = player.Name
                },
                new COM.Consecuence
                {
                    PossibleConsecuence = COM.PossibleConsecuences.Benefits,
                    Stimulus = powerUpGenerator.PowerUp.Name,
                    TargetObject = player.Name
                });
                powerUpGenerator.Take();
                
            }

            if (com.CanCollide && powerUpGenerator.PowerUp.IsActive && com.Rectangle.Intersects(powerUpGenerator.PowerUp.Rectangle))
            {
                com.PowerUp = powerUpGenerator.PowerUp.Power;
                com.AddEvent(new COM.Cause
                {
                    PossibleCause = COM.PossibleCauses.Impacts,
                    Stimulus = powerUpGenerator.PowerUp.Name,
                    TargetObject = com.Name
                },
                new COM.Consecuence
                {
                    PossibleConsecuence = COM.PossibleConsecuences.Benefits,
                    Stimulus = powerUpGenerator.PowerUp.Name,
                    TargetObject = com.Name
                });
                powerUpGenerator.Take();

            }

            if (powerUpGenerator.PowerUp.IsActive )
            { //&& powerUpGenerator.PowerUp.Rectangle.Intersects(com.ActionRange(2))
                query = new PlQuery(String.Format("mover({0}, Veredicto).", powerUpGenerator.PowerUp.Name));
                objectVeredict = query.SolutionVariables.First()["Veredicto"].ToString();
                query.Dispose();
                actions.Add(new Tuple<Rectangle, string>(powerUpGenerator.PowerUp.Rectangle, objectVeredict));
            }
            #endregion

            // TODO: Add your update logic here

            com.Update(gameTime, player.Position, actions);
            background.Update(gameTime);
            meteorController.Update(gameTime);
            player.Update(gameTime);
            com.Update(gameTime, player.Position,actions);
            powerUpGenerator.Update(gameTime);
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
            powerUpGenerator.Draw(spriteBatch);
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
