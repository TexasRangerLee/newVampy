using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerToggleScript : MonoBehaviour {
    public Material powered;
    public Material unpowered;
    public Renderer rend;
    public bool isPowered;

	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    //if cable is unpowered, change to powered;
    //if cable is powered, change to unpowered
    //call this whenever a power/cable system toggles
    public void changePowerState()
    {
        if (isPowered)
        {
            isPowered = false;
            rend.material = unpowered;
        }
        else
        {
            isPowered = true;
            rend.material = powered;
        }
    }
}
