using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UImanager : MonoBehaviour {
    public GameObject creditScreen;
    private GameObject canvas;
    public GameObject OScreen;
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

    public void NewGame()
    {
        Debug.Log("Starting new game.");
        SceneManager.LoadScene("fuckyouunity");
    }

    public void Options()
    {
        GameObject OpScreen = Instantiate(OScreen, canvas.transform, false);
        OpScreen.gameObject.transform.SetParent(canvas.transform);
    }
}
