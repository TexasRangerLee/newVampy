using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class PressStart : MonoBehaviour {
    private Button but;
    private Text tex;
    public AudioSource click;
    public GameObject[] secButtons;
	// Use this for initialization
	void Start () {
        //secButtons = GameObject.FindGameObjectsWithTag("2ndButton");
        but = GetComponent<Button>();
        tex = GetComponentInChildren<Text>();
        click = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Pressed()
    {
        StartCoroutine(StartPressed());
        click.Play();
    }

    
    IEnumerator StartPressed()
    {
        but.interactable = false;
        but.GetComponent<Image>().CrossFadeAlpha(0.0f, 1.0f, false);
        tex.CrossFadeAlpha(0f, 1f, false);
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < secButtons.Length; i++)
        {

            secButtons[i].SetActive(true);
        }
        yield return null;
    }
}
