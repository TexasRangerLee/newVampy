using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Drone")
        {
            DronePatrol temp = other.GetComponent<DronePatrol>();
            if (temp.Target == transform.position)
            {
                temp.ReachedPoint();
            }
        }
    }
  
}
