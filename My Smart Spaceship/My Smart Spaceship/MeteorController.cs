using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace My_Smart_Spaceship
{
    class MeteorController
    {
        private int maxCount;
        private string basePath;
        private List<Meteors> activeMeteors = new List<Meteors>();
        private Stack<Meteors> inactiveMeteors = new Stack<Meteors>();
        private Random random = new Random();
        private SpriteSheetHandler handler;
        private Vector2 maxVelocity;
        private Vector2 minVelocity;
        private Point normalMeteorFrameRange;
        private Point undestructibleMeteorFrameRange;
        private float averageMeteorsPerMinute;
        private float timeBetweenMeteor; //Based seconds.
        private float timeSinceLastMeteor = 0;

    
        public Vector2 MaxVelocity{
            get{
                return maxVelocity;
            }
            set {
                maxVelocity = value;
            }
        }

        public Vector2 MinVelocity
        {
            get{
                return minVelocity;
            }
            set{
                minVelocity = value;
            }
        }

        public List<Meteors> Meteors{
            get{
                return activeMeteors;
            }
        }

        public MeteorController(int maxCount, SpriteSheetHandler handler,string basePath,float averageMeteorsPerMinute,
            Vector2 maxVelocity,Vector2 minVelocity, Point normalMeteorFrameRange, Point undestructibleMeteorFrameRange){
            this.maxCount = maxCount;
            this.handler = handler;
            this.basePath = basePath;
            this.normalMeteorFrameRange = normalMeteorFrameRange;
            this.undestructibleMeteorFrameRange = undestructibleMeteorFrameRange;
            this.maxVelocity = maxVelocity;
            this.minVelocity = minVelocity;
            this.averageMeteorsPerMinute = averageMeteorsPerMinute;
            timeBetweenMeteor = 60 / averageMeteorsPerMinute;
            instantiateMeteors();
        }

        private void instantiateMeteors() {
            for (int i = 0; i < maxCount; i++) {
                //Sprites will be decided when generating, so no basePath is needed at creation time.
                inactiveMeteors.Push(new Meteors(handler));
            }
        }

        private void createMeteorInGame() {
            Meteors meteor = inactiveMeteors.Pop();
            /*
              IMPORTANT NOTE: Asteroids will be flying from the LEFT to the RIGHT. 
              This will be done so that the AI has an easier time dealing with them. It would be too
              easy for the player if all asteroids came from above, and WAY too hard if they came from below xD!

              If anyone plans to implement this otherwise, change initial position ranges in X, and add code to the meteors
              so that they can control when they are "Out of bounds". Note that since they appear out of bounds and then come in,
              some function must be made in order to notice when it goes out based on the initial position.
            */

            Vector2 initialPosition = new Vector2(-200,random.Next(0,MainGame.Instance.ScreenHeight));
            int direction = random.Next(0, 2);
            direction = direction == 0 ? 1 : -1;
            Vector2 initialVelocity = new Vector2(random.Next((int)Math.Max(0.0f,minVelocity.X), (int)maxVelocity.X), direction * random.Next((int)minVelocity.Y, (int)maxVelocity.Y));
            int shouldBeIndestructible = random.Next(0, 2);

            if (shouldBeIndestructible == 0 || undestructibleMeteorFrameRange == null){
                int frame = random.Next(normalMeteorFrameRange.X, normalMeteorFrameRange.Y + 1);
                meteor.Start(initialPosition, initialVelocity, false, basePath + frame);
                meteor.Name = "asteroide_gris";
            }
            else {
                int frame = random.Next(undestructibleMeteorFrameRange.X, undestructibleMeteorFrameRange.Y + 1);
                meteor.Start(initialPosition, initialVelocity, true, basePath + frame);
                meteor.Name = "asteroide";
            }

            activeMeteors.Add(meteor);
        }

        public void Update(GameTime gameTime) {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeSinceLastMeteor += delta;
            if (timeSinceLastMeteor >= timeBetweenMeteor) {
                timeSinceLastMeteor = 0;
                createMeteorInGame();
                double factor = 0.5 + random.NextDouble();
                timeBetweenMeteor *= (float)factor;
                if (timeBetweenMeteor < (60 / averageMeteorsPerMinute) / 2)
                    timeBetweenMeteor = 60 / averageMeteorsPerMinute;
            }
            //Meteors base update.
            for (int i = 0; i < activeMeteors.Count; i++) {
                Meteors m = activeMeteors[i];
                m.Update(gameTime);
                if (!m.IsVisible) {
                    activeMeteors.Remove(m);
                    inactiveMeteors.Push(m);    
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            foreach (Meteors m in activeMeteors)
                m.Draw(spriteBatch);
        }

    }
}
