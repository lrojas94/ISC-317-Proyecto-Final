using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace My_Smart_Spaceship
{
    class Animation
    {
        private Texture2D sprite;
        private Vector2 frameSize;
        private bool shouldLoop;
        private bool isDone;
        private float elapsedTime;
        private float frameTime;
        private int actualFrame;
        private int frameCount;
        private int horizontalFrameCount;
        private int verticalFrameCount;

        public bool IsDone {
            get {
                return isDone;
            }
        }
        
        public Rectangle CurrentFrameRectangle {
            get {
                int yPos = actualFrame / horizontalFrameCount;
                int xPos = actualFrame % horizontalFrameCount;
                return new Rectangle((int)(xPos * frameSize.X), (int)(yPos * frameSize.Y), (int)frameSize.X, (int)frameSize.Y);
            }
        }

        public Animation(Texture2D sprite,int frameRate,Vector2 frameSize,bool shouldLoop = true) {
            this.sprite = sprite;
            frameTime = (float)1 / (float)frameRate;
            this.frameSize = frameSize;
            verticalFrameCount = (int)(sprite.Height / frameSize.Y);
            horizontalFrameCount = (int)(sprite.Width / frameSize.X);
            frameCount = verticalFrameCount * horizontalFrameCount;
            this.shouldLoop = shouldLoop;
            
        }

        public Animation(Texture2D sprite,int frameRate,int horizontalFrameCount, int verticalFrameCount, bool shouldLoop = true) {
            this.sprite = sprite;
            frameTime = (float)1 / (float)frameRate;
            frameSize = new Vector2(sprite.Width / horizontalFrameCount, sprite.Height / verticalFrameCount);
            this.horizontalFrameCount = horizontalFrameCount;
            this.verticalFrameCount = verticalFrameCount;
            frameCount = horizontalFrameCount * verticalFrameCount;
            this.shouldLoop = shouldLoop;

        }

        private void reset() {
            actualFrame = 0;
            elapsedTime = 0;
            isDone = false;
        }

        public void Update(GameTime gameTime) {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsedTime >= frameTime)
            {
                if (shouldLoop)
                {
                    actualFrame++;
                    actualFrame %= frameCount;
                    elapsedTime = 0;
                }
                else if (actualFrame == frameCount - 1)
                    isDone = true;
            }
        }
        private Rectangle positionRectangle(Vector2 position) {
            return new Rectangle((int)position.X, (int)position.Y, (int)(position.X + frameSize.X), (int)(position.Y + frameSize.Y));
        }

        public void Draw(Vector2 position,GameTime gameTime, SpriteBatch spriteBatch) {
            spriteBatch.Draw(sprite, positionRectangle(position), CurrentFrameRectangle, Color.White);
        }
    }
}
