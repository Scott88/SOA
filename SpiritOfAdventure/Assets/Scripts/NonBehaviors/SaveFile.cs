using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class SaveFile
{
    public class LevelState
    {
        public string name = "";
        public int stars = 0;
        public int path = 0;
        public int score = 0;

        public LevelState() { }
        public LevelState(string levelName)
        {
            name = levelName;
        }
    }

    public class TileInfo
    {
        public int x, y;

        public BlockType type;

        public override bool Equals(System.Object obj)
        {
            TileInfo other = obj as TileInfo;

            return (x == other.x) && (y == other.y);
        }
    }

    private List<LevelState> levelStates;

    private List<TileInfo> tileSetInfo;

    private int costumeID;

    private int greenSpirits, blueSpirits, redSpirits;

    private int greenSpiritInv, blueSpiritInv, redSpiritInv;

    private int woodBlockInv, stoneBlockInv, metalBlockInv;

    private static SaveFile instance;

    private XmlDocument saveFile;

    public static SaveFile Instance()
    {
        if (instance == null)
        {
            instance = new SaveFile();
        }

		return instance;
    }

    private SaveFile()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + "/soa.xml"))
        {
            saveFile = TransferFromPrefs();
        }
        else
        {
            saveFile = new XmlDocument();
            saveFile.Load(Application.persistentDataPath + "/soa.xml");
        }

        LoadFromXML();
    }

    private XmlDocument TransferFromPrefs()
    {
        XmlDocument doc = new XmlDocument();

        XmlElement save = doc.CreateElement("Save");
        doc.AppendChild(save);

        XmlElement costume = doc.CreateElement("Costume");

        XmlAttribute costumeNum = doc.CreateAttribute("id");
        costumeNum.Value = PlayerPrefs.GetInt("costume").ToString();

        costume.Attributes.Append(costumeNum);

        save.AppendChild(costume);

        XmlElement levels = doc.CreateElement("Levels");
        save.AppendChild(levels);

        for (int j = 1; j <= 7; j++)
        {
            string sceneName = "Level0" + j;

            XmlElement level = doc.CreateElement("Level");

            XmlAttribute levelName = doc.CreateAttribute("name");
            levelName.Value = sceneName;

            level.Attributes.Append(levelName);

            XmlAttribute levelStars = doc.CreateAttribute("stars");
            levelStars.Value = PlayerPrefs.GetInt(sceneName).ToString();

            level.Attributes.Append(levelStars);

            XmlAttribute levelPath = doc.CreateAttribute("path");
            levelPath.Value = "0";

            level.Attributes.Append(levelPath);

            levels.AppendChild(level);
        }

        doc.Save(Application.persistentDataPath + "/soa.xml");

        return doc;
    }

    void LoadFromXML()
    {
        XmlElement saveNode = saveFile["Save"];

        costumeID = int.Parse(saveNode["Costume"].GetAttribute("id"));

        levelStates = new List<LevelState>();

        XmlElement nextLevel = saveNode["Levels"]["Level"];

        while (nextLevel != null)
        {
            LevelState state = new LevelState();

            state.name = nextLevel.GetAttribute("name");
            state.stars = int.Parse(nextLevel.GetAttribute("stars"));
            state.path = int.Parse(nextLevel.GetAttribute("path"));

            if (nextLevel.GetAttribute("score") != "")
            {
                state.score = int.Parse(nextLevel.GetAttribute("score"));
            }

            levelStates.Add(state);

            nextLevel = (XmlElement)nextLevel.NextSibling;
        }

        tileSetInfo = new List<TileInfo>();

        XmlElement tileNode = saveNode["Tiles"];

        if (tileNode != null)
        {
            XmlElement nextTile = tileNode["Tile"];

            while (nextTile != null)
            {
                TileInfo info = new TileInfo();

                info.x = int.Parse(nextTile.GetAttribute("x"));
                info.y = int.Parse(nextTile.GetAttribute("y"));
                info.type = (BlockType)System.Enum.Parse(typeof(BlockType), nextTile.GetAttribute("type"));

                tileSetInfo.Add(info);

                nextTile = (XmlElement)nextTile.NextSibling;
            }
        }

        XmlElement spiritsNode = saveNode["Spirits"];

        if (spiritsNode != null)
        {
            greenSpirits = int.Parse(spiritsNode.GetAttribute("green"));
            blueSpirits = int.Parse(spiritsNode.GetAttribute("blue"));
            redSpirits = int.Parse(spiritsNode.GetAttribute("red"));
        }

        XmlElement spiritInvNode = saveNode["SpiritInventory"];

        if (spiritInvNode != null)
        {
            greenSpiritInv = int.Parse(spiritInvNode.GetAttribute("green"));
            blueSpiritInv = int.Parse(spiritInvNode.GetAttribute("blue"));
            redSpiritInv = int.Parse(spiritInvNode.GetAttribute("red"));
        }

        XmlElement blockInvNode = saveNode["BlockInventory"];

        if (blockInvNode != null)
        {
            woodBlockInv = int.Parse(blockInvNode.GetAttribute("wood"));
            stoneBlockInv = int.Parse(blockInvNode.GetAttribute("stone"));
            metalBlockInv = int.Parse(blockInvNode.GetAttribute("metal"));
        }
        else
        {
            woodBlockInv = 10;
            stoneBlockInv = 10;
            metalBlockInv = 10;
        }
    }

    public void SaveToXML()
    {
        XmlDocument doc = new XmlDocument();

        XmlElement save = doc.CreateElement("Save");
        doc.AppendChild(save);

        XmlElement costume = doc.CreateElement("Costume");

        XmlAttribute costumeNum = doc.CreateAttribute("id");
        costumeNum.Value = costumeID.ToString();

        costume.Attributes.Append(costumeNum);

        save.AppendChild(costume);

        XmlElement levels = doc.CreateElement("Levels");
        save.AppendChild(levels);

        for (int j = 0; j < levelStates.Count; j++)
        {
            LevelState state = levelStates[j];

            string sceneName = "Level0" + j;

            XmlElement level = doc.CreateElement("Level");

            XmlAttribute levelName = doc.CreateAttribute("name");
            levelName.Value = state.name;

            level.Attributes.Append(levelName);

            XmlAttribute levelStars = doc.CreateAttribute("stars");
            levelStars.Value = state.stars.ToString();

            level.Attributes.Append(levelStars);

            XmlAttribute levelPath = doc.CreateAttribute("path");
            levelPath.Value = state.path.ToString();

            level.Attributes.Append(levelPath);

            XmlAttribute levelScore = doc.CreateAttribute("score");
            levelScore.Value = state.score.ToString();

            level.Attributes.Append(levelScore);

            levels.AppendChild(level);
        }

        XmlElement tiles = doc.CreateElement("Tiles");
        save.AppendChild(tiles);

        for (int j = 0; j < tileSetInfo.Count; j++)
        {
            TileInfo info = tileSetInfo[j];

            XmlElement tile = doc.CreateElement("Tile");

            XmlAttribute tileX = doc.CreateAttribute("x");
            tileX.Value = info.x.ToString();

            tile.Attributes.Append(tileX);

            XmlAttribute tileY = doc.CreateAttribute("y");
            tileY.Value = info.y.ToString();

            tile.Attributes.Append(tileY);

            XmlAttribute tileType = doc.CreateAttribute("type");
            tileType.Value = info.type.ToString();

            tile.Attributes.Append(tileType);

            tiles.AppendChild(tile);
        }

        {
            XmlElement spirits = doc.CreateElement("Spirits");
            save.AppendChild(spirits);

            XmlAttribute spiritsRed = doc.CreateAttribute("red");
            spiritsRed.Value = redSpirits.ToString();

            spirits.Attributes.Append(spiritsRed);

            XmlAttribute spiritsGreen = doc.CreateAttribute("green");
            spiritsGreen.Value = greenSpirits.ToString();

            spirits.Attributes.Append(spiritsGreen);

            XmlAttribute spiritsBlue = doc.CreateAttribute("blue");
            spiritsBlue.Value = blueSpirits.ToString();

            spirits.Attributes.Append(spiritsBlue);
        }

        {
            XmlElement spiritInv = doc.CreateElement("SpiritInventory");
            save.AppendChild(spiritInv);

            XmlAttribute spiritInvRed = doc.CreateAttribute("red");
            spiritInvRed.Value = redSpiritInv.ToString();

            spiritInv.Attributes.Append(spiritInvRed);

            XmlAttribute spiritInvGreen = doc.CreateAttribute("green");
            spiritInvGreen.Value = greenSpiritInv.ToString();

            spiritInv.Attributes.Append(spiritInvGreen);

            XmlAttribute spiritInvBlue = doc.CreateAttribute("blue");
            spiritInvBlue.Value = blueSpiritInv.ToString();

            spiritInv.Attributes.Append(spiritInvBlue);
        }

        {
            XmlElement blockInv = doc.CreateElement("BlockInventory");
            save.AppendChild(blockInv);

            XmlAttribute blockInvWood = doc.CreateAttribute("wood");
            blockInvWood.Value = woodBlockInv.ToString();

            blockInv.Attributes.Append(blockInvWood);

            XmlAttribute blockInvStone = doc.CreateAttribute("stone");
            blockInvStone.Value = stoneBlockInv.ToString();

            blockInv.Attributes.Append(blockInvStone);

            XmlAttribute blockInvMetal = doc.CreateAttribute("metal");
            blockInvMetal.Value = metalBlockInv.ToString();

            blockInv.Attributes.Append(blockInvMetal);
        }

        doc.Save(Application.persistentDataPath + "/soa.xml");
    }

    public void SetLevelStars(string levelName, int stars)
    {
        GetLevelState(levelName).stars = stars;
    }

    public void SetLevelPath(string levelName, int path)
    {
        GetLevelState(levelName).path = path;
    }

    public void SetLevelScore(string levelName, int score)
    {
        GetLevelState(levelName).score = score;
    }

    public void SetCurrentCostume(int id)
    {
        costumeID = id;
    }

    public void SetSpiritCount(SpiritType type, int count)
    {
        switch (type)
        {
            case SpiritType.ST_GREEN:
                greenSpirits = count;
                break;
            case SpiritType.ST_BLUE:
                blueSpirits = count;
                break;
            case SpiritType.ST_RED:
                redSpirits = count;
                break;
        }
    }

    public void ModifySpiritInventory(SpiritType type, int change)
    {
        switch (type)
        {
            case SpiritType.ST_GREEN:
                greenSpiritInv += change;
                break;
            case SpiritType.ST_BLUE:
                blueSpiritInv += change;
                break;
            case SpiritType.ST_RED:
                redSpiritInv += change;
                break;
        }
    }

    public void ModifyBlockInventory(BlockType type, int change)
    {
        switch (type)
        {
            case BlockType.BT_WOOD:
                woodBlockInv += change;
                break;
            case BlockType.BT_STONE:
                stoneBlockInv += change;
                break;
            case BlockType.BT_METAL:
                metalBlockInv += change;
                break;
        }
    }

    public int GetBlockInventory(BlockType type)
    {
        switch (type)
        {
            case BlockType.BT_WOOD:
                return woodBlockInv;
            case BlockType.BT_STONE:
                return stoneBlockInv;
            case BlockType.BT_METAL:
                return metalBlockInv;
            default:
                return 0;
        }
    }

    public int GetSpiritInventory(SpiritType type)
    {
        switch (type)
        {
            case SpiritType.ST_GREEN:
                return greenSpiritInv;
            case SpiritType.ST_BLUE:
                return blueSpiritInv;
            case SpiritType.ST_RED:
                return redSpiritInv;
            default:
                return 0;
        }
    }

    public int GetSpiritCount(SpiritType type)
    {
        switch (type)
        {
            case SpiritType.ST_GREEN:
                return greenSpirits;
            case SpiritType.ST_BLUE:
                return blueSpirits;
            case SpiritType.ST_RED:
                return redSpirits;
            default:
                return 0;
        }
    }

    public void AddTile(Tile tile)
    {
        TileInfo info = new TileInfo();

        info.x = tile.GetX();
        info.y = tile.GetY();
        info.type = tile.GetBlockType();

        tileSetInfo.Add(info);
    }

    public void RemoveTile(Tile tile)
    {
        TileInfo dummy = new TileInfo();

        dummy.x = tile.GetX();
        dummy.y = tile.GetY();
        dummy.type = tile.GetBlockType();

        tileSetInfo.Remove(dummy);
    }

    public int GetStars(string levelName)
    {
        return GetLevelState(levelName).stars;
    }

    public int GetPath(string levelName)
    {
        return GetLevelState(levelName).path;
    }

    public int GetScore(string levelName)
    {
        return GetLevelState(levelName).score;
    }

    public int GetCurrentCostume()
    {
        return costumeID;
    }

    public IEnumerator<TileInfo> GetTileList()
    {
        return tileSetInfo.GetEnumerator();
    }

    public void ClearTileList()
    {
        tileSetInfo.Clear();
    }

    LevelState GetLevelState(string levelName)
    {
        for (int j = 0; j < levelStates.Count; j++)
        {
            LevelState state = (LevelState)levelStates[j];

            if (state.name == levelName)
            {
                return state;
            }
        }

        LevelState newState = new LevelState(levelName);

        levelStates.Add(newState);

        return newState;
    }
    
}
