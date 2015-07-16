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
        private enum BulletStates {
            Moving,Exploding,Inactive
        }
        private BulletStates state = BulletStates.Inactive;
        private Vector2 position;
        private Vector2 velocity;
        private bool shouldMove = true;
        private bool isActive = false;
        private Animator movingAnimation;
        private Animator explodeAnimation;
        private float scale;

        public bool IsActive {
            get{
                return isActive;
            }
        }

        public Bullet(SpriteSheetHandler handler,Vector2 velocity,float scale = 1.0f)
        {
            this.velocity = velocity;
            this.scale = scale;
            movingAnimation = handler.AnimatorWithAnimation("BlueBullet_Move");
            explodeAnimation = handler.AnimatorWithAnimation("BlueBullet_Explode");
        }

        public void StartBullet(Vector2 position) {
            this.position = position;
            isActive = true;
            state = BulletStates.Moving;
            movingAnimation.Reset();
            explodeAnimation.Reset();
        }

        public void Update(GameTime gameTime) {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (isActive) {
                switch (state)
                {
                    case BulletStates.Moving:
                        position += velocity * delta;
                        movingAnimation.Update(gameTime);
                        break;
                    case BulletStates.Exploding:
                        explodeAnimation.Update(gameTime);
                        if (explodeAnimation.IsDone)
                            state = BulletStates.Inactive;
                        break;
                    case BulletStates.Inactive:
                        isActive = false;
                        break;
                }
            } 
           
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            if (isActive) {
                switch (state)
                {
                    case BulletStates.Moving:
                        movingAnimation.Draw(spriteBatch, position,scale);
                        break;
                    case BulletStates.Exploding:
                        explodeAnimation.Draw(spriteBatch, position,scale);
                        break;
                    case BulletStates.Inactive:
                        break;
                }
            }
        }
    }
}
