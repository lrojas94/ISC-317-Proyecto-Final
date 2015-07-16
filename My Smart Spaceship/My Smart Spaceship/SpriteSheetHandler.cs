using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace My_Smart_Spaceship
{
    public struct FrameData
    {
        public string SpritePath;
        public Vector2 Offset;
        public float Delay; // In miliseconds.
    }

    public class SpriteSheetHandler
    {
        public static float AssumedFPS = 60;
        private XmlDocument spriteSheetDocument = new XmlDocument();
        private XmlDocument animationDocument = new XmlDocument();
        private Texture2D spriteSheet;
        private Dictionary<string,Dictionary<string,SpriteData>> spriteSheetData= new Dictionary<string, Dictionary<string, SpriteData>>();
        private Dictionary<string, List<FrameData>> animations = new Dictionary<string, List<FrameData>>();
        public SpriteSheetHandler(string spriteSheetFilePath) {
            spriteSheetDocument.Load(spriteSheetFilePath);
            parseSpriteSheetDocument();
        }

        public SpriteSheetHandler(string spriteSheetFilePath, string animationSheetFilePath) {
            spriteSheetDocument.Load(spriteSheetFilePath);
            parseSpriteSheetDocument();
            animationDocument.Load(animationSheetFilePath);
            parseAnimationDocument();
        }

        private void parseSpriteSheetDocument() {
            // Get spritesheet image:
            XmlNode spriteSheetXMLData = spriteSheetDocument.GetElementsByTagName("img")[0];
            string fileName = spriteSheetXMLData.Attributes.GetNamedItem("name").InnerText;
            spriteSheet = MainGame.Instance.Content.Load<Texture2D>(fileName);
            XmlNodeList spriteSets = spriteSheetXMLData.SelectNodes(".//definitions//dir//dir");
            foreach (XmlNode set in spriteSets) {
                string setName = set.Attributes.GetNamedItem("name").InnerText;
                Dictionary<string, SpriteData> setData = new Dictionary<string, SpriteData>();
                XmlNodeList sprites = set.SelectNodes(".//spr");
                foreach (XmlNode spr in sprites) {
                    string sprName = spr.Attributes.GetNamedItem("name").InnerText;
                    int x = Convert.ToInt32(spr.Attributes.GetNamedItem("x").InnerText);
                    int y = Convert.ToInt32(spr.Attributes.GetNamedItem("y").InnerText);
                    int w = Convert.ToInt32(spr.Attributes.GetNamedItem("w").InnerText);
                    int h = Convert.ToInt32(spr.Attributes.GetNamedItem("h").InnerText);

                    SpriteData data = new SpriteData(x, y, w, h);
                    setData.Add(sprName, data);
                }
                spriteSheetData.Add(setName, setData);
            }
        }

        private void parseAnimationDocument() {
            XmlNode animationXMLData = animationDocument.GetElementsByTagName("animations")[0];
            XmlNodeList animationsData = animationXMLData.SelectNodes(".//anim");
            foreach (XmlNode animationData in animationsData) {
                string name = animationData.Attributes.GetNamedItem("name").InnerText;
                List<FrameData> animation = new List<FrameData>();
                XmlNodeList cellData = animationData.SelectNodes(".//cell");
                foreach (XmlNode cell in cellData) {
                    XmlNode spr = cell.FirstChild;
                    FrameData data;
                    data.Delay = 1.0f/(AssumedFPS/(float)Convert.ToDouble(cell.Attributes.GetNamedItem("delay").InnerText));
                    data.SpritePath = spr.Attributes.GetNamedItem("name").InnerText.Substring(1);
                    Vector2 offset = new Vector2(Convert.ToInt32(spr.Attributes.GetNamedItem("x").InnerText),
                        Convert.ToInt32(spr.Attributes.GetNamedItem("y").InnerText));
                    data.Offset = offset;
                    animation.Add(data);
                }
                animations.Add(name, animation);

                
            }
        }

        #region SpriteHandling
        public Rectangle SpriteRectangle(string spritePath,Vector2 position,float scale = 1.0f) {
            string[] path = spritePath.Split('/');
            SpriteData spr = spriteSheetData[path[0]][path[1]];
            return spr.GetPositionRectangle(position,scale);
        }
        public void DrawSprite(SpriteBatch spriteBatch, Vector2 position, string spritePath,float scale = 1.0f) {
            string[] path = spritePath.Split('/');
            SpriteData spr = spriteSheetData[path[0]][path[1]];
            spr.Draw(spriteBatch, spriteSheet, position,scale);
        }

        public void DrawSprite(SpriteBatch spriteBatch, Vector2 position, string spritePath,Vector2 offset,float scale = 1.0f){
            string[] path = spritePath.Split('/');
            SpriteData spr = spriteSheetData[path[0]][path[1]];
            spr.Draw(spriteBatch, spriteSheet, position,offset,scale);
        }

        #endregion
        #region AnimationHandling
        public Animator AnimatorWithAnimation(string animationName) {
            return new Animator(animations[animationName], this);
        }
        #endregion
    }
}
