using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReadConfig;

[Serializable]
public class OptionList
{
    public List<OptionItem> optionList;
}

[Serializable]
public class OptionItem
{
    public string ID;
    public int startPoint;
    public int endPoint;
    public bool isTesting ;
    public int order;
    public string condition;
}

public class ReadConfig : MonoBehaviour
{
    public static List<OptionItem> user_config = new List<OptionItem>();

    public static void ReadFile()
    {
        // 先讀檔
        StreamReader file = new StreamReader(System.IO.Path.Combine(Application.streamingAssetsPath, "config.json"));
        OptionList loadData = JsonUtility.FromJson<OptionList>(file.ReadToEnd());
        file.Close();

        int user_idx = int.Parse(VROption.ID) - 1;

        for(int i = user_idx * 8; i < user_idx * 8 + 8; i++)
        {
            user_config.Add(loadData.optionList[i]);
        }
    }
}