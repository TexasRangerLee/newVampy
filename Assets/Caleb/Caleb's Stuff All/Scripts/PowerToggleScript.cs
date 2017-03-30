using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerToggleScript : MonoBehaviour
{
    //references for color change
    public Material powered;
    public Material unpowered;
    public Renderer rend;

    //variables to handle powered objects
    public bool hasPower;
    public bool hasTarget; //MUST BE SET IN EDITOR
    public GameObject target;
    //different bools depending on object and desired outcome
    //MUST BE SET IN EDITOR
    public bool isDoor;
    public bool turnLight;
    public bool toggleLight;
    public PoweredDoorScript doorScript;

    public GameObject lightIndicator;
    public PowerToggleScript indicatorScript;

    // Use this for initialization
    void Start()
    {
        rend = this.GetComponent<Renderer>();
        rend.enabled = true;

        hasTarget = (target != null);
        if (hasTarget&&isDoor)
        {
            doorScript = target.GetComponent<PoweredDoorScript>();
        }

        if (hasTarget && toggleLight) //if a wire has a toggle light, sets its state to the wire's
        {                             //state on start
            target.SetActive(hasPower);
            indicatorScript = lightIndicator.GetComponent<PowerToggleScript>();
            indicatorScript.changePowerState(hasPower);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    //if cable is unpowered, change to powered;
    //if cable is powered, change to unpowered
    //call this whenever a power/cable system toggles
    public void changePowerState(bool isPowered)
    {
        if (!isPowered) //because i'm sick and fuck writing this hyper legibly
        {
            rend.material = unpowered;
            hasPower = false;

        }
        else
        {
            rend.material = powered;
            hasPower = true;
        }
        if (hasTarget)
        {
            HandleTarget();
        }
    }

    public void HandleTarget()
    {
        if (isDoor)
        {
            ToggleDoor();
        }
        else if (turnLight)
        {
            RotateLight();
        }
        else if (toggleLight)
        {
            ToggleLight();
        }
        else
        {
        }
    }

    public void ToggleDoor()
    {
        doorScript.MoveDoor();
    }

    public void RotateLight()
    {
        //TODO, might not do?
    }

    public void ToggleLight()
    {
        target.SetActive(hasPower);
        indicatorScript.changePowerState(this.hasPower);
    }

}
