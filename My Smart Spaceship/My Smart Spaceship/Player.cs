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
            this.shootingVelocity = new Vector2(0, -300);
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
            Rectangle sprite = handler.SpriteRectangle(spritePath,position,scale);

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                position.X += -1 * playerSpeed.X * delta;
           

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                position.X += playerSpeed.X * delta;
            

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                position.Y += -1 * playerSpeed.Y * delta;
            

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                position.Y += playerSpeed.Y * delta;

            if (position.X - sprite.Width/2 < 0)
                position.X = sprite.Width/2;
            if (position.X + sprite.Width/2 > MainGame.Instance.ScreenWidth)
                position.X = MainGame.Instance.ScreenWidth - sprite.Width/2;
            if (position.Y - sprite.Height/2 < 0)
                position.Y = sprite.Height/2;
            if (position.Y + sprite.Height/2 > MainGame.Instance.ScreenHeight)
                position.Y = MainGame.Instance.ScreenHeight - sprite.Height/2;
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

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            foreach (Bullet b in activeBullets)
                b.Draw(gameTime, spriteBatch);
            handler.DrawSprite(spriteBatch, position, spritePath,scale);
        }
    }
}
