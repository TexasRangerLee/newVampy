using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Vamp_Player_Controller : MonoBehaviour
{
    enum States { Idle, Walking, Running, Jumping };

    public float walkingSpeed;
    public float runningSpeed;
    public float jumpingHeight;
    public bool inLightTrigger;

    public float maxHealth = 100;
    public float currentHealth;

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

    [SerializeField]
    Canvas PlayerUI;

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
        currentHealth = maxHealth;
        //PlayerUI.transform.GetChild(0).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(inLightTrigger + " from VPC.");
        if (!inLightTrigger && currentHealth != 100 && currentHealth != 0)
        {
            currentHealth += 5;
        }
    }

    void FixedUpdate()
    {

        float forwardMovement = Input.GetAxis("Vertical");
        float strafing = Input.GetAxis("Horizontal");

        if (canDropObject)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (holdingSomething)
                {
                    StartCoroutine("DropTheThing");
                }
            }
        }

        RaycastHit interact;
        //UnityEngine.Debug.DrawRay(this.gameObject.transform.GetChild(0).transform.position, this.gameObject.transform.GetChild(0).transform.forward * 2.5f);
        if (Physics.Raycast(this.transform.position, this.transform.GetChild(0).transform.forward, out interact, 4f))
        {
            if (interact.transform.gameObject != null)
            {
                //UnityEngine.Debug.Log(interact.transform.tag);
                if (interact.transform.gameObject.tag == "Battery")
                {
                    if (!holdingSomething)
                    {
                        if (Input.GetKeyUp(KeyCode.E))
                        {
                            if (interact.transform.gameObject.GetComponent<Rigidbody>().useGravity)
                            {
                                interact.transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
                                interact.transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                                interact.transform.gameObject.transform.parent = this.transform;
                                interact.transform.gameObject.transform.position = this.transform.TransformPoint(0, 0, 2.5f);
                                interact.transform.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                                holdingSomething = true;
                                canDropObject = false;
                                StartCoroutine("CanDrop");
                            }
                        }
                    }
                }

                //interact.transform.gameObject.GetComponent<GlowObject>().enabled = true;
                else if (interact.transform.gameObject.tag == "Door")
                {
                    if (Input.GetKeyUp(KeyCode.E))
                    {
                        interact.transform.gameObject.GetComponent<DoorOpenCloseLerpScript>().MoveDoor();
                    }
                }
                else if (interact.transform.gameObject.tag=="Left Shutter"|| interact.transform.gameObject.tag == "Right Tag")
                {
                    if (Input.GetKeyUp(KeyCode.E))
                    {
                        interact.transform.gameObject.GetComponent<OldShuttersOpenCloseLerpScript>().ToggleShutters(); 
                    }
                }

                if (interact.transform.gameObject.tag == "Socket")
                {
                    //PlayerUI.transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    //PlayerUI.transform.GetChild(0).gameObject.SetActive(false);
                }
            }

            //interact.transform.gameObject.GetComponent<GlowObject>().enabled = false;
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

    }

    IEnumerator CanDrop()
    {
        yield return new WaitForSeconds(1);
        canDropObject = true;
    }

    IEnumerator DropTheThing()
    {
        this.transform.GetChild(1).transform.gameObject.GetComponent<Rigidbody>().useGravity = true;
        this.transform.GetChild(1).transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        this.transform.GetChild(1).transform.localScale.Set(1, 1, 1);
        this.transform.GetChild(1).transform.parent = null;
        holdingSomething = false;
        canDropObject = false;

        yield return new WaitForSeconds(0.1f);
    }

    public void TakeDamage()
    {
        currentHealth -= 25;
    }
}
