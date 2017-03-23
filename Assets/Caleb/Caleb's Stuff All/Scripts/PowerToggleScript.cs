using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerToggleScript : MonoBehaviour {
    public Material powered;
    public Material unpowered;
    public Renderer rend;

	// Use this for initialization
	void Start () {
        rend = this.GetComponent<Renderer>();
        rend.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
	}

    //if cable is unpowered, change to powered;
    //if cable is powered, change to unpowered
    //call this whenever a power/cable system toggles
    public void changePowerState(bool isPowered)
    {
        if (!isPowered) //because i'm sick and fuck writing this hyper legibly
        {
            rend.material = unpowered;
        }
        else
        {
            rend.material = powered;
        }
    }
}
