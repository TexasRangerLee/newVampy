using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vamp_Player_Controller : MonoBehaviour
{
    enum States { Idle, Walking, Running, Jumping };

    public float walkingSpeed;
    public float runningSpeed;
    public float jumpingHeight;

    [SerializeField]
    States currentState;

    [SerializeField]
    bool grounded;

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
        float forwardMovenemt = Input.GetAxis("Vertical");
        float straffing = Input.GetAxis("Horizontal");

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
                            interact.transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
                            interact.transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                            //interact.transform.position = new Vector3(this.transform.GetChild(0).transform.position.x, 
                            //    this.transform.GetChild(0).transform.position.y, this.transform.GetChild(0).transform.transform.position.z + 5.0f);
                            interact.transform.gameObject.transform.position = this.transform.TransformPoint(0,0,2.5f/*this.transform.GetChild(0).transform.position.x, 
                                this.transform.GetChild(0).transform.position.y, this.transform.GetChild(0).transform.position.z + 2.5f*/);
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

                    if ((Mathf.Abs(forwardMovenemt) <= 0.01f) && (Mathf.Abs(straffing) <= 0.01f))
                    {
                        ChangeStates(States.Idle);
                    }
                    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
                    {
                        forward = transform.forward * forwardMovenemt;
                    }
                    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    {
                        straiffing = transform.right * straffing;
                    }

                    //else
                    //{
                    //    Vector3 walking = new Vector3(straffing, 0.0f, forwardMovenemt);

                    //    walking = walking.normalized * walkingSpeed * Time.deltaTime;
                    //    rb.MovePosition(transform.position + walking);
                    //}

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
                        forward = transform.forward * forwardMovenemt;
                    }
                    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    {
                        straiffing = transform.right * straffing;
                    }

                    running = forward + straiffing;
                    running = running.normalized * runningSpeed * Time.deltaTime;
                    rb.MovePosition(transform.position + running);

                    if (Input.GetKeyUp(KeyCode.LeftShift))
                    {
                        ChangeStates(States.Walking);
                    }
                    if ((Mathf.Abs(forwardMovenemt) <= 0.01f) && (Mathf.Abs(straffing) <= 0.01f))
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
            case States.Jumping:
                {
                    rb.useGravity = false;
                    if ((rb.position.x <= maxHeight.x) && (rb.position.y <= maxHeight.y) && (rb.position.z <= maxHeight.z) && !falling)
                    {
                        Vector3 jump = transform.up;
                        jump = jump * jumpingHeight * Time.deltaTime;
                        //Debug.Log(jump);
                        rb.MovePosition(transform.position + jump);
                    }
                    else
                    {
                        falling = true;
                    }

                    if (falling)
                    {
                        //Vector3 fallingVector = -transform.up;
                        //fallingVector = fallingVector * Time.deltaTime;
                        //rb.MovePosition(transform.position - fallingVector);

                        rb.useGravity = true;

                        if (rb.position == groundedPosition)
                        {
                            falling = false;
                            ChangeStates(States.Idle);
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
        grounded = false;
    }
}
