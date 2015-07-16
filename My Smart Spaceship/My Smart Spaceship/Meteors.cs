using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace My_Smart_Spaceship
{
    class Meteors
    {
        Texture2D sprite;
        Vector2 position;
        public Vector2 origin;
        float rotation;
        int speed;
        Rectangle bounds;
        public bool isVisible;

        Random random = new Random();
        float randX, randY;


        public Meteors(Texture2D sprite, Vector2 position)
        {
            this.sprite = sprite;
            this.position = position;
            speed = 4;
            isVisible = true;
            randX = random.Next(0, 750);
            randY = random.Next(-600, -50);

        }

        public void LoadContent(ContentManager Content)
        {
            sprite = Content.Load<Texture2D>("bigMeteor.png");
            origin.X = sprite.Width / 2;
            origin.Y = sprite.Height / 2;

        }

        public void Update(GameTime gameTime)
        {
            bounds = new Rectangle((int)position.X, (int)position.Y, 45, 45);

            //movement
            position.Y = position.Y + speed;
            if (position.Y >= 600)
                position.Y = -50;

            //rotation
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            rotation += elapsed;
            float circle = MathHelper.Pi * 2;
            rotation = rotation % circle;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(isVisible)
            spriteBatch.Draw(sprite, position, null, Color.White,rotation,origin,1.0f,SpriteEffects.None,0f);
        }

    }
}
