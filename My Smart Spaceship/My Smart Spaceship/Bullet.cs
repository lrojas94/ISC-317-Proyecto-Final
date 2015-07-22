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
            Moving,Super,Exploding,Inactive
        }
        private BulletStates state = BulletStates.Inactive;
        private Vector2 position;
        private Vector2 velocity;
        private bool isActive = false;
        private Animator movingAnimation;
        private Animator explodeAnimation;
        private Animator augmentedBullet;
        private float scale;

        public bool IsActive {
            get{
                return isActive;
            }
        }
        
        public Vector2 Position{
            get{
                return position;
            }
        }

        public Rectangle Rectangle {
            get {
                switch (state)
                {
                    case BulletStates.Moving:
                        return movingAnimation.CurrentFrameRectangle(position, scale);
                    case BulletStates.Super:
                        return augmentedBullet.CurrentFrameRectangle(position, scale);
                    case BulletStates.Exploding:
                        return explodeAnimation.CurrentFrameRectangle(position, scale);
                    default:
                        return new Rectangle(0,0,0,0);
                }
            }
        }
        
        public bool CanCollide {
            get {
                return (state == BulletStates.Moving || state == BulletStates.Super);
            }
        }

        public Bullet(SpriteSheetHandler handler,Vector2 velocity,float scale = 1.0f)
        {
            this.velocity = velocity;
            this.scale = scale;
            movingAnimation = handler.AnimatorWithAnimation("BlueBullet_Move");
            explodeAnimation = handler.AnimatorWithAnimation("BlueBullet_Explode",false);
            augmentedBullet = handler.AnimatorWithAnimation("Effect_Shield");
        }

        public void ChangeAnimations(SpriteSheetHandler handler,string movingAnimationName = null, string explodeAnimationName = null) {
            if (movingAnimationName != null)
                movingAnimation = handler.AnimatorWithAnimation(movingAnimationName);
            if (explodeAnimationName != null)
                explodeAnimation = handler.AnimatorWithAnimation(explodeAnimationName,false);
        }

        public void StartBullet(Vector2 position,bool superBullet = false) {
            this.position = position;
            isActive = true;
            if (superBullet)
                state = BulletStates.Super;
            else
                state = BulletStates.Moving;
            augmentedBullet.Reset();
            movingAnimation.Reset();
            explodeAnimation.Reset();
        }

        public void Explode() {
            state = BulletStates.Exploding;
        }

        public void Update(GameTime gameTime) {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (isActive) {
                switch (state)
                {
                    case BulletStates.Super:
                    case BulletStates.Moving:
                        position += velocity * delta;

                        Rectangle positionRectangle;
                        if (state == BulletStates.Moving)
                        {
                            movingAnimation.Update(gameTime);
                            positionRectangle = movingAnimation.CurrentFrameRectangle(position, scale);
                        }
                        else
                        {
                            augmentedBullet.Update(gameTime);
                            positionRectangle = movingAnimation.CurrentFrameRectangle(position, scale);
                        }

                        if (positionRectangle.OutOfGameBounds())
                            state = BulletStates.Inactive;
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

        public void Draw(SpriteBatch spriteBatch) {
            if (isActive) {
                switch (state)
                {
                    case BulletStates.Moving:
                        movingAnimation.Draw(spriteBatch, position,scale);
                        break;
                    case BulletStates.Super:
                        augmentedBullet.Draw(spriteBatch, position, scale);
                        break;
                    case BulletStates.Exploding:
                        explodeAnimation.Draw(spriteBatch, position,scale);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
