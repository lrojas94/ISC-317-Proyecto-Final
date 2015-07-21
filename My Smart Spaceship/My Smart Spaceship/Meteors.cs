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
        private enum MeteorStates {
            Moving,Exploding,Inactive
        }

        private string spritePath;
        private SpriteSheetHandler handler;
        private Animator explodeAnimation;
        private MeteorStates state = MeteorStates.Inactive;
        private Vector2 position = Vector2.Zero;
        private float rotation;
        private Vector2 velocity = Vector2.Zero;
        private bool isActive;
        private bool isUndestructible;
        private float explosionScale = 1.0f;
        public bool IsVisible {
            get {
                return isActive;
            }
        }

        public bool CanCollide {
            get {
                return state == MeteorStates.Moving;
            }
        }
        
        public Vector2 Velocity {
            get {
                return velocity;
            }
        }

        public bool IsUndestructible {
            get {
                return isUndestructible;
            }
        }        

        public Vector2 Position{
            get{
                return position;
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
            explodeAnimation = handler.AnimatorWithAnimation("Explosion", false);
        }

        public void Start(Vector2 position, Vector2 velocity,bool isUndestructible = false, string spritePath = null) {
            if (spritePath != null)
                this.spritePath = spritePath;
            this.isUndestructible = isUndestructible;
            isActive = true;
            this.position = position;
            this.velocity = velocity;
            state = MeteorStates.Moving;
            explodeAnimation.Reset();

        }

        public void Explode() {
            state = MeteorStates.Exploding;
            Vector2 scale = Rectangle.Size.ToVector2() / explodeAnimation.CurrentFrameRectangle(position).Size.ToVector2();
            explosionScale = Math.Min(scale.X, scale.Y);
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (state) {
                case MeteorStates.Moving:
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
                        state = MeteorStates.Inactive;
                    break;
                case MeteorStates.Exploding:
                    explodeAnimation.Update(gameTime);
                    if (explodeAnimation.IsDone)
                        state = MeteorStates.Inactive;
                    break;
                case MeteorStates.Inactive:
                    isActive = false;
                    break;

                }
        }

        public void Draw(SpriteBatch spriteBatch){
            switch (state) {
                case MeteorStates.Moving:
                    handler.DrawSprite(spriteBatch, position, spritePath, rotation, 1.0f, SpriteEffects.None);
                    break;
                case MeteorStates.Exploding:
                    explodeAnimation.Draw(spriteBatch, position, explosionScale);
                    break;
                default:
                    break;
            }
        }

    }
}
