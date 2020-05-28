using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static SaveData;

[Serializable]
public class RecordList
{
    public List<Record> recordList;
}

public class SaveData : MonoBehaviour
{
    public static void SaveFile(Record r)
    {
        // 先讀檔
        StreamReader file = new StreamReader(System.IO.Path.Combine(Application.streamingAssetsPath, "formal_record.json"));
        RecordList loadData = JsonUtility.FromJson<RecordList>(file.ReadToEnd());
        file.Close();
        
        loadData.recordList.Add(r);
        // 將屬性轉成string(json格式)
        string saveString = JsonUtility.ToJson(loadData, true);
        Debug.Log(saveString);
        // 將字串saveString存到硬碟中
        StreamWriter f = new StreamWriter(System.IO.Path.Combine(Application.streamingAssetsPath, "formal_record.json"), append: false);
        f.WriteLine(saveString);
        f.Close();
    }


    [Serializable]
    public class Record
    {
        public string ID;
        public float taskTime;
        public int pickUpNumber;
        public float phoneActiveTime;
        public int startIndex = VROption.startPoint;
        public int endIndex = VROption.endPoint;
        public string condition = VROption.condition;
        public int route = VROption.endPoint;
        public bool isTesting;
        public float distanceMoved;
        public int order = VROption.order;
        public List<Vector3> path;
        public List<PressLog> pressLog;
        public float angleDiff;
        public float distanceDiff;
    }

    [Serializable]
    public class PressLog
    {
        public Vector3 pos;
        public float timeStamp;
        public bool isMap;
        public string describe;
    }
}