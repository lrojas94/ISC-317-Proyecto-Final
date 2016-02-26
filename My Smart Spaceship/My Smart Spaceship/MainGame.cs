using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
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

        private enum GameStates
        {
            Running, Over
        }


        private static MainGame instance;
        public static MainGame Instance
        {
            get{
                if (instance == null)
                    instance = new MainGame();
                return instance;
            }

        }

        Song bgm;
        List<SoundEffectInstance> activeSounds;
        Dictionary<string, SoundEffect> soundfx;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Background background;
        Player player;
        COM com;
        //Meteors List
        MeteorController meteorController;
        PowerUpGenerator powerUpGenerator;
        Random random = new Random();
        GameStates state;
        string winner;
        SpriteFont font;
        bool aiLoaded = false;

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
            meteorController = new MeteorController(100, SpriteSheetHandler, @"Meteors/",300f,
                new Vector2(300, 100),new Vector2(100,50), new Point(0, 9), new Point(10, 19));
            powerUpGenerator = new PowerUpGenerator(SpriteSheetHandler, @"PowerUps/", new Point(0, 3));
            activeSounds = new List<SoundEffectInstance>();
            soundfx = new Dictionary<string, SoundEffect>();
            // SOUND FX !!!
            bgm = Content.Load<Song>("SoundFX/Background_Music");
            font = Content.Load<SpriteFont>("PTSans");
            MediaPlayer.Play(bgm);
            soundfx.Add("Bullet", Content.Load<SoundEffect>("SoundFX/Bullet"));
            soundfx.Add("SuperBullet", Content.Load<SoundEffect>("SoundFX/SuperBullet"));
            soundfx.Add("Shield", Content.Load<SoundEffect>("SoundFX/Shield"));
            soundfx.Add("Explosion", Content.Load<SoundEffect>("SoundFX/Explosion"));

            state = GameStates.Running;

            if (!PlEngine.IsInitialized) {
                PlEngine.Initialize(new string[] { "-q", "AI.pl" });
            }
            if (!aiLoaded)
            {
                com.LoadFromFile("Content/conocimiento.txt");
                aiLoaded = true;
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            switch (state) {
                case GameStates.Running:
                    #region Collisions
                    //PlayerBullets and Asteroids:
                    List<Bullet> playerBullets = player.Bullets;
                    List<Bullet> comBullets = com.Bullets;
                    List<Meteors> meteors = meteorController.Meteors;
                    Rectangle comRange = com.ActionRange();
                    Rectangle comFireRange = com.FireRange();
                    Vector2 direction = Vector2.Zero;
                    List<Tuple<Rectangle, string>> actions = new List<Tuple<Rectangle, string>>();
                    PlQuery query;
                    bool comShouldShoot = false;
                    string objectVeredict;

                    // Dispose all of the inactive audio instances
                    for (int i = 0; i < activeSounds.Count; i++)
                    {
                        if (activeSounds[i].State == SoundState.Stopped)
                        {
                            activeSounds.RemoveAt(i);
                        }
                    }

                    foreach (Bullet b in playerBullets)
                    {
                        if (b.CanCollide)
                        {

                            if (b.Rectangle.Intersects(comFireRange))
                                comShouldShoot = PlQuery.PlCall(String.Format("disparar({0}).", b.Name)) | comShouldShoot;

                            if (comRange.Intersects(b.Rectangle))
                            {
                                query = new PlQuery(String.Format("mover({0}, Veredicto).", b.Name));
                                objectVeredict = query.SolutionVariables.First()["Veredicto"].ToString();
                                query.Dispose();
                                actions.Add(new Tuple<Rectangle, string>(b.Rectangle, objectVeredict));
                            }

                            foreach (Meteors m in meteors)
                            {
                                if (m.CanCollide)
                                {
                                    if (b.Rectangle.Intersects(m.Rectangle))
                                    {
                                        b.Explode();
                                        SoundEffectInstance soundInstance = soundfx["Explosion"].CreateInstance();

                                        if (!m.IsUndestructible)
                                        {
                                            m.Explode();
                                            soundToPlay("Explosion");
                                        }
                                        else
                                            soundToPlay("Explosion", 0.7f);
                                    }
                                }
                            }
                        }
                        if (com.CanCollide && b.Rectangle.Intersects(com.Rectangle))
                        {
                            bool kill_able = com.KillPlayer();
                            soundToPlay("Explosion", kill_able ? 1.0f : 0.4f);
                            com.AddEvent(new COM.Cause
                            {
                                PossibleCause = COM.PossibleCauses.Impacts,
                                Stimulus = b.Name,
                                TargetObject = com.Name
                            }, new COM.Consecuence
                            {
                                PossibleConsecuence = kill_able ? COM.PossibleConsecuences.Damages : COM.PossibleConsecuences.Ignores,
                                Stimulus = b.Name,
                                TargetObject = com.Name
                            });
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
                                        soundToPlay("Explosion", 0.5f, 1.0f);
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
                                        soundToPlay("Explosion");
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
                                    {
                                        soundToPlay("Explosion", 0.7f);
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
                                }

                        if (player.CanCollide && b.Rectangle.Intersects(player.Rectangle))
                        {
                            b.Explode();
                            soundToPlay("Explosion", 0.4f);
                            com.AddEvent(new COM.Cause
                            {
                                PossibleCause = COM.PossibleCauses.Impacts,
                                Stimulus = b.Name,
                                TargetObject = player.Name
                            }, new COM.Consecuence
                            {
                                PossibleConsecuence = player.KillPlayer() ? COM.PossibleConsecuences.Damages : COM.PossibleConsecuences.Ignores,
                                Stimulus = b.Name,
                                TargetObject = player.Name
                            });

                            
                        }
                    }


                    foreach (Meteors m in meteors)
                    {
                        if (!m.CanCollide)
                            continue;

                        if (m.Rectangle.Intersects(comFireRange))
                            comShouldShoot = PlQuery.PlCall(String.Format("disparar({0}).", m.Name))
                                            | comShouldShoot;
                        query = new PlQuery(String.Format("mover({0}, Veredicto).", m.Name));
                        objectVeredict = query.SolutionVariables.First()["Veredicto"].ToString();
                        query.Dispose();

                        if (m.Rectangle.Intersects(comRange))
                        {
                            actions.Add(new Tuple<Rectangle, string>(m.Rectangle, objectVeredict));
                        }

                        if (player.CanCollide && m.Rectangle.Intersects(player.Rectangle))
                        {
                            bool kill_able = player.KillPlayer();
                            if (kill_able)
                                soundToPlay("Explosion", 0.9f);
                            com.AddEvent(
                                new COM.Cause
                                {
                                    PossibleCause = COM.PossibleCauses.Impacts,
                                    Stimulus = m.Name,
                                    TargetObject = player.Name
                                },
                                new COM.Consecuence
                                {
                                    PossibleConsecuence = kill_able ? COM.PossibleConsecuences.Damages : COM.PossibleConsecuences.Ignores,
                                    Stimulus = m.Name,
                                    TargetObject = player.Name
                                }
                                );
                            m.Explode();
                        }

                        if (com.CanCollide && m.Rectangle.Intersects(com.Rectangle))
                        {
                            bool kill_able = com.KillPlayer();
                            if (kill_able)
                                soundToPlay("Explosion", 0.9f, 1.0f);

                            com.AddEvent(
                                new COM.Cause
                                {
                                    PossibleCause = COM.PossibleCauses.Impacts,
                                    Stimulus = m.Name,
                                    TargetObject = com.Name
                                },
                                new COM.Consecuence
                                {
                                    PossibleConsecuence = kill_able ? COM.PossibleConsecuences.Damages : COM.PossibleConsecuences.Ignores,
                                    Stimulus = m.Name,
                                    TargetObject = com.Name
                                }
                                );
                            m.Explode();
                        }

                    }

                    if (player.CanCollide && player.Rectangle.Intersects(comFireRange))
                        comShouldShoot = PlQuery.PlCall(String.Format("disparar({0}).", player.Name)) | comShouldShoot;

                    if (player.CanCollide && powerUpGenerator.PowerUp.IsActive && player.Rectangle.Intersects(powerUpGenerator.PowerUp.Rectangle))
                    {
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
                        player.PowerUp = powerUpGenerator.PowerUp.Power;
                        powerUpGenerator.Take();

                    }

                    if (com.CanCollide && powerUpGenerator.PowerUp.IsActive && com.Rectangle.Intersects(powerUpGenerator.PowerUp.Rectangle))
                    {
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
                        com.PowerUp = powerUpGenerator.PowerUp.Power;
                        powerUpGenerator.Take();

                    }

                    if (powerUpGenerator.PowerUp.IsActive)
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
                    com.Update(gameTime, player.Position, actions);
                    powerUpGenerator.Update(gameTime);
                    if (comShouldShoot)
                        com.Shoot();
                    if (com.GameOver)
                    {
                        winner = "HUMAN RACE";
                        state = GameStates.Over;
                    }
                    else if (player.GameOver) {
                        winner = "COMPUTER";
                        state = GameStates.Over;
                    }
                    break;
                case GameStates.Over:
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        Console.WriteLine("Dumped..");
                        MediaPlayer.Stop();
                        state = GameStates.Running;
                        LoadContent();
                    }
                    break;

            }

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

        public void soundToPlay(string soundName, float volume = 1.0f, float pitch = 0.0f){
            SoundEffectInstance sf = soundfx[soundName].CreateInstance();
            sf.Volume = volume;
            sf.Pitch = pitch;
            sf.Play();
            activeSounds.Add(sf);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            switch (state) {
                case GameStates.Running:
                    background.Draw(gameTime, spriteBatch);
                    //DrawColor(Color.Blue, com.FireRange());
                    meteorController.Draw(spriteBatch);
                    powerUpGenerator.Draw(spriteBatch);
                    player.Draw(spriteBatch);
                    com.Draw(spriteBatch);
                    break;
                case GameStates.Over:
                    GraphicsDevice.Clear(Color.FromNonPremultiplied(44, 62, 80,255));
                    string final = "THE WINNER IS " + winner.ToUpper();
                    Vector2 pos = new Vector2(ScreenWidth / 2, ScreenHeight / 2) - font.MeasureString(final)/2;
                    spriteBatch.DrawString(font, final, pos, Color.White);
                    break;

            }
            
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {

            com.DumpToFile("Content/conocimiento.txt");
            MediaPlayer.Stop();
            Content.Unload();
            Dispose();
            base.OnExiting(sender, args);

        }

    }
}
