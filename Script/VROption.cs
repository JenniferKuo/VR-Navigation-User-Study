using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class VROption : MonoBehaviour
{
    static public string ID = "0";
    static public int startPoint = 1;
    static public int endPoint = 0;
    static public Vector3 endPos;
    static public bool isTesting = false;
    static public int order;
    static public string condition;
    static public int finish_idx = 0;

    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setID(string ID)
    {
        VROption.ID = ID;
    }

    public void startPointValue(Dropdown change)
    {
        int index = change.value;
        startPoint = Int32.Parse(change.options[index].text);
    }

    public void endPointValue(Dropdown change)
    {
        int index = change.value;
        endPoint = Int32.Parse(change.options[index].text);
    }

    public void orderValue(Dropdown change)
    {
        int index = change.value;
        order = Int32.Parse(change.options[index].text);
    }

    public void conditionValue(Dropdown change)
    {
        int index = change.value;
        string c = change.options[index].text;
        VROption.condition = c;
    }

    public void nextStage()
    {
        if (finish_idx == 8)
        {
            SceneManager.LoadScene("Option");
            return;
        }
        Debug.Log("idx:" + finish_idx);
        setOption();
        finish_idx++;
        SceneManager.LoadScene("Main");
    }

    public void preTest()
    {
        SceneManager.LoadScene("Main");
        VROption.isTesting = true;
        VROption.startPoint = 1;
        VROption.endPoint = 0;
        VROption.condition = "A";
    }

    public void preTest2()
    {
        SceneManager.LoadScene("Main");
        VROption.isTesting = true;
        VROption.startPoint = 2;
        VROption.endPoint = 0;
        VROption.condition = "A";
    }

    public void playground()
    {
        SceneManager.LoadScene("Playground");
        VRController.isWalkable = true;
    }

    public void setOption()
    {
        ReadConfig.ReadFile();
        VROption.ID = ReadConfig.user_config[finish_idx].ID;
        VROption.order = ReadConfig.user_config[finish_idx].order;
        VROption.startPoint = ReadConfig.user_config[finish_idx].startPoint;
        VROption.endPoint = ReadConfig.user_config[finish_idx].endPoint;
        VROption.condition = ReadConfig.user_config[finish_idx].condition;
        VROption.isTesting = ReadConfig.user_config[finish_idx].isTesting;
    }
}
