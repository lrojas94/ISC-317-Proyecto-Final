using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace My_Smart_Spaceship
{
    class PowerUp
    {
        private string spritePath;
        private SpriteSheetHandler handler;
        private Player.PowerUps powerUp;
        private Vector2 position;
        private float scale = 1;
        private bool taken = true;

        public string Name {
            get {
                string name = powerUp.ToString().ToLower();
                return String.Format("power_up({0})", name);
            }
        }

        public Player.PowerUps Power {
            get
            {
                return powerUp;
            }
        }

        public Rectangle Rectangle {
            get
            {
                return handler.SpriteRectangle(spritePath, position);
            }
        }

        public bool IsActive {
            get {
                return !taken;
            }
        }

        public PowerUp(SpriteSheetHandler handler) {
            this.handler = handler;
        }

        public void Start(Vector2 position,Player.PowerUps powerUp,string spritePath, float scale = 1)
        {
            this.scale = scale;
            taken = false;
            this.powerUp = powerUp;
            this.position = position;
            this.spritePath = spritePath;
            //Set sprites for PowerUps
        }

        public void Take() {
            taken = true;
        }

        public void Draw(SpriteBatch spriteBatch) {
            handler.DrawSprite(spriteBatch, position, spritePath, scale);
        }
    }
}
