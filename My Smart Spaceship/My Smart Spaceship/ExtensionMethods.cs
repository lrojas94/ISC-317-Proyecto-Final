using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace My_Smart_Spaceship
{
    public static class ExtensionMethods
    {
        //NOTE -> Vector2 is a Struct, hence it's passed by value and CAN'T be altered as is.
        public static Vector2 KeepInGameFrame(this Vector2 position, Rectangle spriteRectangle) {
            if (position.X - spriteRectangle.Width / 2 < 0)
                position.X = spriteRectangle.Width / 2;
            else if (position.X + spriteRectangle.Width / 2 > MainGame.Instance.ScreenWidth)
                position.X = MainGame.Instance.ScreenWidth - spriteRectangle.Width / 2;
            if (position.Y - spriteRectangle.Height / 2 < 0)
                position.Y = spriteRectangle.Height / 2;
            else if (position.Y + spriteRectangle.Height / 2 > MainGame.Instance.ScreenHeight)
                position.Y = MainGame.Instance.ScreenHeight - spriteRectangle.Height / 2;

            return position;
        }

        public static bool OutOfGameBounds(this Rectangle spriteRectangle) {
            Rectangle mainFrame = new Rectangle(0, 0, MainGame.Instance.ScreenWidth, MainGame.Instance.ScreenHeight);
            return !mainFrame.Contains(spriteRectangle);
        }
    }
}
