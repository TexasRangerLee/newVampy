using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireControllerScript : MonoBehaviour
{
    public int numKids;
    public bool systemActive;
    public GameObject child;

    public GameObject wires;

    public PowerToggleScript[] wireScripts;

    // Use this for initialization
    void Start()
    {
        numKids = this.transform.childCount;
        wires = this.transform.GetChild(numKids - 1).gameObject;
        wireScripts = wires.GetComponentsInChildren<PowerToggleScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void evaluateStatus()
    {
        getBatteryStatus();
        handleWires(systemActive);
    }

    public void getBatteryStatus()
    {
        for (int i = 0; i <= numKids - 2; i += 1)
        {
            systemActive = false;
            child = transform.GetChild(i).gameObject;
            if (child.transform.GetComponentInChildren<BatterySocketScript>().isPowered == true)
            {
                systemActive = true;
                break;
            }
        }
    }

    public void handleWires(bool powered)
    {
        foreach (PowerToggleScript script in wireScripts)
        {
            script.changePowerState(powered);
        }
    }
}
