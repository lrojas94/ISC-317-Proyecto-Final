using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Input;
using SbsSW.SwiPlCs;

namespace My_Smart_Spaceship
{
	class COM : Player
	{
        
        public enum PossibleCauses {
            Impacts
        }
        public enum PossibleConsecuences {
            Ignores, Benefits, Damages
        }
        public struct Cause {
            public PossibleCauses PossibleCause;
            public string Stimulus;
            public string TargetObject;
        }

        public struct Consecuence {
            public PossibleConsecuences PossibleConsecuence;
            public string Stimulus;
            public string TargetObject;
        }

        private List<string> eventsOnHold = new List<string>();
        private int counter = 0;
        private Vector2 targetPosition;
        private bool goingToPoint = false;
        private bool goingToStart = true;
        private Vector2 originalPos;
        private float shotsPerSec = 2;
        private float elapsedSinceShot = 0;
        private float timeToShoot;
        private float timeSinceLastShot = 0;
        private float timeToGoToCenter = 0;


        //NOTE: Difference betwee timeSinceLastShot and elapsedSinceShot is that
        //time is supposed to shot EVEN if it cannot shot. The reason why is so that the AI can hit something and learn.

        private bool canShoot = false;
        Random random;

        public COM(SpriteSheetHandler handler, string spritePath, Vector2 playerSpeed) :
            base(handler,spritePath, playerSpeed) {
            name = "ia";
            Rectangle sprite = Rectangle;
            random = new Random();
            //position = new Vector2(MainGame.Instance.ScreenWidth / 2,sprite.Height / 2); //Set COM at top.
            position = new Vector2(0+Rectangle.Width/2,0+Rectangle.Height/2);
            originalPos = new Vector2(MainGame.Instance.ScreenWidth/2, MainGame.Instance.ScreenHeight / 2);
            shootingVelocity.Y = Math.Abs(shootingVelocity.Y); //So that bullets go down.
            timeToShoot = 1/shotsPerSec;
        }

        public new void GenerateBullets(SpriteSheetHandler handler, int count = 100)
        {
            base.GenerateBullets(handler, count);
            foreach (Bullet b in inactiveBullets)
                b.ChangeAnimations(handler, "Bullet_Red_Move", "Bullet_Red_Explode");
        }

        public void Update(GameTime gameTime, Vector2 playerPos, List<Tuple<Rectangle, string>> actions = null)
        {
            if(counter > 30){
                addEvents();
                counter = 0;
            }
            else
                counter++;
            //Update Code ^^
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsedSinceShot += delta;
            timeSinceLastShot += delta;

            switch (state) {
                case PlayerStates.Alive:
                    move(gameTime, actions);
                    if (goingToPoint)
                        goTo(targetPosition, delta);

                    else
                    {
                        timeToGoToCenter -= delta;
                        if(timeToGoToCenter <= 0)
                            goTo(originalPos, delta);
                    }
                    if (timeSinceLastShot >= random.Next(3,7))
                    {
                        shoot();
                        canShoot = false;
                        timeSinceLastShot = 0;
                        elapsedSinceShot = 0;
                    }
                    position = position.KeepInGameFrame(Rectangle);
                    if(elapsedSinceShot > timeToShoot)
                    {
                        elapsedSinceShot = 0;
                        canShoot = true;
                    }
                    break;
                case PlayerStates.Dead:
                    explosionAnimation.Update(gameTime);
                    break;
            }

            for (int i = 0; i < activeBullets.Count; i++)
            {
                activeBullets[i].Update(gameTime);
                if (!activeBullets[i].IsActive)
                {
                    Bullet b = activeBullets[i];
                    activeBullets.RemoveAt(i);
                    inactiveBullets.Push(b);
                    i--;
                }
            }
        }

        public void Shoot() {
            if (canShoot && state != PlayerStates.Dead)
            {
                canShoot = false;
                elapsedSinceShot = 0;
                timeSinceLastShot = 0;
                shoot();
            }

        }

        public new void Draw(SpriteBatch spriteBatch) {
            //Drawing Code.
            foreach (Bullet b in activeBullets)
                b.Draw(spriteBatch);

            switch (state) {
                case PlayerStates.Alive:
                    handler.DrawSprite(spriteBatch, position, spritePath,0,scale,SpriteEffects.FlipVertically);
                    break;
                case PlayerStates.Dead:
                    explosionAnimation.Draw(spriteBatch, position, scale);
                    break;

            }
        }

        public void AddEvent(Cause cause, Consecuence consecuence) {
            string newCause = "";
            switch (cause.PossibleCause) {
                case PossibleCauses.Impacts:
                    newCause = String.Format("impacta({0},{1})", cause.Stimulus, cause.TargetObject);
                    break;
            }
            string newConsecuence = "";
            switch (consecuence.PossibleConsecuence) {
                case PossibleConsecuences.Benefits:
                    newConsecuence = String.Format("beneficia({0},{1})", consecuence.Stimulus, consecuence.TargetObject);
                    break;
                case PossibleConsecuences.Damages:
                    newConsecuence = String.Format("perjudica({0},{1})", consecuence.Stimulus, consecuence.TargetObject);
                    break;
                case PossibleConsecuences.Ignores:
                    newConsecuence = String.Format("evento_nulo");
                    break;

            }
            string newEvent = String.Format("evento({0},{1})", newCause, newConsecuence);
            eventsOnHold.Add(newEvent);
        }

        private void addEvents() {
            string finalQuery = String.Format("percepcion([{0}]).", string.Join(",", eventsOnHold));
            PlQuery.PlCall(finalQuery);
            eventsOnHold.Clear();
        }

        
        public void LoadFromFile(string filePath){

            System.IO.StreamReader sr;

            try{
                sr = new System.IO.StreamReader(filePath);
            }
            catch (System.IO.FileNotFoundException){
                return;
            }
            string text = sr.ReadToEnd();
            text.Replace("\r", "");
            string[] fullKnowledge = text.Split(new char[]{'\n'});

            foreach(string fact in fullKnowledge) {
                if (fact.Length == 0)   continue;

                PlQuery.PlCall("assert(" + fact.Replace(".", "") + ").");
            }

            sr.Close();
            sr.Dispose();
        }

        public void DumpToFile(string filePath)
        {
            
            // Todas las reglas dinamicamente asertadas seran escritas en un archivo

            string query = "clause(conocimiento(), Hecho), Hecho.";
            PlQuery q = new PlQuery(query);

            string buffer = "";
            foreach (PlQueryVariables qv in q.SolutionVariables)
            {
                buffer += (qv["Hecho"].ToString() + ".\n");
            }

            System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, false);
            sw.Write(buffer);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }


        public Rectangle ActionRange(float factor = 1 ){
            Vector2 finalDirection = Vector2.Zero;
            int maxSize = Math.Max(Rectangle.Width, Rectangle.Height);
            Point fieldSize = new Point((int)(maxSize*4*factor), (int)(maxSize*4*factor));
            Point fieldOrigin = new Point((int)position.X - fieldSize.X/2, (int)position.Y - fieldSize.Y/2);
            return new Rectangle(fieldOrigin, fieldSize);
        }


        private void move(GameTime gameTime, List<Tuple<Rectangle, string>> actions) {
            if (actions == null) return;

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 moveTo = position;
            Vector2 difference = Vector2.Zero;
            Random random = new Random();
            int screenHeight = MainGame.Instance.ScreenHeight;
            int screenWidth = MainGame.Instance.ScreenWidth;
            
            foreach (Tuple<Rectangle, string> action in actions) {
                if (action.Item2 == "quedarse_quieto")
                    continue;

                // Console.Out.WriteLine(action.Item2);
                Vector2 pointFromRectBorder = this.pointFromRectBorder(moveTo, action.Item1);
                pointFromRectBorder.Y = action.Item1.Y;
                float distance = Vector2.DistanceSquared(moveTo, pointFromRectBorder);
                difference = (moveTo - pointFromRectBorder) * 4 * screenWidth/distance; //Dividing by distance will farther from the closest one.
                //Console.WriteLine("Actual -> " + moveTo + " Item -> " + action.Item1.Center.ToVector2());
                if (Math.Abs(difference.Y) < 2.0f)
                    difference.Y += (random.Next(0, 1) * 2 - 1) * screenHeight/distance;
                if (Math.Abs(difference.X) < 2.0f)
                    difference.X += (random.Next(0, 1)* 2 - 1) * screenWidth/distance;
              //  Console.WriteLine("Diferencia -> " + difference);
              //  Console.WriteLine("Actual a -> " + moveTo);
                if (action.Item2 == "alejar")
                    moveTo += difference;
                else if(action.Item2 == "acercar")
                    moveTo -= difference;
            }

            //moveTo.Normalize();
            Console.WriteLine();
            if (Vector2.Distance(moveTo, position) < 10f)
                return;
            
            //moveTo *= playerSpeed * delta;
            //position += moveTo;

            //Alejar al jugador de las esquinas:
            if (moveTo.X <= Rectangle.Width)
                moveTo.X = Rectangle.Width;
            if (moveTo.X >= screenWidth - Rectangle.Width)
                moveTo.X = screenWidth - Rectangle.Width;
            if (moveTo.Y <= Rectangle.Height)
                moveTo.Y = Rectangle.Height/2;
            if (moveTo.Y >= screenHeight - Rectangle.Height)
                moveTo.Y = screenHeight - Rectangle.Height/2;

           // Console.WriteLine(moveTo);

            targetPosition = moveTo;
            goingToPoint = true;
            goingToStart = false;
            
        }

        public Rectangle FireRange(){
            Point end = new Point(Rectangle.Width, MainGame.Instance.ScreenHeight);
            Point origin = position.ToPoint();
            origin.X -= Rectangle.Width / 2;
            origin.Y -= Rectangle.Height / 2;
            return new Rectangle(origin, end);
        }

        private bool canGoTo(Vector2 target) {
            if (Math.Abs(Vector2.Distance(position, target)) > 10)
                return true;
            return false;
        }

        private Vector2 pointFromRectBorder(Vector2 moveTo, Rectangle toAvoid) {

            int X;
            if (Math.Abs(moveTo.X - toAvoid.Left) < Math.Abs(moveTo.X - toAvoid.Right))
                X = toAvoid.Left;
            else
                X = toAvoid.Right;
            int Y;
            if (Math.Abs(moveTo.Y - toAvoid.Top) < Math.Abs(moveTo.Y - toAvoid.Bottom))
                Y = toAvoid.Top;
            else
                Y = toAvoid.Bottom;
            return new Vector2(X, Y);


        }

        private void goTo(Vector2 target,float delta) {
            float distance = Math.Abs(Vector2.Distance(position, target));
            if (distance <= 10f)
            {
                goingToPoint = false;
                goingToStart = true;
                timeToGoToCenter = 1f / 3f;
            }
            else if (distance > 10f)
            {
                Vector2 direction = target - position;
                direction.Normalize();
                position += this.playerSpeed * direction * delta ;
            }
        }
       
	}
}
