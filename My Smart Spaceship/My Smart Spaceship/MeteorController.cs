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
        private List<Meteors> activeMeteors = new List<Meteors>();
        private List<Meteors> inactiveMeteors = new List<Meteors>();
        private Random random = new Random();


        public MeteorController(int maxCount,string basePath){
            this.maxCount = maxCount;
            generate();
        }

        private void generate() {

        }

    }
}
