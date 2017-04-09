using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterySocketScript : MonoBehaviour
{

    public bool isPowered;
    GameObject wireController;
    public WireControllerScript script;

    // Use this for initialization
    void Start()
    {
        wireController = this.transform.parent.parent.gameObject;
        script = wireController.GetComponent<WireControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //if a battery object is colliding with the interior zone of a battery socket,
    //the battery socket is considered powered NEEDS RIGIDBODY ON ONE OR THE OTHER TO WORK
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Battery"))
        {
            isPowered = true;
            script.evaluateStatus();
        }
    }

    //if a battery object leaves the interior zone of a battery socket, it loses power
    //NEEDS RIGIDBODY ON ONE OR THE OTHER TO WORK
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Battery"))
        {
            isPowered = false;
            script.evaluateStatus();
        }
    }
}
