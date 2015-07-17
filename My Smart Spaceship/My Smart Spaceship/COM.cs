using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace My_Smart_Spaceship
{
	class COM : Player
	{
        public COM(SpriteSheetHandler handler, string spritePath, Vector2 playerSpeed) :
            base(handler,spritePath, playerSpeed) {
            Rectangle sprite = Rectangle;
            position = new Vector2(MainGame.Instance.ScreenWidth / 2,sprite.Height / 2); //Set COM at top.

        }

        override public void Update(GameTime gameTime) {
            //Update Code ^^
        }

        override public void Draw(SpriteBatch spriteBatch) {
            //Drawing Code.
        }

        // Este metodo sera la abstraccion entre C# y prolog para las consultas'
        // con la base de conocimiento de la IA.
        private void IAlogic()
        {


        }
	}
}
