using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour {
    private Text tex;
    private Color baseC;
    public Color hoverC;
	// Use this for initialization
	void Start () {
        tex = GetComponentInChildren<Text>();
        baseC = tex.color;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseOver()
    {
        //tex.GetComponent<Font>(). = hoverC;
        //tex.color = hoverC; 
    }

    
}
