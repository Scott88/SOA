using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class HintList : MonoBehaviour
{
    public TextAsset hintFile;

    private bool created;

    private XmlDocument hintDoc;

    private List<string> hints;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }

        hintDoc = new XmlDocument();
        hints = new List<string>();

        hintDoc.LoadXml(hintFile.text);

        XmlElement nextHint = hintDoc["Hints"]["Hint"];

        while (nextHint != null)
        {
            hints.Add(nextHint.InnerText);
            nextHint = (XmlElement)nextHint.NextSibling;
        }
    }

    public string GetRandomHint()
    {
        return hints[Random.Range(0, hints.Count)];
    }
}
