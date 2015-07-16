using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace My_Smart_Spaceship
{
	class COM
	{
        Texture2D sprite;
        Vector2 position;
        Vector2 playerSpeed;
       
        public COM(Texture2D sprite,Vector2 playerSpeed) {
            this.sprite = sprite;
            this.playerSpeed = playerSpeed;
            position = new Vector2(MainGame.Instance.ScreenWidth / 2 - sprite.Width / 2, 0);
        }

        public void Update(GameTime gameTime) {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (position.X < 0)
                position.X = 0;
            if (position.X + sprite.Width > MainGame.Instance.ScreenWidth)
                position.X = MainGame.Instance.ScreenWidth - sprite.Width;
            if (position.Y < 0)
                position.Y = 0;
            if (position.Y + sprite.Height > MainGame.Instance.ScreenHeight)
                position.Y = MainGame.Instance.ScreenHeight - sprite.Height;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            spriteBatch.Draw(sprite, position, Color.White);
        }

        // Este metodo sera la abstraccion entre C# y prolog para las consultas'
        // con la base de conocimiento de la IA.
        private void IAlogic()
        {


        }
	}
}
