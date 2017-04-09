using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructorScript : MonoBehaviour
{

    public bool isDone;
    public GameObject[] destroy;
    public GameObject[] enable;
    public GameObject[] powerSystems;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && isDone == false)
        {
            LevelCleanup();
            isDone = true;
        }
    }

    void LevelCleanup()
    {
        foreach (GameObject obj in destroy)
        {
            Destroy(obj);
        }
        foreach(GameObject obj in powerSystems)
        {
            obj.GetComponent<BatterySocketScript>().isPowered = false;
            obj.GetComponent<BatterySocketScript>().script.evaluateStatus();
        }
    }
}
