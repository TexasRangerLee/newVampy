using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronePatrol : MonoBehaviour {
    public Transform[] points;
    public float speed;
    private Vector3 target;
    private int count = 0;
    public enum state { Patrol, Alert};
    public state currentState;
    

    private bool alerted = false;
    SpotlightControl spotlight;

    public int Count
    {
        get
        {
            return count;
        }

        set
        {
            count = value;
        }
    }

    public Vector3 Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
        }
    }

    // Use this for initialization
    void Start () {
        currentState = state.Patrol;
        spotlight = GetComponentInChildren<SpotlightControl>();
        FindNewTarget();
	}
	
	// Update is called once per frame
	void Update () {

        if (currentState == state.Patrol)
        {
            patrolling();
        }
        else if (currentState == state.Patrol)
        {
            alert();
        }

       
    }

    private void patrolling()
    {
        Quaternion lookRo = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRo, 20 * Time.deltaTime);
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    private void alert()
    {

    }

    public void ReachedPoint()
    {
        FindNewTarget();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(Target, 1);
    }

    void FindNewTarget()
    {

        Target = points[count].position;

        if (Count + 1 == points.Length)
        {
            Count = 0;
        }
        else Count++;
    }
}
