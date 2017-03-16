using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animation : MonoBehaviour {
    public Animator anim;
    private float prevEular;
    private float currEular;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        prevEular = transform.rotation.eulerAngles.y;
        
	}
	
	// Update is called once per frame
	void Update () {
        currEular = transform.rotation.eulerAngles.y;
        checkTurn();
        prevEular = currEular;
	}
    void checkTurn()
    {
        Debug.Log("In check turn");
     if (prevEular > currEular)
        {
            Debug.Log("In left turn");
            if (anim.GetBool("turnRight") == true)
            {
                anim.SetBool("turnRight", false);
            }
            anim.SetBool("turnLeft", true);
        } 
     else if (prevEular < currEular)
        {
            Debug.Log("In right turn");
            if (anim.GetBool("turnLeft") == true)
            {
                anim.SetBool("turnLeft", false);
            }
            anim.SetBool("turnRight", true);
        }
     else
        {
            anim.SetBool("turnRight", false);
            anim.SetBool("turnLeft", false);
        }  
    }
}
