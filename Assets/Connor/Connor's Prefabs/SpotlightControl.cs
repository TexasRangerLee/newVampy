using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightControl : MonoBehaviour {
    private int direction = 0;
    public int turnSpeed;
    public bool alerted = false;
    private Vector3 temp;
	// Use this for initialization
	void Start () {
        
        temp = new Vector3(32, 0, 0);
	}

    // Update is called once per frame
    void Update() {

        if (direction == 1)
        {
            TurnLeft();
        }
        else if (direction == 2)
        {
            TurnRight();
        }

        else if (direction == 0)
        {
            transform.localEulerAngles = new Vector3(32.1f, 0, 0);
            
        }
	}

    void TurnLeft()
    {

        temp.y -= turnSpeed * Time.deltaTime;
        transform.localEulerAngles = temp;
        if (temp.y <= -30)
        {
            direction = 2;
        }
    }

    void TurnRight()
    {
        
        temp.y += turnSpeed * Time.deltaTime;
        transform.localEulerAngles = temp;
        if (temp.y >= 30)
        {
            direction = 1;
        }
    }
}
