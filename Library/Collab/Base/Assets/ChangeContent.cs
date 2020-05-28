using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeContent : MonoBehaviour {

    public GameObject button;
    public GameObject MapView;
    public Camera mapCamera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.M))
        {
            openMap();
            Debug.Log("open map");
        }
    }

    public void openMap()
    {
        if (MapView.activeSelf)
        {
            MapView.SetActive(false);
            button.GetComponentInChildren<Text>().text = "Show Map";
        }
        else
        {
            MapView.SetActive(true);
            button.GetComponentInChildren<Text>().text = "Show AR";
        }
    }

    public void moveMap()
    {

    }
}
