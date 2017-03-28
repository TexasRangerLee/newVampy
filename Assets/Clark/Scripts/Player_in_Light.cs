using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_in_Light : MonoBehaviour 
{
    [SerializeField]
    LayerMask obstruction;

    [SerializeField]
    GameObject Player;

	// Use this for initialization
	void Start () 
    {

	}
	
	// Update is called once per frame
	void Update () 
    {
        //Debug.Log(Player.GetComponent<Vamp_Player_Controller>().inLightTrigger + " from Light");

        try
        {
            if (Player.GetComponent<Vamp_Player_Controller>().inLightTrigger)
            {
                CanIHurtPlayer();
            }
        }
        catch (System.Exception)
        {
            Debug.Log("Not in a trigger!!!");
        }
	}

    public void OnTriggerStay(Collider other)
    {
        if ((Time.time % 1.0) == 0)
        {
            if (other.gameObject.tag == "Player")
            {
                Ray inLight = new Ray(this.transform.position, -((this.transform.position - other.gameObject.transform.position) / Vector3.Distance(this.transform.position, other.gameObject.transform.position)));
                Debug.DrawLine(this.transform.position, other.gameObject.transform.position, Color.green);
                Debug.DrawRay(this.transform.position, -((this.transform.position - other.gameObject.transform.position) / Vector3.Distance(this.transform.position, other.gameObject.transform.position)), Color.red);
                RaycastHit lightCheck;
                //LayerMask obstruction = LayerMask.GetMask("Obstruction");
                if (Physics.Raycast(inLight, out lightCheck, obstruction))
                {
                    Debug.Log(lightCheck.transform.gameObject.tag);
                    if (lightCheck.transform.gameObject.tag != "Player")
                    {
                        Debug.Log("Light is being blocked!!!");
                    }
                    else
                    {
                        Debug.Log("OI HE IN THE LIGHT!!!!");
                    }
                    //Call Some Function to be written in about 5 minutes
                }
            } 
        }
    }

    public void CanIHurtPlayer()
    {
        Ray inLight = new Ray(this.transform.position, -((this.transform.position - Player.gameObject.transform.position) / Vector3.Distance(this.transform.position, Player.gameObject.transform.position)));
        Debug.DrawLine(this.transform.position, Player.gameObject.transform.position, Color.green);
        Debug.DrawRay(this.transform.position, -((this.transform.position - Player.gameObject.transform.position) / Vector3.Distance(this.transform.position, Player.gameObject.transform.position)), Color.red);
        RaycastHit lightCheck;
        //LayerMask obstruction = LayerMask.GetMask("Obstruction");
        if (Physics.Raycast(inLight, out lightCheck, obstruction))
        {
            Debug.Log(lightCheck.transform.gameObject.tag);
            if (lightCheck.transform.gameObject.tag != "Player")
            {
                Debug.Log("Light is being blocked!!!");
            }
            else
            {
                Debug.Log("OI HE IN THE LIGHT!!!!");
            }
            //Call Some Function to be written in about 5 minutes
        }
    }

}
