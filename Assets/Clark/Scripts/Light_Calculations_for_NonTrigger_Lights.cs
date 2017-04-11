using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light_Calculations_for_NonTrigger_Lights : MonoBehaviour
{
    [SerializeField]
    GameObject Player;

    [SerializeField]
    Light LightSource;


    // Use this for initialization
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        if (this.GetComponentInChildren<Light>().tag == "LightSource")
            LightSource = GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    void Update()
    {

        BeforeRaycastCalculations();
    }

    public void BeforeRaycastCalculations()
    {
        Debug.Log("Inside the function to do some math!");
        Debug.DrawRay(LightSource.transform.position, (Player.transform.position - LightSource.transform.position).normalized * 10, Color.red);
        if (Vector3.Distance(Player.transform.position, LightSource.transform.position) <= LightSource.range)
        {
            Debug.Log("Inside the first If Statement!");
            if ((Mathf.Acos(Vector3.Dot(LightSource.transform.forward, (Player.transform.position - LightSource.transform.position).normalized))) * Mathf.Rad2Deg <= LightSource.spotAngle / 2)
            {
                //Do Raycasting
                Debug.Log("Player is in a nontrigger lightsource.");
            }
        }
    }
}
