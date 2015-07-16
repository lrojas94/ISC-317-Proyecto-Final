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
        string spritePath;
        Vector2 position;
        Vector2 playerSpeed;
        
       
        public Player(string spritePath,Vector2 playerSpeed) {
            this.spritePath = spritePath;
            this.playerSpeed = playerSpeed;
            Rectangle sprite = MainGame.Instance.spriteSheetHandler.SpriteRectangle(spritePath);
            position = new Vector2(MainGame.Instance.ScreenWidth / 2, MainGame.Instance.ScreenHeight - sprite.Height/2);
        }

        public void Update(GameTime gameTime) {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Rectangle sprite = MainGame.Instance.spriteSheetHandler.SpriteRectangle(spritePath);

            if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
                position.X += -1 * playerSpeed.X * delta;


            if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
                position.X += playerSpeed.X * delta;


            if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W))
                position.Y += -1 * playerSpeed.Y * delta;


            if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S))
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
            MainGame.Instance.spriteSheetHandler.DrawSprite(spriteBatch, position, spritePath);
        }
    }
}
