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
    public PoweredDoorScript script;
    //insert script for light script once finalized

    // Use this for initialization
    void Start()
    {
        rend = this.GetComponent<Renderer>();
        rend.enabled = true;

        hasTarget = (target != null);
        if (hasTarget)
        {
            script = target.GetComponent<PoweredDoorScript>();
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
        script.MoveDoor();
        //script.doorHasPower = !script.doorHasPower;
    }

    public void RotateLight()
    {
        //TODO
    }

    public void ToggleLight()
    {
        //TODO
    }

}
