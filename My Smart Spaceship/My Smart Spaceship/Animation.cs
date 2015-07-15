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
        private string name;
        private Texture2D sprite;
        private Vector2 frameSize;
        private bool shouldLoop;
        private bool isDone;
        private float elapsedTime;
        private float frameTime;
        private int actualFrame;
        private int frameCount;


        public delegate void AnimationDone();

        public Animation(string name,int frameRate,Vector2 frameSize,bool shouldLoop = true) {

        }

        public void Update(GameTime gameTime) {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        }
    }
}
