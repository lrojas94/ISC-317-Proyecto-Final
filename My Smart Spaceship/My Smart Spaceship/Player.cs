using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace My_Smart_Spaceship
{
    class Player
    {
        protected enum PlayerStates {
            Alive,Dead
        }

        protected string spritePath;
        protected Vector2 position;
        protected Vector2 playerSpeed;
        protected float scale = 0.5f;
        protected Vector2 shootingVelocity;
        protected Stack<Bullet> inactiveBullets = new Stack<Bullet>();
        protected List<Bullet> activeBullets = new List<Bullet>();
        protected SpriteSheetHandler handler;
        protected KeyboardState prevKeyboardState = Keyboard.GetState();
        protected Animator explosionAnimation;
        protected PlayerStates state = PlayerStates.Alive;
        protected float animationScale;

        public float Scale {
            get{
                return scale;
            }
            set {
                scale = value;
            }
        }

        public bool CanCollide {
            get {
                return state == PlayerStates.Alive;
            }
        }
        
        public Rectangle Rectangle {
            get {
                return handler.SpriteRectangle(spritePath,position,scale);
            }
        }

        public List<Bullet> Bullets {
            get {
                return activeBullets;
            }
        }

        public Player(SpriteSheetHandler handler, string spritePath,Vector2 playerSpeed) {
            this.handler = handler;
            this.spritePath = spritePath;
            this.playerSpeed = playerSpeed;
            shootingVelocity = -playerSpeed * 1.5f;
            shootingVelocity.X = 0;
            Rectangle sprite = handler.SpriteRectangle(spritePath,Vector2.Zero,scale);
            position = new Vector2(MainGame.Instance.ScreenWidth / 2, MainGame.Instance.ScreenHeight - sprite.Height/2);
            explosionAnimation = handler.AnimatorWithAnimation("Explosion",false);
            
            Vector2 scaleFactor = Rectangle.Size.ToVector2() / explosionAnimation.CurrentFrameRectangle(position).Size.ToVector2();
            animationScale = Math.Min(scaleFactor.X, scaleFactor.Y);
        }

        public void GenerateBullets(SpriteSheetHandler handler,int count = 100) {
            for (int i = 0; i < count; i++) {
                Bullet b = new Bullet(handler,shootingVelocity, scale);
                inactiveBullets.Push(b);
            }
        }

        public void KillPlayer() {
            state = PlayerStates.Dead;
        }


        public void Update(GameTime gameTime) {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (state) {
                case PlayerStates.Alive:
                    Rectangle spriteBounds = Rectangle;

                    if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
                        position.X += -1 * playerSpeed.X * delta;


                    if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
                        position.X += playerSpeed.X * delta;


                    if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W))
                        position.Y += -1 * playerSpeed.Y * delta;


                    if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S))
                        position.Y += playerSpeed.Y * delta;

                    position = position.KeepInGameFrame(spriteBounds);

                    // If alive can shoot
                    if (Keyboard.GetState().IsKeyDown(Keys.Space) && !prevKeyboardState.IsKeyDown(Keys.Space))
                    {
                        //Shot a bullet.
                        Bullet b = inactiveBullets.Pop();
                        b.StartBullet(position);
                        activeBullets.Add(b);
                    }

                    break;
                case PlayerStates.Dead:
                    explosionAnimation.Update(gameTime);
                    break;
            }

            //Check Active bullets:
            for (int i = 0; i < activeBullets.Count; i++) {
                activeBullets[i].Update(gameTime);
                if (!activeBullets[i].IsActive)
                {
                    Bullet b = activeBullets[i];
                    activeBullets.RemoveAt(i);
                    inactiveBullets.Push(b);
                    i--;
                }
            }

            prevKeyboardState = Keyboard.GetState();
        }

        public void Draw(SpriteBatch spriteBatch) {
            foreach (Bullet b in activeBullets)
                b.Draw(spriteBatch);

            switch (state) {
                case PlayerStates.Alive:
                    handler.DrawSprite(spriteBatch, position, spritePath, scale);
                    break;
                case PlayerStates.Dead:
                    explosionAnimation.Draw(spriteBatch, position, animationScale);
                    break;

            }
        }
    }
}
