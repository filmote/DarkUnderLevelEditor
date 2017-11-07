using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkUnderLevelEditor {

    class Level {

        public String line1;
        public String line2;

        public int startPosX;
        public int startPosY;
        public int direction;

        public int levelDimensionX;
        public int levelDimensionY;

        public Byte[,] tileData;

        public List<LevelEnemy> enemies = new List<LevelEnemy>();
        public List<LevelItem> items = new List<LevelItem>();
        public List<LevelDoor> doors = new List<LevelDoor>();

    }

}
