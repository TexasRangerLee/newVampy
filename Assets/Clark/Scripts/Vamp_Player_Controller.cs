﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vamp_Player_Controller : MonoBehaviour
{
    enum States { Idle, Walking, Running, Jumping };

    public float walkingSpeed;
    public float runningSpeed;
    public float jumpingHeight;
    public bool inLightTrigger;

    [SerializeField]
    States currentState;

    [SerializeField]
    bool grounded;

    [SerializeField]
    bool holdingSomething = false;

    [SerializeField]
    bool canDropObject = false;

    [SerializeField]
    LayerMask obstruction; 

    Rigidbody rb;
    Vector3 maxHeight;
    Vector3 groundedPosition;
    bool falling;

    // Use this for initialization
    void Start()
    {
        currentState = States.Idle;
        rb = this.GetComponent<Rigidbody>();
        falling = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        float forwardMovement = Input.GetAxis("Vertical");
        float strafing = Input.GetAxis("Horizontal");

        try
        {
            RaycastHit interact;
            Debug.DrawRay(this.gameObject.transform.GetChild(0).transform.position, this.gameObject.transform.GetChild(0).transform.forward * 2.5f);
            if (Physics.Raycast(this.transform.position, this.transform.GetChild(0).transform.forward, out interact, 2.5f))
            {
                if (interact.transform.gameObject != null)
                {
                    if (interact.transform.gameObject.tag == "Battery")
                    {
                        if (Input.GetKeyUp(KeyCode.E))
                        {
                            if (interact.transform.gameObject.GetComponent<Rigidbody>().useGravity)
                            {
                                interact.transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
                                interact.transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                                interact.transform.gameObject.transform.parent = this.transform;
                                interact.transform.gameObject.transform.position = this.transform.TransformPoint(0, 0, 2.5f);
                                interact.transform.gameObject.transform.rotation = Quaternion.EulerAngles(0, 90, 0);
                                holdingSomething = true;
                                canDropObject = false;
                                StartCoroutine("CanDrop");
                            }
                        }
                    }

                    interact.transform.gameObject.GetComponent<GlowObject>().enabled = true;
                    if (Input.GetKeyUp(KeyCode.E))
                    {
                        interact.transform.gameObject.GetComponent<DoorOpenCloseLerpScript>().MoveDoor();
                    }
                }
                interact.transform.gameObject.GetComponent<GlowObject>().enabled = false;
            }
        }
        catch
        {

        }

        if (canDropObject)
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                if (holdingSomething)
                {
                    this.transform.GetChild(1).transform.gameObject.GetComponent<Rigidbody>().useGravity = true;
                    this.transform.GetChild(1).transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    this.transform.GetChild(1).transform.gameObject.transform.localScale.Set(1, 1, 1);
                    this.transform.GetChild(1).transform.gameObject.transform.parent = null;
                    holdingSomething = false;
                    canDropObject = false;
                }
            }
        }


        if (Input.GetKey(KeyCode.LeftShift))
        {
            ChangeStates(States.Running);
        }

        switch (currentState)
        {
            case States.Idle:
                {
                    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                    {
                        ChangeStates(States.Walking);
                    }

                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (grounded)
                        {
                            rb.velocity = new Vector3(0.0f, jumpingHeight, 0.0f);
                        }
                    }
                    break;
                }
            case States.Walking:
                {
                    Vector3 forward = Vector3.zero;
                    Vector3 straiffing = Vector3.zero;

                    //Z is the foward demention, must be remembered for when level design takes place

                    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
                    {
                        forward = transform.forward * forwardMovement;
                    }
                    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    {
                        straiffing = transform.right * strafing;
                    }

                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (grounded)
                        {
                            rb.velocity = new Vector3(0.0f, jumpingHeight, 0.0f);
                        }
                    }

                    Vector3 walking = forward + straiffing;
                    walking = walking.normalized * walkingSpeed * Time.deltaTime;
                    rb.MovePosition(transform.position + walking);

                    break;
                }
            case States.Running:
                {
                    Vector3 running = Vector3.zero;
                    Vector3 forward = Vector3.zero;
                    Vector3 straiffing = Vector3.zero;

                    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
                    {
                        forward = transform.forward * forwardMovement;
                    }
                    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    {
                        straiffing = transform.right * strafing;
                    }

                    running = forward + straiffing;
                    running = running.normalized * runningSpeed * Time.deltaTime;
                    rb.MovePosition(transform.position + running);

                    if (Input.GetKeyUp(KeyCode.LeftShift))
                    {
                        ChangeStates(States.Walking);
                    }
                    if ((Mathf.Abs(forwardMovement) <= 0.01f) && (Mathf.Abs(strafing) <= 0.01f))
                    {
                        ChangeStates(States.Idle);
                    }

                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (grounded)
                        {
                            rb.velocity = new Vector3(0.0f, jumpingHeight, 0.0f);
                        }
                    }

                    break;
                }
            default:
                {
                    ChangeStates(States.Idle);
                    break;
                }
        }
    }

    private void ChangeStates(States entering)
    {
        currentState = entering;
    }

    public void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag != "Something")
        {
            grounded = true;
        }
    }

    public void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag != "Light")
            grounded = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Light")
        {
            inLightTrigger = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Light")
        {
            inLightTrigger = false;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        //if (other.gameObject.tag == "Light")
        //{
        //    Ray inLight = new Ray(this.transform.position, -((this.transform.position - other.gameObject.transform.position) / Vector3.Distance(this.transform.position, other.gameObject.transform.position)));
        //    Debug.DrawLine(this.transform.position, other.gameObject.transform.position, Color.green);
        //    Debug.DrawRay(this.transform.position, -((this.transform.position - other.gameObject.transform.position)/Vector3.Distance(this.transform.position, other.gameObject.transform.position)), Color.red);
        //    RaycastHit lightCheck;
        //    //LayerMask obstruction = LayerMask.GetMask("Obstruction");
        //    if (Physics.Raycast(inLight, out lightCheck, obstruction))
        //    {
        //        Debug.Log(lightCheck.transform.gameObject.tag);
        //        if (lightCheck.transform.gameObject.tag != "Light")
        //        {
        //            Debug.Log("Light is being blocked!!!");
        //        }
        //        else
        //        {
        //            Debug.Log("OI IM IN THE LIGHT!!!!");
        //        }
        //        //Call Some Function to be written in about 5 minutes
        //    }
        //}
    }

    IEnumerable CanDrop()
    {
        canDropObject = true;
        yield return new WaitForSeconds(1);
    }
}
