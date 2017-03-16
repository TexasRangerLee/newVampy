using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UImanager : MonoBehaviour {
    public GameObject creditScreen;
    private GameObject canvas;
	// Use this for initialization
	void Start () {
        canvas = GameObject.FindGameObjectWithTag("MainCanvas");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Credits()
    {
        Debug.Log("In credits screen");
        GameObject cScreen = Instantiate(creditScreen,canvas.transform, false);
        cScreen.gameObject.transform.SetParent(canvas.transform);
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
