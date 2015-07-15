using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace My_Smart_Spaceship
{


    delegate void BulletDelegate(Bullet bullet);

    class Bullet
    {
        private Texture2D sprite;
        private Vector2 position;
        private Vector2 velocity;
        private bool shouldMove;
        private bool isActive;
        public BulletDelegate Delegate;  

        public bool IsActive {
            get{
                return isActive;
            }
        }

        public Bullet(Texture2D sprite)
        {
            this.sprite = sprite;
        }

        public void StartBullet(Vector2 position) {
            this.position = position;
        }

        public void Update(GameTime gameTime) {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += velocity * delta;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {

        }
    }
}
