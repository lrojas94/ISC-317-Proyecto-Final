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
        private bool isActive;
        private bool isUndestructible;

        public bool IsVisible {
            get {
                return isActive;
            }
        }
        
        public bool IsUndestructible {
            get {
                return isUndestructible;
            }
        }        

        public Rectangle Rectangle{
            get{
                return handler.SpriteRectangle(spritePath, position);
            }
        }

        public Meteors(SpriteSheetHandler handler,string spritePath = null)
        {
            this.handler = handler;
            this.spritePath = spritePath;
            isActive = true;

        }

        public void Start(Vector2 position, Vector2 velocity,bool isUndestructible = false, string spritePath = null) {
            if (spritePath != null)
                this.spritePath = spritePath;
            this.isUndestructible = isUndestructible;
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

            //TO BE CHANGED IF TO WORK WITH DIFFERENT START/END POSITIONS.
            Rectangle rectangle = this.Rectangle;
            if (rectangle.Left - rectangle.Width > MainGame.Instance.ScreenWidth || 
                rectangle.Bottom + rectangle.Height < 0 || 
                rectangle.Top - rectangle.Height > MainGame.Instance.ScreenHeight)
                isActive = false;

        }

        public void Draw(SpriteBatch spriteBatch){
            handler.DrawSprite(spriteBatch, position, spritePath, rotation,1.0f,SpriteEffects.None);
        }

    }
}
