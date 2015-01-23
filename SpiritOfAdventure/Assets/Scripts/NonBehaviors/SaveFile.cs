using UnityEngine;
using System.Collections;
using System.Xml;

public class SaveFile
{
    public class LevelState
    {
        public string name;
        public int stars;
        public int path;
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

            levels.AppendChild(level);
        }

        doc.Save(Application.persistentDataPath + "/soa.xml");
    }

    public void SetLevelStars(string levelName, int stars)
    {
        LevelState state = GetLevelState(levelName);

        if (state == null)
        {
            state = new LevelState();

            state.name = levelName;
            state.stars = stars;
            state.path = 0;

            levelStates.Add(state);
        }
        else
        {
            state.stars = stars;
        }
    }

    public void SetLevelPath(string levelName, int path)
    {
        LevelState state = GetLevelState(levelName);

        if (state == null)
        {
            state = new LevelState();

            state.name = levelName;
            state.stars = 0;
            state.path = path;

            levelStates.Add(state);
        }
        else
        {
            state.path = path;
        }
    }

    public void SetCurrentCostume(int id)
    {
        costumeID = id;
    }

    public int GetStars(string levelName)
    {
        LevelState state = GetLevelState(levelName);

        if (state == null)
        {
            state = new LevelState();

            state.name = levelName;
            state.stars = 0;
            state.path = 0;

            levelStates.Add(state);
        }

        return state.stars;
    }

    public int GetPath(string levelName)
    {
        LevelState state = GetLevelState(levelName);

        if (state == null)
        {
            state = new LevelState();

            state.name = levelName;
            state.stars = 0;
            state.path = 0;

            levelStates.Add(state);
        }

        return state.path;
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

        return null;
    }
    
}
