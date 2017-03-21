using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireControllerScript : MonoBehaviour
{

    public int numKids;
    public bool systemActive;
    public GameObject child;

    public GameObject wires;
    public Material unpoweredWire;
    Color white;

    // Use this for initialization
    void Start()
    {
        numKids = this.transform.childCount;
        wires = this.transform.GetChild(numKids - 1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void evaluateStatus()
    {
        getBatteryStatus();

    }

    public void getBatteryStatus()
    {
        for (int i = 0; i <= numKids - 2; i -= 1)
        {
            child = transform.GetChild(i).gameObject;
            if (child.transform.GetComponentInChildren<BatterySocketScript>().enabled)
            {
                systemActive = true;
                break;
            }
            systemActive = false;
        }
    }

    public void handleWires(bool powered)
    {
        if (powered)
        {
            unpoweredWire.EnableKeyword("_EMISSION");
            unpoweredWire.SetColor("_EmissionColor", Color.white);
        }
        else
        {
            unpoweredWire.EnableKeyword("_EMISSION");
            unpoweredWire.SetColor("_EmissionColor", Color.black);
        }
    }
}
