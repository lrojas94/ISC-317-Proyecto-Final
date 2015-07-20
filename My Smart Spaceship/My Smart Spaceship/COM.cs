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
        private int counter = 0;

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

        public COM(SpriteSheetHandler handler, string spritePath, Vector2 playerSpeed) :
            base(handler,spritePath, playerSpeed) {
            Rectangle sprite = Rectangle;
            //position = new Vector2(MainGame.Instance.ScreenWidth / 2,sprite.Height / 2); //Set COM at top.
            position = new Vector2(MainGame.Instance.ScreenWidth / 2,MainGame.Instance.ScreenHeight / 2);
            shootingVelocity.Y = Math.Abs(shootingVelocity.Y); //So that bullets go down.
        }

        public new void GenerateBullets(SpriteSheetHandler handler, int count = 100)
        {
            base.GenerateBullets(handler, count);
            foreach (Bullet b in inactiveBullets)
                b.ChangeAnimations(handler, "Bullet_Red_Move", "Bullet_Red_Explode");
        }

        public void Update(GameTime gameTime, List<Tuple<Vector2, string>> actions = null)
        {
            if(counter > 30){
                addEvents();
                counter = 0;
            }
            else
                counter++;

            //Update Code ^^
            switch (state) {
                case PlayerStates.Alive:
                    if(counter > 29)
                        shoot();
                    mover(gameTime, actions);
                    position = position.KeepInGameFrame(Rectangle);
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

            Console.Out.WriteLine(finalQuery);

            PlQuery.PlCall(finalQuery);
            eventsOnHold.Clear();
        }

        
        public void loadFromFile(string filePath){

            System.IO.StreamReader sr;

            try{
                sr = new System.IO.StreamReader(filePath);
            }
            catch (System.IO.FileNotFoundException){
                return;
            }

            string[] fullKnowledge = sr.ReadToEnd().Split(new char[]{'\n'});

            foreach(string fact in fullKnowledge) {
                if (fact.Length == 0)   continue;

                PlQuery.PlCall("assert(" + fact.Replace(".", "") + ").");
            }

            sr.Close();
            sr.Dispose();
        }

        public void dumpToFile(string filePath)
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


        public Rectangle actionRange(){
            Vector2 finalDirection = Vector2.Zero;

            Point fieldSize = new Point(this.Rectangle.Size.X*8, this.Rectangle.Size.Y*8);
            Point fieldOrigin = new Point(this.Rectangle.Center.X - fieldSize.X/2, this.Rectangle.Center.Y - fieldSize.Y/2);
            
            return new Rectangle(fieldOrigin, fieldSize);
        }

        private void mover(GameTime gameTime, List<Tuple<Vector2,string>> actions){
            if (actions == null) return;

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 newDirection = Vector2.Zero;
            Vector2 tmp = Vector2.Zero;

            foreach(Tuple<Vector2,string> o in actions){
                if (o.Item2 == "quedarse_quieto")
                    continue;

                Console.Out.WriteLine(o.Item2);
                tmp = (position - o.Item1);
                tmp.Normalize();

                if(o.Item2 == "alejar"){
                    newDirection += tmp;
                }
                else if(o.Item2 == "acercar"){
                    newDirection -= tmp;
                }
            }

            if (newDirection == Vector2.Zero)
                return;

            newDirection.Normalize();
            newDirection *= playerSpeed.Length() * delta;
            position += newDirection;
        }

        public Rectangle fireRange(){
            Point end = new Point(Rectangle.Width, MainGame.Instance.ScreenHeight);
            Point origin = position.ToPoint();
            origin.X -= Rectangle.Width / 2;
            origin.Y -= Rectangle.Height / 2;

            return new Rectangle(origin, end);
        }
	}
}
