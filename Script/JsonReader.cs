using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static SaveData;

namespace SimpleJSON
{

    public class JsonReader : MonoBehaviour
    {
        List<Vector3> path;
        public GameObject linePrefab;
        GameObject line;
        public GameObject route1;
        public GameObject route2;
        public GameObject route3;
        public GameObject route4;
        public GameObject spot;

        // Start is called before the first frame update
        void Start()
        {
            RecordList loadData = new RecordList
            {
                recordList = new List<Record>()
            };

            //讀取json檔案並轉存成文字格式
            StreamReader file = new StreamReader(System.IO.Path.Combine(Application.streamingAssetsPath, "formal_record.json"));
            string loadJson = file.ReadToEnd();
            file.Close();

            //使用JsonUtillty的FromJson方法將存文字轉成Json
            loadData = JsonUtility.FromJson<RecordList>(loadJson);
            
            foreach (Record r in loadData.recordList)
            {
                line = Instantiate(linePrefab);
                Renderer rend = line.GetComponent<Renderer>();

                // 在名稱顯示ID以及移動距離
                line.name = "ID: " + r.ID + ", " + r.condition + "," + r.distanceMoved + ", isTesting: " + r.isTesting;
                LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
                switch (r.condition)
                {
                    case "A":
                        rend.material.SetColor("_Color", Color.green);
                        break;
                    case "B":
                        rend.material.SetColor("_Color", Color.blue);
                        break;
                    case "C":
                        rend.material.SetColor("_Color", Color.yellow);
                        break;
                    case "D":
                        rend.material.SetColor("_Color", Color.magenta);
                        break;
                }

                // 設定線段的位置
                lineRenderer.positionCount = r.path.Count;
                for (int i = 0; i < r.path.Count; i++)
                {
                    Vector3 pos = r.path[i];
                    pos.y = 2f;
                    lineRenderer.SetPosition(i, pos);
                }

                switch (r.endIndex)
                {
                    case 1:
                        line.transform.SetParent(route1.transform);
                        break;
                    case 2:
                        line.transform.SetParent(route2.transform);
                        break;
                    case 3:
                        line.transform.SetParent(route3.transform);
                        break;
                    case 4:
                        line.transform.SetParent(route4.transform);
                        break;

                }

                // 顯示Log的發生位置
                Debug.Log(r.pressLog.Count);
                GameObject pressLog = new GameObject("PressLog");
                for (int i = 0; i < r.pressLog.Count; i++)
                {
                    PressLog log = r.pressLog[i];
                    GameObject s = Instantiate(spot, new Vector3(log.pos.x, 5f, log.pos.z), Quaternion.Euler(90, 0, 0));
                    s.name = "ID: " + r.ID +"," + log.describe;
                    s.transform.SetParent(pressLog.transform);
                }
                pressLog.transform.SetParent(line.transform);
            }
        }


        // Update is called once per frame
        void Update()
        {

        }
    }

}