using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

using itavio.Utilities;

public class itavioAssetPostProcessor : AssetPostprocessor {

    private const string k_pluginPathAndroid = "Assets/Plugins/Android/";

    private static readonly List<string> k_ignoredFileExtensions = new List<string>{ 
        ".meta",
        ".DS_Store"
    };

    private static readonly Dictionary<string, string> k_currentAndroidFiles = new Dictionary<string,string>{
        {"ItavioPlugin{0}.aar", ""},
        {"ItavioUnityPlugin{0}.jar", ""},
        {"android-async-http-{0}.jar", "1.4.8"},
        {"appcompat-v7-{0}.aar", "22.2.1"},
        {"library-{0}.aar", "2.1.4"},
        {"library-{0}.jar", "2.4.0"},
        {"MaterialDesign-{0}.aar", "1.5"},
        {"support-v4-{0}.aar", "22.2.1"}
    };

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        //Debug.Log("Itavio Asset Post Process!");
        
        List<string> files = new List<string>(Directory.GetFiles(k_pluginPathAndroid));

        foreach(string file in files)
        {
            if (ShouldCheckFile(file) && !FileIsSafe(file))
            {
                //Debug.Log("Should Check File: " + file);
                CheckFile(file);
            }
        }

        AssetDatabase.Refresh();
    }

    static bool ShouldCheckFile(string filename)
    {
        foreach (string ignoredExtension in k_ignoredFileExtensions)
        {
            if (filename.ToLower().EndsWith(ignoredExtension.ToLower()))
            {
                return false;
            }
        }

        return true;
    }

    static bool FileIsSafe(string filename)
    {
        if (filename.StartsWith(k_pluginPathAndroid))
        {
            filename = filename.Remove(0, k_pluginPathAndroid.Length);

            foreach(KeyValuePair<string, string> fileDef in k_currentAndroidFiles)
            {
                if (filename.ToLower().Equals(string.Format(fileDef.Key, fileDef.Value).ToLower()))
                {
                    return true;
                }
            }

            foreach (KeyValuePair<string, string> fileDef in k_currentAndroidFiles)
            {
                string fileNameWithoutVersion = fileDef.Key.Substring(0, fileDef.Key.IndexOf('{'));
                if (filename.ToLower().StartsWith(fileNameWithoutVersion.ToLower()))
                {
                    return false;
                }
            }

        }
        //itavioDbg.Log("File was not identified: " + filename);
        return true;
    }

    static void CheckFile(string filename)
    {
        if (filename.StartsWith(k_pluginPathAndroid))
        {
            filename = filename.Remove(0, k_pluginPathAndroid.Length);
        }

        foreach (KeyValuePair<string, string> fileDef in k_currentAndroidFiles)
        {
            string fileNameWithoutVersion = fileDef.Key.Substring(0, fileDef.Key.IndexOf('{'));
            string fileExtension = fileDef.Key.Substring(fileDef.Key.LastIndexOf('.'));

            string lFilename = filename.ToLower();

            if (lFilename.StartsWith(fileNameWithoutVersion.ToLower()) && lFilename.EndsWith(fileExtension.ToLower()))
            {
                string fileVersionString = filename.Remove(0, fileNameWithoutVersion.Length);
                fileVersionString = fileVersionString.Substring(0, fileVersionString.LastIndexOf('.'));

                Version fileVersion = new Version(fileVersionString);
                Version minVersion = new Version(fileDef.Value);

                if (fileVersion > minVersion)
                {
                    DeleteFile(k_pluginPathAndroid + string.Format(fileDef.Key, fileDef.Value));
                }
                else
                {
                    DeleteFile(k_pluginPathAndroid + string.Format(fileDef.Key, fileVersionString));
                }
            }
        }
    }

    static void DeleteFile(string filename)
    {
        if (File.Exists(filename))
        {
            if (FileUtil.DeleteFileOrDirectory(filename))
            {
                itavioDbg.LogWarning("Deleted <b><color=red>" + filename + "</color></b> because a newer version was found.");
            }
        }
    }
}
