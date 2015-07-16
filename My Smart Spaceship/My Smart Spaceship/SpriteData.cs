using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace My_Smart_Spaceship
{
    class SpriteData
    {
        private int x;
        private int y;
        private int width;
        private int height;
        private Vector2 offset;
        public Vector2 Offset {
            get {
                return offset;
            }
            set {
                offset = value;
            }
        }

        public SpriteData(int x, int y, int width, int height) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        
        public Rectangle GetRectangle(){
            return new Rectangle(x, y, width, height);
        }

        public Rectangle GetPositionRectangle(Vector2 position) {
            //IMPORTANT NOTE: This gets drawn from the CENTER instead of the TopLeft corner.
            //This is because the animation software used works like that.
            return new Rectangle((int)position.X + (int)offset.X - width/2, (int)position.Y + (int)offset.Y - height/2, width, height);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D spriteSheet,Vector2 position) {
            spriteBatch.Draw(spriteSheet, GetPositionRectangle(position), GetRectangle(), Color.White);
        }

    }
}
