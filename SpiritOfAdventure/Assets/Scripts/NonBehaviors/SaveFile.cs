using UnityEngine;
using System.Collections;
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

    private ArrayList levelStates;

    private int costumeID;

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

        levelStates = new ArrayList();

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
            LevelState state = (LevelState)levelStates[j];

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

        doc.Save(Application.persistentDataPath + "/soa.xml");
    }

    public void SetLevelStars(string levelName, int stars)
    {
        GetLevelState(levelName).stars = stars;
        SaveToXML();
    }

    public void SetLevelPath(string levelName, int path)
    {
        GetLevelState(levelName).path = path;
        SaveToXML();
    }

    public void SetLevelScore(string levelName, int score)
    {
        GetLevelState(levelName).score = score;
        SaveToXML();
    }

    public void SetCurrentCostume(int id)
    {
        costumeID = id;
        SaveToXML();
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
