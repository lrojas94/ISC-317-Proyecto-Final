using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace My_Smart_Spaceship
{
    class Background
    {
        Texture2D sprite;
        List<Vector2> positions = new List<Vector2>();
        Vector2 velocity;
        bool repeat;
        public Vector2 Velocity {
            get {
                return velocity;
            }
            set {
                velocity = value;
            }
        }

        public bool ShouldRepeat {
            get {
                return repeat;
            }
            set {
                repeat = value;
            }
        }

        public Background(Texture2D sprite, Vector2 velocity, bool shouldRepeat = true) {
            this.sprite = sprite;
            this.velocity = velocity;
            this.ShouldRepeat = shouldRepeat;
            //Create spritePositions:
            createSpriteTiles();
            
        }

        private void createSpriteTiles() {
            int posY = -sprite.Height, posX = -sprite.Width;
            int screenWidth = MainGame.Instance.ScreenWidth;
            int screenHeight = MainGame.Instance.ScreenHeight;
            while (posY < screenHeight)
            {
                while (posX < screenWidth)
                {
                    positions.Add(new Vector2(posX, posY));
                    posX += sprite.Width-1; //There's always a misplaced pixel -_- that asshole.
                }
                posY += sprite.Height-1;
                posX = -sprite.Width; //Reset X position.
            }
        }

        public void Update(GameTime gameTime){
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < positions.Count; i++) {
                //IMPORTANT:
                //  Vector2 is a Structure, which means it is passed by VALUE, Not reference.
                Vector2 pos = positions[i];
                if (ShouldRepeat) {
                    if (pos.X > MainGame.Instance.ScreenWidth)
                        pos.X = -sprite.Width;
                    if (pos.Y > MainGame.Instance.ScreenHeight)
                        pos.Y = -sprite.Height;
                }
                pos += velocity * delta;
                positions[i] = pos;

            }

        }

        public void Draw(GameTime gameTime,SpriteBatch spriteBatch) {
            foreach (Vector2 pos in positions)
                spriteBatch.Draw(sprite, pos, Color.White);
        }

    }
}
