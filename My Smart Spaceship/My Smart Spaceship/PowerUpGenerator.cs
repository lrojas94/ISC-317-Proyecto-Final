using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace My_Smart_Spaceship
{
    class PowerUpGenerator
    {
        private PowerUp powerUp;
        private string basePath;
        private Point spriteRange;
        private Random random = new Random();
        private float timeSinceLast = 0;
        private float timeBetweenPowerUps;

        public PowerUp PowerUp {
            get {
                return powerUp;
            }
        }

        public PowerUpGenerator(SpriteSheetHandler handler,string basePath, Point spriteRange) {
            this.basePath = basePath;
            this.spriteRange = spriteRange;
            this.powerUp = new PowerUp(handler);
            timeBetweenPowerUps = random.Next(3, 10);
        }

        public void Update(GameTime gameTime) {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (!powerUp.IsActive)
            {
                timeSinceLast += delta;
                if (timeSinceLast > timeBetweenPowerUps)
                {
                    int screenWidth = MainGame.Instance.ScreenWidth;
                    int screenHeight = MainGame.Instance.ScreenHeight;
                    Vector2 puPosition = new Vector2(random.Next(screenWidth / 10, screenWidth - screenWidth / 10),
                        random.Next(screenHeight / 10, screenHeight - screenHeight / 10));
                    int spriteNumber = random.Next(0, 3);
                    Console.WriteLine(spriteNumber);
                    Player.PowerUps powerUp = (Player.PowerUps)spriteNumber;
                    this.powerUp.Start(puPosition, powerUp, basePath + spriteNumber);

                }
            }
        }


        public void Take() {
            powerUp.Take();
            timeSinceLast = 0;
            timeBetweenPowerUps = random.Next(3, 10);
        }

        public void Draw(SpriteBatch spriteBatch) {
            if(powerUp.IsActive)
                powerUp.Draw(spriteBatch);
        }
       
    }
}
