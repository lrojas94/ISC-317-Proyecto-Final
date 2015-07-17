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
        private string spritePath;
        private float scale = 0.5f;
        private Vector2 position;
        private Vector2 playerSpeed;
        private Vector2 shootingVelocity;
        private Stack<Bullet> inactiveBullets = new Stack<Bullet>();
        private List<Bullet> activeBullets = new List<Bullet>();
        private SpriteSheetHandler handler;
       
        public COM(SpriteSheetHandler handler, string spritePath, Vector2 playerSpeed) {
            this.handler = handler;
            this.spritePath = spritePath;
            this.playerSpeed = playerSpeed;
            shootingVelocity = new Vector2(0, -1.5f * playerSpeed.Y);

            position = new Vector2(MainGame.Instance.ScreenWidth / 2, 1.5f * handler.SpriteRectangle(spritePath, Vector2.Zero, scale).Height);
        }

        public float Scale{
            get{
                return scale;
            }
            set{
                scale = value; 
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                return handler.SpriteRectangle(spritePath, position, scale);
            }
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            bool shoot = false;

            /* Movement handling
             * Algoritmo para moverse basado en los objetos contenidos en un rectangulo alrededor de la IA y las 
             * consideraciones de cuales de ellos son peligrosos o son beneficiosos.
             * */
  
            position.KeepInGameFrame(handler.SpriteRectangle(spritePath, position, scale));

            /* Bullet handling
             * Si hay objetos a los que se les puede disparar en el rango de tiro, se les disparara ...
             * */

            //Check for shots:
            if (shoot == true)
            {
                //Shot a bullet.
                Bullet b = inactiveBullets.Pop();
                b.StartBullet(position);
                activeBullets.Add(b);
            }

            //Check Active bullets:
            for (int i = 0; i < activeBullets.Count; i++)
            {
                activeBullets[i].Update(gameTime);
                if (!activeBullets[i].IsActive){
                    Bullet b = activeBullets[i];
                    activeBullets.RemoveAt(i);
                    inactiveBullets.Push(b);
                    i--;
                }
            }
        }

        public void GenerateBullets(SpriteSheetHandler handler, int count = 100){
            for (int i = 0; i < count; i++){
                Bullet b = new Bullet(handler, shootingVelocity, scale);
                inactiveBullets.Push(b);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            foreach (Bullet b in activeBullets)
                b.Draw(gameTime, spriteBatch);
            handler.DrawSprite(spriteBatch, position, spritePath, scale);
        }

        /* Devuleve una lista de objetos que estan en el rango de disparo de la IA */
        private List<Object> shootRange()
        {

            return null;
        }
	}
}
