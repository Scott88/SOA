using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;

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
    private List<TileInfo> tilesToRemove;

    private int costumeID;

    private int greenSpirits, blueSpirits, redSpirits;

    private int greenSpiritInv, blueSpiritInv, redSpiritInv;

    private int woodBlockInv, stoneBlockInv, metalBlockInv;

    private int stars;

    private bool showAds;

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

            try
            {
                saveFile.Load(Application.persistentDataPath + "/soa.xml");
            }
            catch (XmlException)
            {
                try
                {
                    saveFile.LoadXml(Decrypt(File.ReadAllText(Application.persistentDataPath + "/soa.xml")));
                }
                catch (XmlException)
                {
                    File.Delete(Application.persistentDataPath + "/soa.xml");
                    saveFile = TransferFromPrefs();
                }
            }

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

#if UNITY_EDITOR
        doc.Save(Application.persistentDataPath + "/soa.xml");
#else
        File.WriteAllText(Application.persistentDataPath + "/soa.xml", Encrypt(doc.OuterXml));
#endif

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
        tilesToRemove = new List<TileInfo>();

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
        else
        {
            greenSpiritInv = 5;
            blueSpiritInv = 5;
            redSpiritInv = 5;
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
            woodBlockInv = 5;
            stoneBlockInv = 5;
            metalBlockInv = 5;
        }

        XmlElement starsNode = saveNode["Stars"];

        if (starsNode != null)
        {
            stars = int.Parse(starsNode.GetAttribute("count"));
        }
        else
        {
            stars = 0;

            for (int j = 0; j < levelStates.Count; j++)
            {
                stars += levelStates[j].stars;
            }
        }

        XmlElement adsNode = saveNode["ShowAds"];

        if (adsNode != null)
        {
            showAds = bool.Parse(adsNode.GetAttribute("value"));
        }
        else
        {
            showAds = true;
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

        XmlElement starsNode = doc.CreateElement("Stars");
        save.AppendChild(starsNode);

        XmlAttribute starsCount = doc.CreateAttribute("count");
        starsCount.Value = stars.ToString();

        starsNode.Attributes.Append(starsCount);

        XmlElement adsNode = doc.CreateElement("ShowAds");
        save.AppendChild(adsNode);

        XmlAttribute showAdsValue = doc.CreateAttribute("value");
        showAdsValue.Value = showAds.ToString();

        adsNode.Attributes.Append(showAdsValue);

#if UNITY_EDITOR
        doc.Save(Application.persistentDataPath + "/soa.xml");
#else
        File.WriteAllText(Application.persistentDataPath + "/soa.xml", Encrypt(doc.OuterXml));
#endif
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

    public void SetSpiritInventory(SpiritType type, int count)
    {
        switch (type)
        {
            case SpiritType.ST_GREEN:
                greenSpiritInv = count;
                break;
            case SpiritType.ST_BLUE:
                blueSpiritInv = count;
                break;
            case SpiritType.ST_RED:
                redSpiritInv = count;
                break;
        }
    }

    public void ModifyBlockInventory(BlockType type, int change)
    {
        switch (type)
        {
            case BlockType.BT_WOOD:
                woodBlockInv += change;
                if (woodBlockInv < 5) woodBlockInv = 5;
                break;
            case BlockType.BT_STONE:
                stoneBlockInv += change;
                break;
            case BlockType.BT_METAL:
                metalBlockInv += change;
                break;
        }
    }

    public void SetBlockInventory(BlockType type, int count)
    {
        switch (type)
        {
            case BlockType.BT_WOOD:
                woodBlockInv = count;
                break;
            case BlockType.BT_STONE:
                stoneBlockInv = count;
                break;
            case BlockType.BT_METAL:
                metalBlockInv = count;
                break;
        }
    }

    public void ModifyStars(int change)
    {
        stars += change;
    }

    public int GetStars()
    {
        return stars;
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

    public bool ShowAds()
    {
        return showAds;
    }

    public void DisableAds()
    {
        showAds = false;
    }

    public void AddTile(Tile tile)
    {
        TileInfo info = new TileInfo();

        info.x = tile.GetX();
        info.y = tile.GetY();
        info.type = tile.GetBlockType();

        if (tileSetInfo.Contains(info))
        {
            tileSetInfo.Remove(info);
        }

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

    public void RemoveLater(int x, int y)
    {
        TileInfo info = new TileInfo();

        info.x = x;
        info.y = y;

        tilesToRemove.Add(info);
    }

    public void ExecuteRemovals()
    {
        for (int j = 0; j < tilesToRemove.Count; j++)
        {
            tileSetInfo.Remove(tilesToRemove[j]);
        }

        tilesToRemove.Clear();
    }

    public int GetLevelStars(string levelName)
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

    string Encrypt(string toEncrypt)
    {
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes("53120246197329465012435945700311");
        // 256-AES key
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        // http://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode.aspx
        rDel.Padding = PaddingMode.PKCS7;
        // better lang support
        ICryptoTransform cTransform = rDel.CreateEncryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    string Decrypt(string toDecrypt)
    {
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes("53120246197329465012435945700311");
        // AES-256 key
        byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        // http://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode.aspx
        rDel.Padding = PaddingMode.PKCS7;
        // better lang support
        ICryptoTransform cTransform = rDel.CreateDecryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return UTF8Encoding.UTF8.GetString(resultArray);
    }

}
