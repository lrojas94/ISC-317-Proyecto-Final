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
        private string spritePath;
        private SpriteSheetHandler handler;
        private Vector2 position = Vector2.Zero;
        private float rotation;
        private Vector2 velocity = Vector2.Zero;
        private Rectangle bounds;
        private bool isActive;

        public bool IsVisible {
            get {
                return isActive;
            }
        }
        

        public Meteors(SpriteSheetHandler handler,string spritePath)
        {
            this.handler = handler;
            this.spritePath = spritePath;
            isActive = true;

        }

        public void Start(Vector2 position, Vector2 velocity) {
            isActive = true;
            this.position = position;
            this.velocity = velocity;
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //movement
            position += velocity * delta;
            //rotation
            rotation += delta;
            rotation = rotation % (MathHelper.Pi * 2);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch){
            handler.DrawSprite(spriteBatch, position, spritePath, rotation,1.0f,SpriteEffects.None);
        }

    }
}
