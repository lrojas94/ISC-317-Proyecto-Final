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

        public SpriteData(int x, int y, int width, int height) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        
        public Rectangle GetRectangle(){
            return new Rectangle(x, y, width, height);
        }

        public Rectangle GetPositionRectangle(Vector2 position,float scale = 1.0f) {
            //IMPORTANT NOTE: This gets drawn from the CENTER instead of the TopLeft corner.
            //This is because the animation software used works like that.
            return new Rectangle((int)position.X - (int)(width * scale) / 2, (int)position.Y - (int)(height * scale) / 2, 
                (int)(width * scale), (int)(height * scale));
        }

        public Rectangle GetPositionRectangle(Vector2 position,Vector2 offset,float scale = 1.0f)
        {
            //IMPORTANT NOTE: This gets drawn from the CENTER instead of the TopLeft corner.
            //This is because the animation software used works like that.
            Rectangle rectangle = GetPositionRectangle(position,scale);
            rectangle.Offset(offset);
            return rectangle;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D spriteSheet,Vector2 position,float scale = 1.0f) {
            spriteBatch.Draw(spriteSheet, GetPositionRectangle(position,scale), GetRectangle(), Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D spriteSheet, Vector2 position,Vector2 offset, float scale = 1.0f)
        {
            spriteBatch.Draw(spriteSheet, GetPositionRectangle(position,offset,scale), GetRectangle(), Color.White);
        }
        

    }
}
