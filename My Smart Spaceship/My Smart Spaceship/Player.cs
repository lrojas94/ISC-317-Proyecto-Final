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
        private string spritePath;
        private Vector2 position;
        private Vector2 playerSpeed;
        private float scale = 0.5f;
        private Vector2 shootingVelocity;
        private Stack<Bullet> inactiveBullets = new Stack<Bullet>();
        private List<Bullet> activeBullets = new List<Bullet>();
        private SpriteSheetHandler handler;
        private KeyboardState prevKeyboardState = Keyboard.GetState();
        
        public float Scale {
            get{
                return scale;
            }
            set {
                scale = value;
            }
        }
        
        public Rectangle Rectangle {
            get {
                return handler.SpriteRectangle(spritePath,position,scale);
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
        }

        public void GenerateBullets(SpriteSheetHandler handler,int count = 100) {
            for (int i = 0; i < count; i++) {
                Bullet b = new Bullet(handler,shootingVelocity, scale);
                inactiveBullets.Push(b);
            }
        }


        public void Update(GameTime gameTime) {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Rectangle spriteBounds = handler.SpriteRectangle(spritePath,position,scale);

            if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
                position.X += -1 * playerSpeed.X * delta;


            if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
                position.X += playerSpeed.X * delta;


            if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W))
                position.Y += -1 * playerSpeed.Y * delta;


            if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S))
                position.Y += playerSpeed.Y * delta;

            position = position.KeepInGameFrame(spriteBounds);

            //Check for shots:
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !prevKeyboardState.IsKeyDown(Keys.Space)) {
                //Shot a bullet.
                Bullet b = inactiveBullets.Pop();
                b.StartBullet(position);
                activeBullets.Add(b);
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
            handler.DrawSprite(spriteBatch, position, spritePath,scale);
        }
    }
}
