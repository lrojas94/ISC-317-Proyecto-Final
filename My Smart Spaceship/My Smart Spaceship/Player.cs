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
        Texture2D sprite;
        Vector2 position;
        Vector2 playerSpeed;
       
        public Player(Texture2D sprite,Vector2 playerSpeed) {
            this.sprite = sprite;
            this.playerSpeed = playerSpeed;
            position = new Vector2(MainGame.screenWidth / 2 - sprite.Width / 2, MainGame.screenHeight - sprite.Height);
        }

        public void Update(GameTime gameTime) {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                position.X += -1 * playerSpeed.X * delta;
           

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                position.X += playerSpeed.X * delta;
            

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                position.Y += -1 * playerSpeed.Y * delta;
            

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                position.Y += playerSpeed.Y * delta;

            if (position.X < 0)
                position.X = 0;
            if (position.X + sprite.Width > MainGame.screenWidth)
                position.X = MainGame.screenWidth - sprite.Width;
            if (position.Y < 0)
                position.Y = 0;
            if (position.Y + sprite.Height > MainGame.screenHeight)
                position.Y = MainGame.screenHeight - sprite.Height;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            spriteBatch.Draw(sprite, position, Color.White);
        }
    }
}
