using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkUnderLevelEditor {

    class Utils {

        public enum Images {
            Enemy,
            Item,
            Door,
            EnemyRoot,
            ItemRoot,
            DoorRoot,
            Level
        };

        public enum MapElement{
          Floor,
          Wall,
          LockedGate = 100,
          LockedDoor,
          UnlockedDoor,
        };

        public enum EnemyType {
          Beholder,
          Skeleton,
          Displacer,
          Wraith,
          Dragon,
          Rat,
          Slime
        };

        public enum ItemType {
          None,
          Key,
          Potion,
          Scroll,
          Shield,
          Sword,
          LockedGate = 100,
          LockedDoor,
          UnlockedDoor,
        };

        public enum Direction {
          North,
          East,
          South,
          West,
          Count
        };

        public static String getDoorTypeDescription(ItemType itemType) {

            switch (itemType) {

                case (ItemType.LockedGate):
                    return "Locked Gate";

                case (ItemType.LockedDoor):
                    return "Locked Door";

                default:
                    return "Ahhhh!";

            }

        }

        public static String getItemTypeDescription(ItemType itemType) {

            switch (itemType) {

                case (ItemType.Key):
                    return "Key";

                case (ItemType.Potion):
                    return "Potion";

                case (ItemType.Scroll):
                    return "Scroll";

                default:
                    return "Ahhhh!";

            }

        }

        public static String getEnemyTypeDescription(EnemyType enemyType) {

            switch (enemyType) {

                case EnemyType.Beholder:
                    return "Beholder";

                case EnemyType.Skeleton:
                    return "Skeleton";

                case EnemyType.Displacer:
                    return "Displacer";

                case EnemyType.Wraith:
                    return "Wraith";

                case EnemyType.Dragon:
                    return "Dragon";

                case EnemyType.Rat:
                    return "Rat";

                case EnemyType.Slime:
                    return "Slime";

                default:
                    return "Ahhh!";

            }

        }


    }

}
