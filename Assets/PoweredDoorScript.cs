using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoweredDoorScript : MonoBehaviour
{
    //variables used for lighting up door markers for powered doors
    //MUST BE SET IN EDITOR
    public GameObject lightningA;
    public GameObject lightningB;

    public PowerToggleScript scriptA;
    public PowerToggleScript scriptB;

    //powered variant variables
    public GameObject connectedWire;  //MUST BE SET IN EDITOR
    public bool doorHasPower;
    public PowerToggleScript wireScript;

    //reference to object this is on
    public GameObject self;

    //set up start/end rotations for lerping
    Quaternion startRotation;
    Quaternion endRotation;

    //boolean to turn on/off lerping and prevent multiple lerp calls
    public bool readyToTurn;

    //float that keeps track of total time passed while lerping; alleviates frame dependence
    public float totalDeltaTime;


    //DOOR STUFF BELOW

    //boolean to know if door is open/closed
    //MUST BE SET IN EDITOR TO BE CONSIDERED OPEN AT START
    public bool isOpen;

    //pair of vectors for door opening/closing
    //only supports 90 degree turns
    Vector3 clockwise;
    Vector3 counterclockwise;

    //bool to know which way to turn the door on close
    //if door starts open, MUST BE SET IN EDITOR
    //if door was opened clockwise, is true; else is false
    public bool lastWasClockwise;


    // Use this for initialization
    void Start()
    {
        //make reference to attached object
        self = this.gameObject;

        //prop should start off ready to turn
        readyToTurn = true;

        //set starting rotation
        startRotation = self.transform.rotation;

        //initialize turning vectors to turn 90 degrees either way
        clockwise = new Vector3(0, 90f, 0);
        counterclockwise = new Vector3(0, -90f, 0);

        //total delta time should start as 0
        totalDeltaTime = 0.00f;

        //set scripts if items are present
        if (connectedWire != null)
        {
            wireScript = connectedWire.GetComponent<PowerToggleScript>();
        }

        if (lightningA != null)
        {
            scriptA = lightningA.GetComponent<PowerToggleScript>();
            scriptB = lightningB.GetComponent<PowerToggleScript>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Code below for triggering lerp on keycode; deprecated with new function system below
        //if (Input.GetKeyDown(KeyCode.K) && readyToTurn)
        //{
        //    //Old code to move door on keypress
        //    MoveDoor();
        //}

        //NEW FRAME INDEPENDENT LERPING STUFF BELOW

        //if the door is trying to move, increases total delta time by delta time and lerps to
        //new location
        if (readyToTurn == false)
        {
            //if totaldeltatime would be greater than 1, sets it to 1 to avoid lerping weirdness
            //otherwise, increases totaldeltatime by deltatime and as usual
            if (totalDeltaTime + Time.deltaTime >= 1)
            {
                totalDeltaTime = 1;
            }
            else
            {
                totalDeltaTime += Time.deltaTime;
            }
            self.transform.rotation = Quaternion.Slerp(startRotation, endRotation, totalDeltaTime);

            //if current rotation is destination rotation (is done lerping), resets relevant values
            if (self.transform.rotation == endRotation)
            {
                readyToTurn = true;
                totalDeltaTime = 0;
                startRotation = self.transform.rotation;
            }
        }

        //OLD FRAME DEPENDENT CODE, NO LONGER IN USE
        //lerp if statement (loops on update), SCREW COROUTINES; if it's not ready to turn, it's trying to turn
        //if (readyToTurn == false)
        //{
        //    //set lerp to new location, increase lerp rate for next go-through or lerp fall-out sequence
        //    self.transform.rotation = Quaternion.Slerp(startRotation, endRotation, lerpRate);
        //    lerpRate += (1.00f / 60.00f);

        //    //special catch, just in case things go screwy; cannot lerp infinitely
        //    if (lerpRate > 1)
        //    {
        //        //reset lerpRate to maximum; set lerping to final location just in case
        //        lerpRate = 1;
        //        Debug.Log("Lerping rate should be 1; lerpRate is now: " + lerpRate);
        //        self.transform.rotation = Quaternion.Slerp(startRotation, endRotation, 1);
        //    }

        //    //if done lerping, reset relevant values, get ready to lerp some more
        //    if (self.transform.rotation == endRotation)
        //    {
        //        readyToTurn = true;
        //        lerpRate = (1.00f / 60.00f);
        //        startRotation = self.transform.rotation;
        //    }
        //}

    }

    //Purpose:  Set up lerping parameters, should be called from outside context
    //Input:    Vector 3 representing how the object should lerp (Vector3= (0,90f,0) rotates 90 degrees on Y axis)
    //Output:   None, engages starting values so lerp can occur in update
    //Extra:    Lerp should occur over 1 second, moving smoothly
    public void DoSomeLerping(Vector3 end)
    {
        if (readyToTurn)
        {
            //attached object no longer ready to turn; is turning
            readyToTurn = false;

            //clear old start value and ensure it is current
            startRotation = self.transform.rotation;

            //clear old end value and recalculate
            endRotation = startRotation * Quaternion.Euler(end);
        }
    }

    //Purpose:  Call OpenDoor or CloseDoor, as appropriate
    //Input:    None
    //Output:   None, calls open/close door functions
    //Extra:    Relies on isOpen bool to know which function to call
    public void MoveDoor()
    {
        if (isOpen) //door is open
        {
            CloseDoor();
        }
        else //door is closed
        {
            OpenDoor();
        }
    }

    //Purpose:  Open doors, depending on which side the player is on
    //Input:    None YET, NOT FULLY IMPLEMENTED
    //Output:   None, calls DoSomeLerping, passing it the proper value to open away from the player
    //Extra:    NOT FULLY IMPLEMENTED
    public void OpenDoor()
    {
        if (true && readyToTurn) //player needs to open door clockwise (hinge is to the right)
        {
            DoSomeLerping(clockwise);
            isOpen = true;
            lastWasClockwise = true;
        }
        else if (false && readyToTurn) //player needs to open door counterclockwise (hinge is to the left)
        {
            DoSomeLerping(counterclockwise);
            isOpen = true;
            lastWasClockwise = false;
        }
    }

    //Purpose:  Close doors, depending on how it was last opened
    //Input:    None YET, NOT FULLY IMPLEMENTED
    //Output:   None, calls DoSomeLerping, passing it the proper value to close the door
    public void CloseDoor()
    {
        if (lastWasClockwise && readyToTurn)
        {
            DoSomeLerping(counterclockwise);
            isOpen = false;
        }
        else if (!lastWasClockwise && readyToTurn) //last was counterclockwise
        {
            DoSomeLerping(clockwise);
            isOpen = false;
        }
    }

    public void LateUpdate()
    {
        doorHasPower = isOpen;
        scriptA.changePowerState(doorHasPower);
        scriptB.changePowerState(doorHasPower);

        //catch on powered doors; door will move to end position uninterrupted
        //then check to make sure it matches power state; if not, it moves back uninterrupted
        if (connectedWire != null)
        {
            if (wireScript.hasPower != doorHasPower)
            {
                MoveDoor();
            }
        }
    }
}
