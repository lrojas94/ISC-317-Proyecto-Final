using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace My_Smart_Spaceship
{
    class Meteors
    {
        Texture2D sprite;
        Vector2 position;
        Vector2 speed;
        //int speed;

        Random random = new Random();
        int randX, randY;


        public Meteors(Texture2D sprite, Vector2 position)
        {
            this.sprite = sprite;
            this.position = position;

            randX = random.Next(-5, 5);
            randY = random.Next(5, 1);
            speed = new Vector2(randX, randY);
            //position = new Vector2(MainGame.screenWidth / 4- sprite.Width/2, 0);


        }

        public void Update(GameTime gameTime)
        {
  
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, Color.White);
        }


        internal void Update(GraphicsDevice graphicsDevice)
        {
            throw new NotImplementedException();
        }
    }
}
