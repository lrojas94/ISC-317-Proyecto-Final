using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace My_Smart_Spaceship
{
    public class Animator
    {
        private List<FrameData> animation;
        private float elapsedTime = 0;
        private int currentFrame = 0;
        private SpriteSheetHandler handler;
        private bool shouldLoop = false;
        private bool isDone = false;

        public bool IsDone {
            get {
                return isDone;
            }
        }

        public Animator(List<FrameData> animation, SpriteSheetHandler handler,bool shouldLoop = true) {
            this.handler = handler;
            this.animation = animation;
            this.shouldLoop = shouldLoop;
        }

        public void Reset() {
            elapsedTime = 0;
            currentFrame = 0;
            isDone = false ;
        }

        public void Update(GameTime gameTime) {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsedTime >= animation[currentFrame].Delay && !isDone){
                currentFrame++;
                elapsedTime = 0;
                if (currentFrame == animation.Count) {
                    if (shouldLoop)
                        currentFrame %= animation.Count;
                    else
                        isDone = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position) {
            handler.DrawSprite(spriteBatch, position, animation[currentFrame].SpritePath, animation[currentFrame].Offset);
        }

        public Rectangle CurrentFrameRectangle(Vector2 position,float scale = 1.0f) {
            return handler.SpriteRectangle(animation[currentFrame].SpritePath,position,scale);
        }
    }
}
