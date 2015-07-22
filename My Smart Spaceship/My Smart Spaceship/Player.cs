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

        public enum PowerUps {
            AugmentedBullet,Shield,DoubleShot,None
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
        protected PowerUps powerUp = PowerUps.None;
        protected float animationScale;
        protected float powerUpTime = 0;
        protected Random random = new Random();
        protected Animator shieldAnimation;
        protected string name = "humano";

        public string Name {
            get {
                return name + (powerUp != PowerUps.None ? String.Format("({0})", powerUp.ToString().ToLower()) : "") ;
            }
            set {
                name = value;
            }
        }

        public Player.PowerUps PowerUp {
            set {
                powerUp = value;
                powerUpTime  = random.Next(5, 10);
            }
        }

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

        public Vector2 Position{
            get{
                return position;
            }
        }

        public Player(SpriteSheetHandler handler, string spritePath,Vector2 playerSpeed) {
            this.handler = handler;
            this.spritePath = spritePath;
            this.playerSpeed = playerSpeed;
            shootingVelocity = -playerSpeed;
            shootingVelocity.X = 0;
            Rectangle sprite = handler.SpriteRectangle(spritePath,Vector2.Zero,scale);
            position = new Vector2(MainGame.Instance.ScreenWidth / 2, MainGame.Instance.ScreenHeight - sprite.Height/2);
            explosionAnimation = handler.AnimatorWithAnimation("Explosion",false);
            shieldAnimation = handler.AnimatorWithAnimation("Effect_Shield", true);
            Vector2 scaleFactor = Rectangle.Size.ToVector2() / explosionAnimation.CurrentFrameRectangle(position).Size.ToVector2();
            animationScale = Math.Min(scaleFactor.X, scaleFactor.Y);
        }

        public void GenerateBullets(SpriteSheetHandler handler,int count = 100) {
            for (int i = 0; i < count; i++) {
                Bullet b = new Bullet(handler,shootingVelocity, scale);
                b.Name = String.Format("disparo({0})", name);
                inactiveBullets.Push(b);
            }
        }

        public bool KillPlayer() {
            if (powerUp == PowerUps.Shield)
                return false;
            state = PlayerStates.Dead;
            return true;
        }
        

        protected void shoot() {
            Bullet b, b1;
            switch (powerUp) {
                case PowerUps.AugmentedBullet:
                    b = inactiveBullets.Pop();
                    b.StartBullet(position, true);
                    activeBullets.Add(b);
                    break;
                case PowerUps.DoubleShot:
                    b = inactiveBullets.Pop();
                    b1 = inactiveBullets.Pop();
                    b.StartBullet(position + new Vector2(Rectangle.Width/2, 0));
                    b1.StartBullet(position - new Vector2(Rectangle.Width/2, 0));
                    activeBullets.Add(b);
                    activeBullets.Add(b1);
                    break;
                default:
                    b = inactiveBullets.Pop();
                    b.StartBullet(position);
                    activeBullets.Add(b);
                    break;

            }
            
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
                        shoot();
                    if (Keyboard.GetState().IsKeyDown(Keys.X) && !prevKeyboardState.IsKeyDown(Keys.X))
                        shoot();

                    //PowerUps:
                    if (powerUp != PowerUps.None) 
                        powerUpTime -= delta;
                    if (powerUpTime <= 0)
                        powerUp = PowerUps.None;
                    if (powerUp == PowerUps.Shield)
                        shieldAnimation.Update(gameTime);
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
                    if (powerUp == PowerUps.Shield)
                        shieldAnimation.Draw(spriteBatch, position);
                    break;
                case PlayerStates.Dead:
                    explosionAnimation.Draw(spriteBatch, position, animationScale);
                    break;

            }
        }
    }
}
