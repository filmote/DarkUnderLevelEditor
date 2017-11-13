﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DarkUnderLevelEditor {

    public partial class frmDarkUnder : Form
    {

        private const int NUMBER_OF_ENEMIES = 30;
        private const int NUMBER_OF_ITEMS = 30;
        private const int NUMBER_OF_DOORS = 30;
        private const int NUMBER_OF_LEVELS = 10;
        private const int NUMBER_OF_TILES = 20;

        private Byte[] tileData = new Byte[30];
        private Level selectedLevel;
        private TreeNode selectedLevelNode;
        private Tile selectedTile;
        private int selectedCol;
        private int selectedRow;
        private int tileCount = 0;
        private List<Tile> tiles = new List<Tile>();
        private List<Level> levels = new List<Level>();
        private Boolean mouseDown = false;
        private Color cellDragColour; 

        public frmDarkUnder() {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e) {

            for (int i = 0; i < 15; i++) {
                tileEditor.Rows.Add("");
            }


            for (int i = 0; i < 255; i++) {
                levelEditor.Columns[i].Width = 12;
                levelEditor.Rows.Add("");
            }

            dgOpenMapData.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dgOpenMapData.FileName = "";
            dgOpenMapData.Filter = "MapData (*.h)|*.h";
            dgOpenMapData.FilterIndex = 0;
            dgOpenMapData.RestoreDirectory = false;

            dgSaveMapData.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dgSaveMapData.FileName = "";
            dgSaveMapData.Filter = "MapData (*.h)|*.h";
            dgSaveMapData.FilterIndex = 0;
            dgSaveMapData.RestoreDirectory = false;

        }

        private void clearLevels() {

            tiles = new List<Tile>();
            levels = new List<Level>();
            selectedLevel = null;
            selectedLevelNode = null;
            selectedTile = null;
            selectedCol = 0;
            selectedRow = 0;
            tvwLevels.Nodes.Clear();
            clearLevel();
            clearTile();
            tileCount = 0;

            for (int i = tabPageTileEditor.Controls.Count - 1; i >= 0; --i) {

                if (tabPageTileEditor.Controls[i].GetType() == typeof(Tile)) {
                    tabPageTileEditor.Controls.RemoveAt(i);
                }

            }

            mnuAddTiles.DropDownItems.Clear();

        }

        private void cmdSave_Click(object sender, EventArgs e) {

            selectedTile.Data = (Byte[])tileData.Clone();

            if (selectedLevel != null) {

                for (int x = 0; x < selectedLevel.levelDimensionX; x++) {

                    for (int y = 0; y < selectedLevel.levelDimensionY; y++) {

                        if (selectedLevel.tileData[y, x] == selectedTile.Index) {

                            populateLevelEditor(selectedLevelNode);

                        }

                    }

                }

            }

            validateDungeons();

        }

        void populateTileEditor(Byte[] data) {

            tileData = data;

            for (Byte y = 0; y < 15; y++) {

                for (Byte x = 0; x < 15; x++) {

                    int index = x + ((Byte)(y / 8) * 15);

                    if ((data[((y / 8) * 15) + x] & (1 << (y % 8))) > 0) {

                        tileEditor.Rows[y].Cells[x].Style.BackColor = Color.Gray;
                        tileData[index] = (Byte)(tileData[index] | (Byte)(1 << (y % 8)));
                    }
                    else {

                        tileEditor.Rows[y].Cells[x].Style.BackColor = Color.White;
                        tileData[index] = (Byte)(tileData[index] & ~(Byte)(1 << (y % 8)));

                    }

                }

            }

        }

        void mapTileToLevel(Byte[] data, int col, int row) {

            for (Byte y = 0; y < 15; y++) {

                for (Byte x = 0; x < 15; x++) {

                    int index = x + ((Byte)(y / 8) * 15);

                    if ((data[((y / 8) * 15) + x] & (1 << (y % 8))) > 0) {

                        levelEditor.Rows[row + y].Cells[col + x].Style.BackColor = Color.Gray;
                    }
                    else {

                        levelEditor.Rows[row + y].Cells[col + x].Style.BackColor = Color.White;

                    }

                }

            }

        }

        private void tile_Click(object sender, EventArgs e) {

            selectTile((Tile)sender);

        }

        private void selectTile(Tile tile) {

            if (selectedTile != null) { selectedTile.BackColor = SystemColors.Window; }
            selectedTile = tile;
            selectedTile.BackColor = SystemColors.ControlLight;
            populateTileEditor((Byte[])selectedTile.Data.Clone());
            tileEditor.Enabled = true;

            cmdTileDelete.Enabled = true;
            foreach (Level level in levels) {

                foreach (Byte tileIndex in level.tileData) {

                    if (tileIndex == selectedTile.Index) {
                        cmdTileDelete.Enabled = false;
                        break;
                    }

                }

            }

        }

        private void cmdTileAdd_Click(object sender, EventArgs e) {

            Tile newTile = new Tile();
            newTile.Title = "Tile " + (tileCount < 10 ? "0" : "") + tileCount;
            newTile.Click += new EventHandler(tile_Click);
            newTile.Parent = tabPageTileEditor;
            newTile.Location = new Point(410 + ((tileCount % 5) * 91), 6 + (((Byte)tileCount / 5) * 110));
            newTile.Index = tileCount;
            tiles.Add(newTile);

            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Text = newTile.Title;
            menuItem.Click += new EventHandler(mnuAddTile_Click);
            menuItem.Tag = newTile;
            mnuAddTiles.DropDownItems.Add(menuItem);

            tileCount++;

            if (tileCount == NUMBER_OF_TILES) {
                cmdTileAdd.Enabled = false;
            }

            if (selectedTile == null) {
                selectTile(newTile);
            }

        }

        private void cmdReset_Click(object sender, EventArgs e) {

            populateTileEditor(selectedTile.Data);

        }

        private void cmdClear_Click(object sender, EventArgs e) {

            for (Byte y = 0; y < 15; y++) {

                for (Byte x = 0; x < 15; x++) {

                    int index = x + ((Byte)(y / 8) * 15);
                    tileEditor.Rows[y].Cells[x].Style.BackColor = Color.White;
                    tileData[index] = 0;

                }

            }

        }

        private void levelEditor_MouseClick(object sender, MouseEventArgs e) {

            selectedCol = levelEditor.HitTest(e.X, e.Y).ColumnIndex;
            selectedRow = levelEditor.HitTest(e.X, e.Y).RowIndex;


            // Highlight the enemy in the treeview if one is selected ..

            if (selectedCol >= 0 && selectedRow >= 0) {

                if (levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor == Color.Red) {

                    foreach (TreeNode node in selectedLevelNode.Nodes[0].Nodes) {

                        LevelEnemy enemy = (LevelEnemy)node.Tag;

                        if ((enemy.startPosX == selectedCol) && (enemy.startPosY == selectedRow)) {

                            node.EnsureVisible();
                            tvwLevels.SelectedNode = node;
                            break;
                        }

                    }

                }


                // Highlight the item in the treeview if one is selected ..

                if (levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor == Color.Green) {

                    foreach (TreeNode node in selectedLevelNode.Nodes[1].Nodes) {

                        LevelItem item = (LevelItem)node.Tag;

                        if ((item.startPosX == selectedCol) && (item.startPosY == selectedRow)) {

                            node.EnsureVisible();
                            tvwLevels.SelectedNode = node;
                            break;
                        }

                    }

                }


                // Highlight the door in the treeview if one is selected ..

                if (levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor == Color.Yellow) {

                    foreach (TreeNode node in selectedLevelNode.Nodes[2].Nodes) {

                        LevelDoor door = (LevelDoor)node.Tag;

                        if ((door.startPosX == selectedCol) && (door.startPosY == selectedRow)) {

                            node.EnsureVisible();
                            tvwLevels.SelectedNode = node;
                            break;
                        }

                    }

                }


                if (e.Button == MouseButtons.Right) {

                    mnuAddTiles.Enabled = (selectedCol % 15 == 0 && selectedRow % 15 == 0);

                    if (selectedLevel != null) {

                        mnuEnemyAddBase.Enabled = (selectedLevel.enemies.Count < NUMBER_OF_ENEMIES && levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor == Color.White);
                        mnuEnemyDelete.Enabled = (levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor == Color.Red);
                        mnuItemAddBase.Enabled = (selectedLevel.items.Count < NUMBER_OF_ITEMS && levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor == Color.White);
                        mnuItemDelete.Enabled = (levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor == Color.Green);
                        mnuDoorAddBase.Enabled = (selectedLevel.doors.Count < NUMBER_OF_DOORS && levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor == Color.White);
                        mnuDoorDelete.Enabled = (levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor == Color.Yellow);
                        mnuPlacePlayer.Enabled = (levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor == Color.White);

                    }
                    else {

                        mnuEnemyAddBase.Enabled = false;
                        mnuEnemyDelete.Enabled = false;
                        mnuItemAddBase.Enabled = false;
                        mnuItemDelete.Enabled = false;
                        mnuDoorAddBase.Enabled = false;
                        mnuDoorDelete.Enabled = false;
                        mnuPlacePlayer.Enabled = false;

                    }

                    mnuContext.Show(Cursor.Position);

                }

            }

        }

        private void mnuAddTile_Click(object sender, EventArgs e) {

            Tile tile = (Tile)((ToolStripMenuItem)sender).Tag;
            mapTileToLevel(tile.Data, selectedCol, selectedRow);

            selectedLevel.tileData[selectedRow / 15, selectedCol / 15] = (byte)tile.Index;

            if ((selectedCol / 15) + 1 > selectedLevel.levelDimensionX) { selectedLevel.levelDimensionX = selectedLevel.levelDimensionX + 1; }
            if ((selectedRow / 15) + 1 > selectedLevel.levelDimensionY) { selectedLevel.levelDimensionY = selectedLevel.levelDimensionY + 1; }

            if (tile == selectedTile) cmdTileDelete.Enabled = false;
            validateDungeons();
            populateLevelEditor(Utils.getRootNode(selectedLevelNode));

        }

        private void levelEditor_CellPainting(object sender, DataGridViewCellPaintingEventArgs e) {

            if (e.ColumnIndex % 15 == 0 || e.RowIndex % 15 == 0) {
                Brush brush = new SolidBrush(Color.Red);
                e.Graphics.FillRectangle(brush, e.CellBounds);
                brush.Dispose();

                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                ControlPaint.DrawBorder(e.Graphics, e.CellBounds,
                    (e.ColumnIndex % 15 == 0 ? Color.Green : levelEditor.GridColor), 1, (e.ColumnIndex % 15 == 0 ? ButtonBorderStyle.Dashed : ButtonBorderStyle.None),
                    (e.RowIndex % 15 == 0 ? Color.Green : levelEditor.GridColor), 1, (e.RowIndex % 15 == 0 ? ButtonBorderStyle.Dashed : ButtonBorderStyle.None),
                    levelEditor.GridColor, 1, ButtonBorderStyle.None,
                    levelEditor.GridColor, 1, ButtonBorderStyle.None
                    );
                e.Handled = true;
            }

        }

        private void mnuOpenMapData_Click(object sender, EventArgs e) {

            Tile newTile = null;
            Level newLevel = null;

            int counter = 0;
            int numberOfEnemies = 0;
            int numberOfItems = 0;
            int numberOfDoors = 0;
            Byte[] tileData = new Byte[30];

            TreeNode treeNode = null;
            TreeNode treeNodeEnemies = null;
            TreeNode treeNodeItems = null;
            TreeNode treeNodeDoors = null;

            if (dgOpenMapData.ShowDialog() == DialogResult.OK) {

                clearLevels();
                dgSaveMapData.FileName = dgOpenMapData.FileName;
                lblFileName.Text = dgOpenMapData.FileName;
                lblFileName.Visible = true;

                String[] lines = System.IO.File.ReadAllLines(dgOpenMapData.FileName);

                foreach (String line in lines) {

                    if (counter == 13) {

                        Byte[,] tileDataSet = new Byte[17, 17];
                        String newLine = line.Replace(" ", "");
                        String[] data = newLine.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        int x1 = 0;
                        int y1 = 0;

                        for (int i = 0; i < 11; i++) {
                            if (i == data.Length) break;
                            tileDataSet[y1, x1] = Byte.Parse(data[i]);

                            x1++;
                            if (x1 == newLevel.levelDimensionX) {
                                x1 = 0;
                                y1++;
                                if (y1 == newLevel.levelDimensionY) {
                                    break;
                                }
                            }
                        }

                        newLevel.tileData = tileDataSet;
                        counter++;

                    }

                    if (counter == 12) { // doors

                        String newLine = line.Replace(" ", "");
                        String[] data = newLine.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        LevelDoor door = new LevelDoor();
                        door.doorType = (ItemType)int.Parse(data[0]);
                        door.startPosX = int.Parse(data[1]);
                        door.startPosY = int.Parse(data[2]);
                        newLevel.doors.Add(door);

                        TreeNode node = treeNodeDoors.Nodes.Add(Utils.getDoorTypeDescription((ItemType)door.doorType));
                        node.ImageIndex = (int)Images.Door;
                        node.SelectedImageIndex = (int)Images.Door;
                        node.Tag = door;
                        door.node = node;

                        numberOfDoors--;
                        if (numberOfDoors == 0) { counter++; }

                    }

                    if (counter == 11) { // Number of doors ..

                        String newLine = line.Replace(",", "");
                        numberOfDoors = int.Parse(newLine.Trim());

                        if (numberOfDoors == 0) {
                            counter += 2;
                        }
                        else {
                            counter++;
                        }

                    }
                    if (counter == 10) { // items

                        String newLine = line.Replace(" ", "");
                        String[] data = newLine.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        LevelItem item = new LevelItem();
                        item.itemType = (ItemType)int.Parse(data[0]);
                        item.startPosX = int.Parse(data[1]);
                        item.startPosY = int.Parse(data[2]);
                        newLevel.items.Add(item);

                        TreeNode node = treeNodeItems.Nodes.Add(Utils.getItemTypeDescription((ItemType)item.itemType));
                        node.ImageIndex = (int)Images.Item;
                        node.SelectedImageIndex = (int)Images.Item;
                        node.Tag = item;
                        item.node = node;

                        numberOfItems--;
                        if (numberOfItems == 0) { counter++; }

                    }

                    if (counter == 9) { // Number of items ..

                        String newLine = line.Replace(",", "");
                        numberOfItems = int.Parse(newLine.Trim());

                        if (numberOfItems == 0) {
                            counter += 2;
                        }
                        else {
                            counter++;
                        }

                    }

                    if (counter == 8) { // enemies

                        String newLine = line.Replace(" ", "");
                        String[] data = newLine.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        LevelEnemy enemy = new LevelEnemy();
                        enemy.enemyType = (EnemyType)int.Parse(data[0]);
                        enemy.startPosX = int.Parse(data[1]);
                        enemy.startPosY = int.Parse(data[2]);
                        newLevel.enemies.Add(enemy);

                        TreeNode node = treeNodeEnemies.Nodes.Add(Utils.getEnemyTypeDescription((EnemyType)enemy.enemyType));
                        node.ImageIndex = (int)Images.Enemy;
                        node.SelectedImageIndex = (int)Images.Enemy;
                        node.Tag = enemy;
                        enemy.node = node;

                        numberOfEnemies--;
                        if (numberOfEnemies == 0) { counter++; }

                    }

                    if (counter == 7) { // Number of enemies ..

                        String newLine = line.Replace(",", "");
                        numberOfEnemies = int.Parse(newLine.Trim());

                        if (numberOfEnemies == 0) {
                            counter += 2;
                        }
                        else {
                            counter++;
                        }

                    }

                    if (counter == 6) { // Level dimension

                        String newLine = line.Replace(" ", "");
                        String[] data = newLine.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        newLevel.levelDimensionX = int.Parse(data[0]);
                        newLevel.levelDimensionY = int.Parse(data[1]);
                        counter++;

                    }

                    if (counter == 5) { // direction
                        newLevel.direction = int.Parse(line.Trim().Substring(0, 1));
                        counter++;
                    }

                    if (counter == 4) { // startPos

                        String newLine = line.Replace(" ", "");
                        String[] data = newLine.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        newLevel.startPosX = int.Parse(data[0]);
                        newLevel.startPosY = int.Parse(data[1]);
                        counter++;

                    }

                    if (counter == 2 || counter == 3) {

                        Byte[] caption = new Byte[11];
                        String newLine = line.Replace(" ", "");
                        String[] data = newLine.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < 11; i++) {
                            caption[i] = Byte.Parse(data[i]);
                        }

                        if (counter == 2) {
                            newLevel.line1 = Encoding.ASCII.GetString(caption);
                        }
                        else {
                            newLevel.line2 = Encoding.ASCII.GetString(caption);
                        }

                        counter++;

                    }

                    if (counter == 1) {

                        tileData = new Byte[30];
                        String newLine = line.Replace(" ", "");
                        String[] data = newLine.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < 30; i++) {
                            tileData[i] = Convert.ToByte(data[i], 16);
                        }

                        newTile.Data = tileData;
                        counter = 0;

                    }

                    if (line.StartsWith("const uint8_t PROGMEM tile_") && !line.Trim().EndsWith("{};")) {

                        newTile = new Tile();
                        newTile.Title = "Tile " + (tileCount < 10 ? "0" : "") + tileCount;
                        newTile.Click += new EventHandler(tile_Click);
                        newTile.Parent = tabPageTileEditor;
                        newTile.Location = new Point(410 + ((tileCount % 5) * 91), 6 + (((Byte)tileCount / 5) * 110));
                        newTile.Index = tileCount;
                        tiles.Add(newTile);

                        ToolStripMenuItem menuItem = new ToolStripMenuItem();
                        menuItem.Text = newTile.Title;
                        menuItem.Click += new EventHandler(mnuAddTile_Click);
                        menuItem.Tag = newTile;
                        mnuAddTiles.DropDownItems.Add(menuItem);

                        tileCount++;
                        counter = 1;

                    }

                    if (line.StartsWith("const uint8_t PROGMEM level_") && !line.Trim().EndsWith("{};")) {

                        newLevel = new Level();
                        levels.Add(newLevel);
                        counter = 2;

                        treeNode = tvwLevels.Nodes.Add("Level " + (levels.Count - 1 < 10 ? "0" : "") + (levels.Count - 1));
                        treeNode.Tag = newLevel;
                        treeNode.ImageIndex = (int)Images.Level;
                        treeNode.SelectedImageIndex = (int)Images.Level;

                        treeNodeEnemies = treeNode.Nodes.Add("Enemies");
                        treeNodeEnemies.ImageIndex = (int)Images.EnemyRoot;
                        treeNodeEnemies.SelectedImageIndex = (int)Images.EnemyRoot;

                        treeNodeItems = treeNode.Nodes.Add("Items");
                        treeNodeItems.ImageIndex = (int)Images.ItemRoot;
                        treeNodeItems.SelectedImageIndex = (int)Images.ItemRoot;

                        treeNodeDoors = treeNode.Nodes.Add("Doors");
                        treeNodeDoors.ImageIndex = (int)Images.DoorRoot;
                        treeNodeDoors.SelectedImageIndex = (int)Images.DoorRoot;
                        newLevel.node = treeNode;

                    }

                    if (line.StartsWith("#define ENEMY_")) {

                        String newLine = line.Trim();

                        if (newLine.StartsWith("#define ENEMY_BEHOLDER_HP")) { udBeholder_HP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_BEHOLDER_AP")) { udBeholder_AP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_BEHOLDER_XP")) { udBeholder_XP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_BEHOLDER_MV")) { chkBeholder_MV.Checked = (newLine.Substring(newLine.LastIndexOf(" ") + 1) == "false"); }

                        if (newLine.StartsWith("#define ENEMY_SKELETON_HP")) { udSkeleton_HP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_SKELETON_AP")) { udSkeleton_AP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_SKELETON_XP")) { udSkeleton_XP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_SKELETON_MV")) { chkSkeleton_MV.Checked = (newLine.Substring(newLine.LastIndexOf(" ") + 1) == "false"); }

                        if (newLine.StartsWith("#define ENEMY_DISPLACER_HP")) { udDisplacer_HP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_DISPLACER_AP")) { udDisplacer_AP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_DISPLACER_XP")) { udDisplacer_XP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_DISPLACER_MV")) { chkDisplacer_MV.Checked = (newLine.Substring(newLine.LastIndexOf(" ") + 1) == "false"); }

                        if (newLine.StartsWith("#define ENEMY_WRAITH_HP")) { udWraith_HP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_WRAITH_AP")) { udWraith_AP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_WRAITH_XP")) { udWraith_XP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_WRAITH_MV")) { chkWraith_MV.Checked = (newLine.Substring(newLine.LastIndexOf(" ") + 1) == "false"); }

                        if (newLine.StartsWith("#define ENEMY_DRAGON_HP")) { udDragon_HP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_DRAGON_AP")) { udDragon_AP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_DRAGON_XP")) { udDragon_XP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_DRAGON_MV")) { chkDragon_MV.Checked = (newLine.Substring(newLine.LastIndexOf(" ") + 1) == "false"); }

                        if (newLine.StartsWith("#define ENEMY_RAT_HP")) { udRat_HP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_RAT_AP")) { udRat_AP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_RAT_XP")) { udRat_XP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_RAT_MV")) { chkRat_MV.Checked = (newLine.Substring(newLine.LastIndexOf(" ") + 1) == "false"); }

                        if (newLine.StartsWith("#define ENEMY_SLIME_HP")) { udSlime_HP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_SLIME_AP")) { udSlime_AP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_SLIME_XP")) { udSlime_XP.Value = int.Parse(newLine.Substring(newLine.LastIndexOf(" ") + 1)); }
                        if (newLine.StartsWith("#define ENEMY_SLIME_MV")) { chkSlime_MV.Checked = (newLine.Substring(newLine.LastIndexOf(" ") + 1) == "false"); }

                    }

                }

            }

            cmdTileAdd.Enabled = (tileCount != NUMBER_OF_TILES);


            // Pre-select the first tile and level if they exist ..

            if (tiles.Count> 0) {

                tiles[0].Select();
                selectTile(tiles[0]);

            }

            if (levels.Count > 0) {

                tvwLevels.SelectedNode = tvwLevels.Nodes[0];
//                populateLevelEditor(tvwLevels.Nodes[0]);

            }

        }

        private void tvwLevels_AfterSelect(object sender, TreeViewEventArgs e) {

            populateLevelEditor(e.Node);

        }

        private void populateLevelEditor(TreeNode levelNode) {
        
            if ((levelNode.Tag != null) && (levelNode.Tag.GetType() == typeof(Level))) {

                Level level = (Level)levelNode.Tag;
                levelEditor.SuspendLayout();
                clearLevel();

                if (level.levelDimensionX > 0 && level.levelDimensionY > 0) {

                    for (int x = 0; x < level.levelDimensionX; x++) {

                        for (int y = 0; y < level.levelDimensionY; y++) {

                            int tileNumber = level.tileData[y, x];
                            Tile tile = tiles[tileNumber];
                            mapTileToLevel(tile.Data, x * 15, y * 15);

                        }

                    }

                }

                foreach (TreeNode node in levelNode.Nodes[0].Nodes) {

                    LevelEnemy enemy = (LevelEnemy)node.Tag;
                    levelEditor.Rows[enemy.startPosY].Cells[enemy.startPosX].Style.BackColor = Color.Red;

                }

                foreach (TreeNode node in levelNode.Nodes[1].Nodes) {

                    LevelItem item = (LevelItem)node.Tag;
                    levelEditor.Rows[item.startPosY].Cells[item.startPosX].Style.BackColor = Color.Green;

                }

                foreach (TreeNode node in levelNode.Nodes[2].Nodes) {

                    LevelDoor door = (LevelDoor)node.Tag;
                    levelEditor.Rows[door.startPosY].Cells[door.startPosX].Style.BackColor = Color.Yellow;

                }

                foreach (TreeNode node in tvwLevels.Nodes) {

                    if (node != levelNode) {
                        node.Collapse();
                    }
                }

                if (level.startPosX > 0 && level.startPosY > 0) {

                    levelEditor.Rows[level.startPosY].Cells[level.startPosX].Style.BackColor = Color.Turquoise;

                }

                levelNode.Expand();

                selectedLevel = level;
                selectedLevelNode = levelNode;
                levelEditor.Enabled = true;

                pnlLevelDetails.Visible = true;
                pnlLevelDetails.Enabled = true;
                pnlItemDetails.Visible = false;
                pnlItemDetails.Enabled = false;
                pnlBlank.Visible = false;

                txtLevelHeading1.Text = level.line1;
                txtLevelHeading2.Text = level.line2;
                cboLevelDirection.SelectedIndex = level.direction;
                txtLevelPositionX.Text = (level.startPosX >= 0 ? level.startPosX.ToString() : "");
                txtLevelPositionY.Text = (level.startPosY >= 0 ? level.startPosY.ToString() : "");

                cmdLevelDelete.Enabled = true;

                cmdLevelDown.Enabled = (tvwLevels.Nodes.Count > 1 && levelNode != tvwLevels.Nodes[tvwLevels.Nodes.Count - 1]);
                cmdLevelUp.Enabled = (tvwLevels.Nodes.Count > 1 && levelNode != tvwLevels.Nodes[0]);

                levelEditor.ResumeLayout();

            }

            if ((levelNode.Tag != null) && (levelNode.Tag.GetType() == typeof(LevelEnemy))) {

                LevelEnemy enemy = (LevelEnemy)levelNode.Tag;
                levelEditor.CurrentCell = levelEditor.Rows[enemy.startPosY].Cells[enemy.startPosX];

                if (!levelEditor.Rows[enemy.startPosY].Visible) { levelEditor.FirstDisplayedScrollingRowIndex = enemy.startPosY; }
                if (!levelEditor.Columns[enemy.startPosX].Visible) { levelEditor.FirstDisplayedScrollingColumnIndex = enemy.startPosX; }

                pnlLevelDetails.Enabled = false;
                pnlLevelDetails.Visible = false;
                pnlBlank.Visible = false;

                pnlItemDetails.Enabled = true;
                pnlItemDetails.Visible = true;
                lblItemDetails_Caption.Text = "Enemy Details";

                txtItemDetails.Text = Utils.getEnemyTypeDescription(enemy.enemyType);
                txtItemDetailsPositionX.Text = enemy.startPosX.ToString();
                txtItemDetailsPositionY.Text = enemy.startPosY.ToString();

            }

            if ((levelNode.Tag != null) && (levelNode.Tag.GetType() == typeof(LevelItem))) {

                LevelItem item = (LevelItem)levelNode.Tag;
                levelEditor.CurrentCell = levelEditor.Rows[item.startPosY].Cells[item.startPosX];

                if (!levelEditor.Rows[item.startPosY].Visible) { levelEditor.FirstDisplayedScrollingRowIndex = item.startPosY; }
                if (!levelEditor.Columns[item.startPosX].Visible) { levelEditor.FirstDisplayedScrollingColumnIndex = item.startPosX; }

                pnlLevelDetails.Enabled = false;
                pnlLevelDetails.Visible = false;
                pnlBlank.Visible = false;

                pnlItemDetails.Enabled = true;
                pnlItemDetails.Visible = true;
                lblItemDetails_Caption.Text = "Item Details";

                txtItemDetails.Text = Utils.getItemTypeDescription(item.itemType);
                txtItemDetailsPositionX.Text = item.startPosX.ToString();
                txtItemDetailsPositionY.Text = item.startPosY.ToString();

            }

            if ((levelNode.Tag != null) && (levelNode.Tag.GetType() == typeof(LevelDoor))) {

                LevelDoor door = (LevelDoor)levelNode.Tag;
                levelEditor.CurrentCell = levelEditor.Rows[door.startPosY].Cells[door.startPosX];

                if (!levelEditor.Rows[door.startPosY].Visible) { levelEditor.FirstDisplayedScrollingRowIndex = door.startPosY; }
                if (!levelEditor.Columns[door.startPosX].Visible) { levelEditor.FirstDisplayedScrollingColumnIndex = door.startPosX; }

                pnlLevelDetails.Enabled = false;
                pnlLevelDetails.Visible = false;
                pnlBlank.Visible = false;

                pnlItemDetails.Enabled = true;
                pnlItemDetails.Visible = true;
                lblItemDetails_Caption.Text = "Door Details";

                txtItemDetails.Text = Utils.getDoorTypeDescription(door.doorType);
                txtItemDetailsPositionX.Text = door.startPosX.ToString();
                txtItemDetailsPositionY.Text = door.startPosY.ToString();


            }

        }

        private void clearLevel() {

            levelEditor.SuspendLayout();
            for (int y = 0; y < 255; y++) {

                for (int x = 0; x < 255; x++) {

                    levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor = Color.FromArgb(245, 245, 245); ;

                }

            }
            levelEditor.ResumeLayout();

        }

        private void clearTile() {

            for (int y = 0; y < 15; y++) {

                for (int x = 0; x < 15; x++) {

                    tileEditor.Rows[y].Cells[x].Style.BackColor = Color.White;

                }

            }

        }

        private void mnuEnemyAddSlime_Click(object sender, EventArgs e) {
            mnuEnemyAdd(EnemyType.Slime);
        }

        private void mnuEnemyAddBeholder_Click(object sender, EventArgs e) {
            mnuEnemyAdd(EnemyType.Beholder);
        }

        private void mnuEnemyAddSkeleton_Click(object sender, EventArgs e) {
            mnuEnemyAdd(EnemyType.Skeleton);
        }

        private void mnuEnemyAddDisplacer_Click(object sender, EventArgs e) {
            mnuEnemyAdd(EnemyType.Displacer);
        }

        private void mnuEnemyAddWraith_Click(object sender, EventArgs e) {
            mnuEnemyAdd(EnemyType.Wraith);
        }

        private void mnuEnemyAddDragon_Click(object sender, EventArgs e) {
            mnuEnemyAdd(EnemyType.Dragon);
        }

        private void mnuEnemyAddRat_Click(object sender, EventArgs e) {
            mnuEnemyAdd(EnemyType.Rat);
        }

        private void mnuEnemyAdd(EnemyType enemyType) {

            LevelEnemy enemy = new LevelEnemy();
            enemy.enemyType = enemyType;
            enemy.startPosX = selectedCol;
            enemy.startPosY = selectedRow;

            levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor = Color.Red;
            selectedLevel.enemies.Add(enemy);

            TreeNode node = selectedLevelNode.Nodes[0].Nodes.Add(Utils.getEnemyTypeDescription((EnemyType)enemy.enemyType));
            node.ImageIndex = (int)Images.Enemy;
            node.SelectedImageIndex = (int)Images.Enemy;
            node.Tag = enemy;
            enemy.node = node;
            validateDungeons();

        }

        private void mnuEnemyDelete_Click(object sender, EventArgs e) {

            foreach (TreeNode node in selectedLevelNode.Nodes[0].Nodes) {

                LevelEnemy enemy = (LevelEnemy)node.Tag;

                if ((enemy.startPosX == selectedCol) && (enemy.startPosY == selectedRow)) {

                    selectedLevel.enemies.Remove(enemy);
                    selectedLevelNode.Nodes[0].Nodes.Remove(node);
                    break;

                }

            }

            levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor = Color.White;
            validateDungeons();

        }

        private void mnuItemAddPotion_Click(object sender, EventArgs e) {
            mnuItemAdd(ItemType.Potion);
        }

        private void mnuItemAddKey_Click(object sender, EventArgs e) {
            mnuItemAdd(ItemType.Key);
        }

        private void mnuItemAddScroll_Click(object sender, EventArgs e) {
            mnuItemAdd(ItemType.Scroll);
        }

        private void mnuItemAdd(ItemType itemType) {

            LevelItem item = new LevelItem();
            item.itemType = itemType;
            item.startPosX = selectedCol;
            item.startPosY = selectedRow;

            levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor = Color.Green;
            selectedLevel.items.Add(item);

            TreeNode node = selectedLevelNode.Nodes[1].Nodes.Add(Utils.getItemTypeDescription((ItemType)item.itemType));
            node.ImageIndex = (int)Images.Item;
            node.SelectedImageIndex = (int)Images.Item;
            node.Tag = item;
            item.node = node;
            validateDungeons();

        }

        private void mnuItemDelete_Click(object sender, EventArgs e) {

            foreach (TreeNode node in selectedLevelNode.Nodes[1].Nodes) {

                LevelItem item = (LevelItem)node.Tag;

                if ((item.startPosX == selectedCol) && (item.startPosY == selectedRow)) {

                    selectedLevel.items.Remove(item);
                    selectedLevelNode.Nodes[1].Nodes.Remove(node);
                    break;

                }

            }

            levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor = Color.White;
            validateDungeons();

        }

        private void mnuDoorAddLevel_Click(object sender, EventArgs e) {
            mnuDoorAdd(ItemType.LockedDoor);
        }

        private void mnuDoorAddGate_Click(object sender, EventArgs e) {
            mnuDoorAdd(ItemType.LockedGate);
        }

        private void mnuDoorAdd(ItemType doorType) {

            LevelDoor door = new LevelDoor();
            door.doorType = doorType;
            door.startPosX = selectedCol;
            door.startPosY = selectedRow;

            levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor = Color.Yellow;
            selectedLevel.doors.Add(door);

            TreeNode node = selectedLevelNode.Nodes[2].Nodes.Add(Utils.getDoorTypeDescription((ItemType)door.doorType));
            node.ImageIndex = (int)Images.Door;
            node.SelectedImageIndex = (int)Images.Door;
            node.Tag = door;
            door.node = node;
            validateDungeons();

        }

        private void mnuDoorDelete_Click(object sender, EventArgs e) {

            foreach (TreeNode node in selectedLevelNode.Nodes[2].Nodes) {

                LevelDoor door = (LevelDoor)node.Tag;

                if ((door.startPosX == selectedCol) && (door.startPosY == selectedRow)) {

                    selectedLevel.doors.Remove(door);
                    selectedLevelNode.Nodes[2].Nodes.Remove(node);
                    break;

                }

            }

            levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor = Color.White;
            validateDungeons();

        }

        private void cmdLevelAdd_Click(object sender, EventArgs e) {

            Level newLevel = new Level();
            newLevel.tileData = new Byte[17, 17];
            newLevel.line1 = "";
            newLevel.line2 = "";
            levels.Add(newLevel);

            TreeNode treeNode = tvwLevels.Nodes.Add("Level " + (levels.Count - 1 < 10 ? "0" : "") + (levels.Count - 1));
            treeNode.Tag = newLevel;
            treeNode.ImageIndex = (int)Images.Level;
            treeNode.SelectedImageIndex = (int)Images.Level;
            newLevel.node = treeNode;

            TreeNode treeNodeEnemies = treeNode.Nodes.Add("Enemies");
            treeNodeEnemies.ImageIndex = (int)Images.EnemyRoot;
            treeNodeEnemies.SelectedImageIndex = (int)Images.EnemyRoot;

            TreeNode treeNodeItems = treeNode.Nodes.Add("Items");
            treeNodeItems.ImageIndex = (int)Images.ItemRoot;
            treeNodeItems.SelectedImageIndex = (int)Images.ItemRoot;

            TreeNode treeNodeDoors = treeNode.Nodes.Add("Doors");
            treeNodeDoors.ImageIndex = (int)Images.DoorRoot;
            treeNodeDoors.SelectedImageIndex = (int)Images.DoorRoot;

            tvwLevels.CollapseAll();
            tvwLevels.SelectedNode = treeNode;
            treeNode.Expand();

            if (tvwLevels.Nodes.Count == NUMBER_OF_LEVELS) {

                cmdLevelAdd.Enabled = false;

            }

            validateDungeons();

        }

        private void txtLevelHeading1_KeyPress(object sender, KeyPressEventArgs e) {

            if (e.KeyChar != (Char)Keys.Back) {

                if (System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"[^0-9^A-Z^\.^\!^ ]")) {
                    e.Handled = true;
                }

            }

        }

        private void txtLevelHeading2_KeyPress(object sender, KeyPressEventArgs e) {

            if (e.KeyChar != (Char)Keys.Back) {

                if (System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"[^0-9^A-Z^\.^\!^ ]")) {
                    e.Handled = true;
                }

            }

        }

        private void tvwLevels_AfterExpand(object sender, TreeViewEventArgs e) {

            if ((e.Node.Tag != null) && (e.Node.Tag.GetType() == typeof(Level))) {

                selectedLevelNode = null;

                foreach (TreeNode node in tvwLevels.Nodes) {

                    if (node != e.Node) {
                        node.Collapse();
                    }

                }

            }

            tvwLevels.SelectedNode = e.Node;

        }

        private void tvwLevels_BeforeCollapse(object sender, TreeViewCancelEventArgs e) {

            if ((e.Node.Tag != null) && (e.Node.Tag.GetType() == typeof(Level))) {
                if (Utils.getRootNode(e.Node) == selectedLevelNode) { e.Cancel = true; }
            }

        }

        private void cmdLevelDelete_Click(object sender, EventArgs e) {

            int count = 0;

            levels.Remove(selectedLevel);
            tvwLevels.Nodes.Remove(selectedLevelNode);

            foreach (TreeNode node in tvwLevels.Nodes) {

                node.Text = "Level " + (count < 10 ? "0" : "") + count;
                count++;
            }

            if (tvwLevels.Nodes.Count > 0) {

                tvwLevels.SelectedNode = tvwLevels.Nodes[0];

            }
            else {

                clearLevel();
                levelEditor.Enabled = false;
                cmdLevelDelete.Enabled = false;

            }

            validateDungeons();

        }

        private void txtLevelHeading1_TextChanged(object sender, EventArgs e) {

            selectedLevel.line1 = txtLevelHeading1.Text;

        }

        private void txtLevelHeading2_TextChanged(object sender, EventArgs e) {

            selectedLevel.line2 = txtLevelHeading2.Text;

        }

        private void cboLevelDirection_SelectedIndexChanged(object sender, EventArgs e) {

            selectedLevel.direction = cboLevelDirection.SelectedIndex;

        }

        private void mnuPlacePlayer_Click(object sender, EventArgs e) {

            for (int y = 0; y < 255; y++) {

                for (int x = 0; x < 255; x++) {

                    if (levelEditor.Rows[y].Cells[x].Style.BackColor == Color.Turquoise) {
                        levelEditor.Rows[y].Cells[x].Style.BackColor = Color.White;
                    }

                }

            }

            selectedLevel.startPosX = selectedCol;
            selectedLevel.startPosY = selectedRow;

            txtLevelPositionX.Text = selectedLevel.startPosX.ToString();
            txtLevelPositionY.Text = selectedLevel.startPosY.ToString();

            levelEditor.Rows[selectedRow].Cells[selectedCol].Style.BackColor = Color.Turquoise;
            validateDungeons();

        }

        private void cmdLevelUp_Click(object sender, EventArgs e) {

            int index = tvwLevels.SelectedNode.Index - 1;

            levels.Remove(selectedLevel);
            levels.Insert(index, selectedLevel);

            TreeNode newLevel = (TreeNode)selectedLevelNode.Clone();

            tvwLevels.Nodes.Remove(selectedLevelNode);
            tvwLevels.Nodes.Insert(index, newLevel);
            selectedLevelNode = newLevel;
            tvwLevels.SelectedNode = newLevel;

            int count = 0;

            foreach (TreeNode node in tvwLevels.Nodes) {

                node.Text = "Level " + (count < 10 ? "0" : "") + count;
                count++;
            }

        }

        private void cmdLevelDown_Click(object sender, EventArgs e) {

            int index = tvwLevels.SelectedNode.Index + 1;

            levels.Remove(selectedLevel);
            levels.Insert(index, selectedLevel);

            TreeNode newLevel = (TreeNode)selectedLevelNode.Clone();

            tvwLevels.Nodes.Remove(selectedLevelNode);
            tvwLevels.Nodes.Insert(index, newLevel);
            selectedLevelNode = newLevel;
            tvwLevels.SelectedNode = newLevel;

            int count = 0;

            foreach (TreeNode node in tvwLevels.Nodes) {

                node.Text = "Level " + (count < 10 ? "0" : "") + count;
                count++;
            }

        }

        private void tileEditor_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {

            if (mouseDown) {

                int index = e.ColumnIndex + ((Byte)(e.RowIndex / 8) * 15);

                if (cellDragColour == Color.White) {
                    tileEditor.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                    tileData[index] = (Byte)(tileData[index] & ~(Byte)(1 << (e.RowIndex % 8)));
                }
                else {
                    tileEditor.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Gray;
                    tileData[index] = (Byte)(tileData[index] | (Byte)(1 << (e.RowIndex % 8)));
                }
            }

        }

        private void tileEditor_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e) {

            mouseDown = true;

            int x = (MousePosition.X - tileEditor.Parent.PointToScreen(tileEditor.Location).X) / 25;
            int y = (MousePosition.Y - tileEditor.Parent.PointToScreen(tileEditor.Location).Y) / 25;
            int index = x + ((Byte)(y / 8) * 15);

            if (tileEditor.Rows[y].Cells[x].Style.BackColor == Color.Gray) {

                cellDragColour = Color.White;
                tileEditor.Rows[y].Cells[x].Style.BackColor = Color.White;
                tileData[index] = (Byte)(tileData[index] & ~(Byte)(1 << (y % 8)));


            }
            else {

                cellDragColour = Color.Gray;
                tileEditor.Rows[y].Cells[x].Style.BackColor = Color.Gray;
                tileData[index] = (Byte)(tileData[index] | (Byte)(1 << (y % 8)));

            }

            tileEditor.ClearSelection();
            tileEditor.CurrentCell = null;

        }

        private void tileEditor_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e) {
            mouseDown = false;
        }

        private void tileEditor_MouseLeave(object sender, EventArgs e) {
            mouseDown = false;
        }

        private void mnuAbout_Click(object sender, EventArgs e) {

            frmAboutBox aboutBox = new frmAboutBox();
            aboutBox.ShowDialog(this);

        }

        private void cmdTileDelete_Click(object sender, EventArgs e) {

            int count = 0;
            int tileIndex = selectedTile.Index;

            tiles.Remove(selectedTile);
            tabPageTileEditor.Controls.Remove(selectedTile);


            foreach (Tile tile in tiles) {

                tile.Location = new Point(410 + ((count % 5) * 91), 6 + (((Byte)count / 5) * 110));
                tile.Title = "Tile " + (count < 10 ? "0" : "") + count;
                count++;

            }

            foreach (Level level in levels) {

                for (int x = 0; x < 17; x++) {

                    for (int y = 0; y < 17; y++) {

                        if (level.tileData[y, x] > tileIndex) {
                            level.tileData[y, x] = (Byte)(level.tileData[y, x] - 1);
                        }

                    }

                }

            }

            cmdTileAdd.Enabled = true;
            cmdTileDelete.Enabled = false;
            clearTile();
            tileEditor.Enabled = false;

        }

        private void mnuSaveMapData_Click(object sender, EventArgs e) {

            validateDungeons();

            if (tvwErrors.Nodes.Count > 0) {

                MessageBox.Show(this, "One or more errors exist and must be corrected.", "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else {

                if (dgOpenMapData.FileName != "") {

                    saveMapData(dgOpenMapData.FileName);

                }
                else {

                    mnuSaveMapDataAs_Click(sender, e);

                }

            }

        }

        private void mnuSaveMapDataAs_Click(object sender, EventArgs e) {

            validateDungeons();

            if (tvwErrors.Nodes.Count > 0) {

                MessageBox.Show(this, "One or more errors exist and must be corrected.", "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else {

                if (dgSaveMapData.ShowDialog() == DialogResult.OK) {

                    dgOpenMapData.FileName = dgSaveMapData.FileName;
                    saveMapData(dgOpenMapData.FileName);

                }

            }

        }

        private void saveMapData(String fileName) {

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName)) {

                lblFileName.Text = fileName;
                lblFileName.Visible = true;

                file.WriteLine("#pragma once");
                file.WriteLine("");

                file.WriteLine("#define MAX_LEVEL_COUNT {0}", levels.Count);
                file.WriteLine("");
                file.WriteLine("#define ENEMY_BEHOLDER_HP {0}", udBeholder_HP.Value);
                file.WriteLine("#define ENEMY_BEHOLDER_AP {0}", udBeholder_AP.Value);
                file.WriteLine("#define ENEMY_BEHOLDER_XP {0}", udBeholder_XP.Value);
                file.WriteLine("#define ENEMY_BEHOLDER_MV {0}", (!chkBeholder_MV.Checked).ToString().ToLower());
                file.WriteLine("");
                file.WriteLine("#define ENEMY_SKELETON_HP {0}", udSkeleton_HP.Value);
                file.WriteLine("#define ENEMY_SKELETON_AP {0}", udSkeleton_AP.Value);
                file.WriteLine("#define ENEMY_SKELETON_XP {0}", udSkeleton_XP.Value);
                file.WriteLine("#define ENEMY_SKELETON_MV {0}", (!chkSkeleton_MV.Checked).ToString().ToLower());
                file.WriteLine("");
                file.WriteLine("#define ENEMY_DISPLACER_HP {0}", udDisplacer_HP.Value);
                file.WriteLine("#define ENEMY_DISPLACER_AP {0}", udDisplacer_AP.Value);
                file.WriteLine("#define ENEMY_DISPLACER_XP {0}", udDisplacer_XP.Value);
                file.WriteLine("#define ENEMY_DISPLACER_MV {0}", (!chkDisplacer_MV.Checked).ToString().ToLower());
                file.WriteLine("");
                file.WriteLine("#define ENEMY_WRAITH_HP {0}", udWraith_HP.Value);
                file.WriteLine("#define ENEMY_WRAITH_AP {0}", udWraith_AP.Value);
                file.WriteLine("#define ENEMY_WRAITH_XP {0}", udWraith_XP.Value);
                file.WriteLine("#define ENEMY_WRAITH_MV {0}", (!chkWraith_MV.Checked).ToString().ToLower());
                file.WriteLine("");
                file.WriteLine("#define ENEMY_DRAGON_HP {0}", udDragon_HP.Value);
                file.WriteLine("#define ENEMY_DRAGON_AP {0}", udDragon_AP.Value);
                file.WriteLine("#define ENEMY_DRAGON_XP {0}", udDragon_XP.Value);
                file.WriteLine("#define ENEMY_DRAGON_MV {0}", (!chkDragon_MV.Checked).ToString().ToLower());
                file.WriteLine("");
                file.WriteLine("#define ENEMY_RAT_HP {0}", udRat_HP.Value);
                file.WriteLine("#define ENEMY_RAT_AP {0}", udRat_AP.Value);
                file.WriteLine("#define ENEMY_RAT_XP {0}", udRat_XP.Value);
                file.WriteLine("#define ENEMY_RAT_MV {0}", (!chkRat_MV.Checked).ToString().ToLower());
                file.WriteLine("");
                file.WriteLine("#define ENEMY_SLIME_HP {0}", udSlime_HP.Value);
                file.WriteLine("#define ENEMY_SLIME_AP {0}", udSlime_AP.Value);
                file.WriteLine("#define ENEMY_SLIME_XP {0}", udSlime_XP.Value);
                file.WriteLine("#define ENEMY_SLIME_MV {0}", (!chkSlime_MV.Checked).ToString().ToLower());
                file.WriteLine("");

                // Tiles
                int count = 0;
                foreach (Tile tile in tiles) {
                    file.WriteLine("const uint8_t PROGMEM tile_{0}[] = {{", count.ToString("D2"));
                    for (int i = 0; i < 30; i++) {
                        file.Write("0x{0}, ", tile.Data[i].ToString("x"));
                    }
                    file.WriteLine("\n};");
                    count++;
                }

                for (int i = count; i < NUMBER_OF_TILES; i++) {
                    file.WriteLine("const uint8_t PROGMEM tile_{0}[] = {{}};", count.ToString("D2"));
                    count++;
                }

                file.WriteLine("");


                // Levels

                count = 0;
                foreach (Level level in levels) {
                    file.WriteLine("const uint8_t PROGMEM level_{0}[] = {{", count.ToString("D2"));


                    // Line 1

                    int tokenCount = 0;
                    if (level.line1 == null) level.line1 = "";
                    for (int i = 0; i < level.line1.Length; i++) {
                        file.Write("{0}, ", Convert.ToInt32(Convert.ToChar(level.line1.Substring(i, 1))));
                        tokenCount++;
                    }
                    for (int i = tokenCount; i < 11; i++) {
                        file.Write("32, ");
                    }
                    file.WriteLine("");


                    // Line 2

                    tokenCount = 0;
                    if (level.line2 == null) level.line2 = "";
                    for (int i = 0; i < level.line2.Length; i++) {
                        file.Write("{0}, ", Convert.ToInt32(Convert.ToChar(level.line2.Substring(i, 1))));
                        tokenCount++;
                    }
                    for (int i = tokenCount; i < 11; i++) {
                        file.Write("32, ");
                    }
                    file.WriteLine("");


                    // Player position ..

                    file.WriteLine("{0}, {1},", level.startPosX, level.startPosY);
                    file.WriteLine("{0},", level.direction);


                    // Level Dimensions ..

                    file.WriteLine("{0}, {1},", level.levelDimensionX, level.levelDimensionY);


                    // Enemies ..

                    file.WriteLine("{0},", level.enemies.Count);
                    foreach (LevelEnemy enemy in level.enemies) {

                        file.WriteLine("{0}, {1}, {2},", (int)enemy.enemyType, enemy.startPosX, enemy.startPosY);

                    }


                    // Items ..

                    file.WriteLine("{0},", level.items.Count);
                    foreach (LevelItem item in level.items) {

                        file.WriteLine("{0}, {1}, {2},", (int)item.itemType, item.startPosX, item.startPosY);

                    }


                    // Doors ..

                    file.WriteLine("{0},", level.doors.Count);
                    foreach (LevelDoor door in level.doors) {

                        file.WriteLine("{0}, {1}, {2},", (int)door.doorType, door.startPosX, door.startPosY);

                    }


                    // Tile info ..

                    for (int x = 0; x < level.levelDimensionX; x++) {

                        for (int y = 0; y < level.levelDimensionY; y++) {

                            file.Write("{0}, ", level.tileData[y, x]);
                        }

                    }

                    file.WriteLine("\n};");
                    count++;

                }

                for (int i = count; i < NUMBER_OF_LEVELS; i++) {
                    file.WriteLine("const uint8_t PROGMEM level_{0}[] = {{}};", count.ToString("D2")); ;
                    count++;
                }

                file.WriteLine("");

            }

        }

        private void mnuExit_Click(object sender, EventArgs e) {

            this.Close();

        }

        private void validateDungeons() {

            int count = 0;
            int errorCount = 0;
            tvwErrors.Nodes.Clear();
            lblStatusError.Text = "";

            foreach (Level level in levels) {

                List<LevelError> errors = Utils.validateLevel(level, tiles);

                if (errors.Count > 0) {

                    TreeNode node = tvwErrors.Nodes.Add("Level " + (count - 1 < 10 ? "0" : "") + count);
                    node.SelectedImageIndex = (int)Images.Level;
                    node.ImageIndex = (int)Images.Level;

                    foreach (LevelError levelError in errors) {

                        TreeNode child = node.Nodes.Add(levelError.error);
                        child.Tag = levelError;
                        child.SelectedImageIndex = (int)Images.Error;
                        child.ImageIndex = (int)Images.Error;

                    }

                    errorCount = errorCount + errors.Count;

                }

                count++;

            }

            if (tvwErrors.Nodes.Count > 0) {
                tabErrors.Text = "Errors (" + errorCount + ")";
                tabErrors.ImageIndex = (int)Images.Error;
            }
            else {
                tabErrors.Text = "Errors";
                tabErrors.ImageIndex = -1;
            }

            tvwErrors.ExpandAll();

        }

        private void tvwErrors_AfterSelect(object sender, TreeViewEventArgs e) {

            if (e.Node.Tag != null) {

                tabs.SelectedTab = tabLevelEdito;

                LevelError levelError = (LevelError)e.Node.Tag;

                if (levelError.node.Tag.GetType() == typeof(Level)) {

                    tvwLevels.SelectedNode = levelError.node;
                    levelError.node.EnsureVisible();
                    lblStatusError.Text = levelError.error;

                }

                if (levelError.node.Tag.GetType() != typeof(Level)) {

                    if (selectedLevelNode == null && selectedLevelNode != Utils.getRootNode(levelError.node)) {
                        tvwLevels.SelectedNode = Utils.getRootNode(levelError.node);
                    }

                    levelError.node.EnsureVisible();
                    tvwLevels.SelectedNode = levelError.node;
                    lblStatusError.Text = levelError.error;

                }

            }

        }

        private void tvwLevels_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {

            mnuTreeViewDeleteEnemy.Enabled = (e.Node.Tag != null && e.Node.Tag.GetType() == typeof(LevelEnemy));
            mnuTreeViewDeleteItem.Enabled = (e.Node.Tag != null && e.Node.Tag.GetType() == typeof(LevelItem));
            mnuTreeViewDeleteDoor.Enabled = (e.Node.Tag != null && e.Node.Tag.GetType() == typeof(LevelDoor));

            if (e.Button == MouseButtons.Right) {
                mnuTreeViewContext.Show(Cursor.Position);
            }

            populateLevelEditor(e.Node);

        }

        private void mnuTreeViewDeleteEnemy_Click(object sender, EventArgs e) {

            LevelEnemy levelEnemy = (LevelEnemy)tvwLevels.SelectedNode.Tag;
            TreeNode rootNode = Utils.getRootNode(tvwLevels.SelectedNode);
            TreeNode parentNode = tvwLevels.SelectedNode.Parent;
            selectedLevel.enemies.Remove(levelEnemy);
            tvwLevels.Nodes.Remove(tvwLevels.SelectedNode);
            tvwLevels.SelectedNode = parentNode;
            populateLevelEditor(rootNode);

            validateDungeons();

        }

        private void mnuTreeViewDeleteItem_Click(object sender, EventArgs e) {

            LevelItem levelItem = (LevelItem)tvwLevels.SelectedNode.Tag;
            TreeNode rootNode = Utils.getRootNode(tvwLevels.SelectedNode);
            TreeNode parentNode = tvwLevels.SelectedNode.Parent;
            selectedLevel.items.Remove(levelItem);
            tvwLevels.Nodes.Remove(tvwLevels.SelectedNode);
            tvwLevels.SelectedNode = parentNode;
            populateLevelEditor(rootNode);

            validateDungeons();

        }

        private void mnuTreeViewDeleteDoor_Click(object sender, EventArgs e) {

            LevelDoor levelDoor = (LevelDoor)tvwLevels.SelectedNode.Tag;
            TreeNode rootNode = Utils.getRootNode(tvwLevels.SelectedNode);
            TreeNode parentNode = tvwLevels.SelectedNode.Parent;
            selectedLevel.doors.Remove(levelDoor);
            tvwLevels.Nodes.Remove(tvwLevels.SelectedNode);
            tvwLevels.SelectedNode = parentNode;
            populateLevelEditor(rootNode);

            validateDungeons();

        }

        private void mnuClearMapData_Click(object sender, EventArgs e) {

            if (MessageBox.Show(this, "Clear exsiting map data?", "Confirm Map Clear", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {

                clearLevels();
                clearTile();
                clearLevel();

                lblFileName.Visible = false;
                dgSaveMapData.FileName = "";
                dgOpenMapData.FileName = "";

            }

        }

        private void tvwLevels_MouseClick(object sender, MouseEventArgs e) {

            TreeViewHitTestInfo info = tvwLevels.HitTest(e.X, e.Y);

            if (info.Node != null) {
                tvwLevels.SelectedNode = info.Node;
            }

        }

        private void mnuValidate_Click(object sender, EventArgs e) {

            validateDungeons();

        }

    }

}
