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
        private Stack<Bullet> inactiveBullets = new Stack<Bullet>();
        private List<Bullet> activeBullets = new List<Bullet>();

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
                return MainGame.Instance.spriteSheetHandler.SpriteRectangle(spritePath,position,scale);
            }
        }
       
        public Player(string spritePath,Vector2 playerSpeed) {
            this.spritePath = spritePath;
            this.playerSpeed = playerSpeed;
            Rectangle sprite = MainGame.Instance.spriteSheetHandler.SpriteRectangle(spritePath,Vector2.Zero,scale);
            position = new Vector2(MainGame.Instance.ScreenWidth / 2, MainGame.Instance.ScreenHeight - sprite.Height/2);
        }



        public void Update(GameTime gameTime) {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Rectangle sprite = MainGame.Instance.spriteSheetHandler.SpriteRectangle(spritePath,position,scale);

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
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            MainGame.Instance.spriteSheetHandler.DrawSprite(spriteBatch, position, spritePath,scale);
        }
    }
}
