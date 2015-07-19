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
            position = new Vector2(MainGame.Instance.ScreenWidth / 2,sprite.Height / 2); //Set COM at top.

        }

        public void Update(GameTime gameTime)
        {
            //Update Code ^^
        }

        public void Draw(SpriteBatch spriteBatch) {
            //Drawing Code.
            switch (state) {
                case PlayerStates.Alive:
                    handler.DrawSprite(spriteBatch, position, spritePath,0,scale,SpriteEffects.FlipVertically);
                    break;
            }
            
        }

        public void AddEvent(Cause cause, Consecuence consecuence) {
            string newCause = "";
            switch (cause.PossibleCause) {
                case PossibleCauses.Impacts:
                    newCause = String.Format("impacta({0},{1}).", cause.Stimulus, cause.TargetObject);
                    break;
            }
            string newConsecuence = "";
            switch (consecuence.PossibleConsecuence) {
                case PossibleConsecuences.Benefits:
                    newConsecuence = String.Format("beneficia({0},{1}).", consecuence.Stimulus, consecuence.TargetObject);
                    break;
                case PossibleConsecuences.Damages:
                    newConsecuence = String.Format("perjudica({0},{1}).", consecuence.Stimulus, consecuence.TargetObject);
                    break;
                case PossibleConsecuences.Ignores:
                    newConsecuence = String.Format("evento_nulo");
                    break;

            }

            string newEvent = String.Format("evento({0},{1})", newCause, newConsecuence);
            eventsOnHold.Add(newEvent);
        }

        private void addEvents() {
            string finalQuery = String.Format("percepcion([{0}])", string.Join(",", eventsOnHold));
            eventsOnHold.Clear();
            //CALL finalQuery in prolog :)          
        }

        // Este metodo sera la abstraccion entre C# y prolog para las consultas'
        // con la base de conocimiento de la IA.
        private void IAlogic()
        {


        }
	}
}
