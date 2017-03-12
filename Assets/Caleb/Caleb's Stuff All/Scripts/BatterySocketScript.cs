using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterySocketScript : MonoBehaviour
{

    public bool isPowered;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //if a battery object is colliding with the interior zone of a battery socket,
    //the battery socket is considered powered NEEDS RIGIDBODY ON ONE OR THE OTHER TO WORK
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Battery"))
        {
            isPowered = true;
        }
    }

    //if a battery object leaves the interior zone of a battery socket, it loses power
    //NEEDS RIGIDBODY ON ONE OR THE OTHER TO WORK
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Battery"))
        {
            isPowered = false;
        }
    }
}
