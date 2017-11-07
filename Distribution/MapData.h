#pragma once

#define MAX_LEVEL_COUNT 1

#define ENEMY_BEHOLDER_HP 20
#define ENEMY_BEHOLDER_AP 12
#define ENEMY_BEHOLDER_XP 4
#define ENEMY_BEHOLDER_MV true

#define ENEMY_SKELETON_HP 10
#define ENEMY_SKELETON_AP 8
#define ENEMY_SKELETON_XP 3
#define ENEMY_SKELETON_MV true

#define ENEMY_DISPLACER_HP 10
#define ENEMY_DISPLACER_AP 5
#define ENEMY_DISPLACER_XP 2
#define ENEMY_DISPLACER_MV true

#define ENEMY_WRAITH_HP 12
#define ENEMY_WRAITH_AP 10
#define ENEMY_WRAITH_XP 4
#define ENEMY_WRAITH_MV true

#define ENEMY_DRAGON_HP 30
#define ENEMY_DRAGON_AP 20
#define ENEMY_DRAGON_XP 6
#define ENEMY_DRAGON_MV true

#define ENEMY_RAT_HP 5
#define ENEMY_RAT_AP 2
#define ENEMY_RAT_XP 1
#define ENEMY_RAT_MV false

#define ENEMY_SLIME_HP 8
#define ENEMY_SLIME_AP 4
#define ENEMY_SLIME_XP 6
#define ENEMY_SLIME_MV false

const uint8_t PROGMEM tile_00[] = {
0xff, 0x81, 0xbd, 0xe5, 0xd, 0xe9, 0xab, 0xa0, 0xbf, 0x81, 0xed, 0x5, 0xfd, 0x1, 0xff, 0x7f, 0x40, 0x5a, 0x52, 0x5a, 0x52, 0x7e, 0x2, 0x76, 0x52, 0x5a, 0x42, 0x5b, 0x48, 0x7f, 
};
const uint8_t PROGMEM tile_01[] = {
0xff, 0x1, 0xdd, 0x51, 0x55, 0x15, 0x57, 0x54, 0xd5, 0x15, 0xf5, 0x85, 0xed, 0x21, 0xff, 0x7f, 0x40, 0x56, 0x54, 0x54, 0x50, 0x54, 0x14, 0x5e, 0x54, 0x56, 0x52, 0x56, 0x40, 0x7f, 
};
const uint8_t PROGMEM tile_02[] = {
0xff, 0x9, 0xed, 0x21, 0xad, 0xa5, 0xb7, 0xa0, 0xbf, 0xa5, 0x2d, 0xa5, 0xad, 0x81, 0xff, 0x7f, 0x40, 0x5f, 0x50, 0x5b, 0x40, 0x7e, 0x2, 0x6a, 0x4b, 0x58, 0x53, 0x5e, 0x40, 0x7f, 
};
const uint8_t PROGMEM tile_03[] = {
0x7f, 0x1, 0xdd, 0x5, 0x75, 0x11, 0x55, 0x4, 0x55, 0x11, 0x75, 0x5, 0xdd, 0x1, 0x7f, 0x7f, 0x40, 0x5d, 0x50, 0x57, 0x44, 0x55, 0x10, 0x55, 0x44, 0x57, 0x50, 0x5d, 0x40, 0x7f, 
};
const uint8_t PROGMEM tile_04[] = {};
const uint8_t PROGMEM tile_05[] = {};
const uint8_t PROGMEM tile_06[] = {};
const uint8_t PROGMEM tile_07[] = {};
const uint8_t PROGMEM tile_08[] = {};
const uint8_t PROGMEM tile_09[] = {};
const uint8_t PROGMEM tile_10[] = {};
const uint8_t PROGMEM tile_11[] = {};
const uint8_t PROGMEM tile_12[] = {};
const uint8_t PROGMEM tile_13[] = {};
const uint8_t PROGMEM tile_14[] = {};
const uint8_t PROGMEM tile_15[] = {};
const uint8_t PROGMEM tile_16[] = {};
const uint8_t PROGMEM tile_17[] = {};
const uint8_t PROGMEM tile_18[] = {};
const uint8_t PROGMEM tile_19[] = {};

const uint8_t PROGMEM level_00[] = {
72, 65, 76, 76, 87, 65, 89, 32, 79, 70, 32, 
84, 72, 69, 32, 68, 69, 65, 68, 32, 32, 32, 
7, 58,
0,
1, 4,
14,
1, 7, 3,
5, 9, 49,
5, 4, 49,
2, 4, 5,
6, 7, 8,
2, 1, 9,
6, 7, 12,
6, 13, 17,
5, 5, 18,
1, 5, 22,
6, 13, 25,
6, 1, 36,
5, 1, 40,
6, 7, 41,
13,
2, 2, 6,
3, 6, 6,
2, 5, 11,
3, 13, 12,
2, 9, 13,
3, 5, 16,
3, 13, 21,
2, 5, 25,
2, 1, 28,
2, 11, 41,
2, 2, 51,
3, 6, 51,
1, 6, 14,
1,
101, 7, 0,
0, 1, 2, 0, 
};
const uint8_t PROGMEM level_01[] = {};
const uint8_t PROGMEM level_02[] = {};
const uint8_t PROGMEM level_03[] = {};
const uint8_t PROGMEM level_04[] = {};
const uint8_t PROGMEM level_05[] = {};
const uint8_t PROGMEM level_06[] = {};
const uint8_t PROGMEM level_07[] = {};
const uint8_t PROGMEM level_08[] = {};
const uint8_t PROGMEM level_09[] = {};

