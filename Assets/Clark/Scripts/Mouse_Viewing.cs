﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse_Viewing : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        float normalizedMouseX = Input.mousePosition.x / Screen.width;
        float normalizedMouseY = Input.mousePosition.y / Screen.height;

        transform.localEulerAngles = new Vector3((-75 * (normalizedMouseY - 0.5f)), 0.0f, 0.0f);
        //this.GetComponentInParent<Rigidbody>().transform.localEulerAngles = new Vector3(0.0f, -(-75 * (normalizedMouseX - 0.5f)), 0.0f);
        //this.transform.parent.transform.localEulerAngles = this.transform.localEulerAngles;

    }
}
