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
    public class SpriteSheetHandler
    {
        private XmlDocument document = new XmlDocument();
        private Texture2D spriteSheet;
        private Dictionary<string,Dictionary<string,SpriteData>> spriteSheetData= new Dictionary<string, Dictionary<string, SpriteData>>();

        public SpriteSheetHandler(string spriteSheetFilePath) {
            document.Load(spriteSheetFilePath);
            parseDocument();
        }

        private void parseDocument() {
            // Get spritesheet image:
            XmlNode spriteSheetXMLData = document.GetElementsByTagName("img")[0];
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
        
        public Rectangle SpriteRectangle(string spritePath) {
            string[] path = spritePath.Split('\\');
            SpriteData spr = spriteSheetData[path[0]][path[1]];
            return spr.GetRectangle();
        }
        public void DrawSprite(SpriteBatch spriteBatch, Vector2 position, string spritePath) {
            string[] path = spritePath.Split('\\');
            SpriteData spr = spriteSheetData[path[0]][path[1]];
            spr.Draw(spriteBatch, spriteSheet, position);
        }
    }
}
