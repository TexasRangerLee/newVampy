using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldShuttersOpenCloseLerpScript : MonoBehaviour
{

    //reference to object this is on; kept so both shutters
    //can start up properly
    public GameObject self;

    //reference to entire shutter object (parent)
    public GameObject parent;

    //both shutters are referenced so i don't have to figure out which
    //one is which and have them call the other only if this and that and
    //all that bullshit

    //reference to left shutter
    public GameObject left;
    public OldShuttersOpenCloseLerpScript leftScript;
    //reference to other shutter
    public GameObject right;
    public OldShuttersOpenCloseLerpScript rightScript;

    //set up start/end rotations for lerping
    Quaternion startRotation;
    Quaternion endRotation;

    //set up increment value for lerping
    //NO LONGER IN USE
    //public float lerpRate;

    //boolean to turn on/off lerping and prevent multiple lerp calls
    public bool readyToTurn;

    //float that keeps track of total time passed while lerping; alleviates frame dependence
    public float totalDeltaTime;


    //DOOR STUFF BELOW (totally ripped from door)

    //boolean to know if shutter is open/closed
    //MUST BE SET IN EDITOR TO BE CONSIDERED OPEN AT START
    public bool isOpen;

    //pair of vectors for door opening/closing
    //only supports 90 degree turns
    Vector3 clockwise;
    Vector3 counterclockwise;


    // Use this for initialization
    void Start()
    {
        //set lerprate to (hopefully) work over 1 second
        //NO LONGER IN USe
        //lerpRate = 1.00f / 60.00f;

        //make reference to self
        self = this.gameObject;

        //make reference to parent
        parent = this.transform.parent.gameObject;

        //make reference to both shutters (MUST RESPECT SHUTTER ORDER; RIGHT IS ALWAYS 0,
        //LEFT IS ALWAYS 1, DO NOT SCREW THIS UP!
        left = parent.transform.GetChild(1).gameObject;
        leftScript = left.GetComponent<OldShuttersOpenCloseLerpScript>();

        right = parent.transform.GetChild(0).gameObject;
        rightScript = right.GetComponent<OldShuttersOpenCloseLerpScript>();

        //prop should start off ready to turn
        readyToTurn = true;

        //set starting rotation
        startRotation = self.transform.rotation;

        //if isOpen is set to true in editor, this ensures the decision is respected
        //if not, then respects that as well; could probably use less code, but I'd rather be certain
        if (isOpen)
        {
            isOpen = true;
        }
        else
        {
            isOpen = false;
        }

        //initialize turning vectors to turn 90 degrees either way
        clockwise = new Vector3(0, 90f, 0);
        counterclockwise = new Vector3(0, -90f, 0);

        //total delta time should start as 0
        totalDeltaTime = 0.00f;
    }

    // Update is called once per frame
    void Update()
    {
        //Code below for triggering lerp on keycode; deprecated with new function system below
        if (Input.GetKeyDown(KeyCode.K) && readyToTurn)
        {
            //Old code to move door on keypress
            ToggleShutters();
        }

        //NEW FRAME INDEPENDENT LERPING STUFF BELOW

        //if the prop is trying to turn, increases total delta time by delta time and lerps to
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

        //OLD FRAME DEPENDENT STUFF BELOW; NO LONGER IN USE
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

    //Purpose:  Call OpenShutters or CloseShutters, as appropriate
    //Input:    None
    //Output:   None, calls open/close shutters functions
    //Extra:    Relies on isOpen bool to know which function to call
    public void ToggleShutters()
    {
        if (isOpen) //door is open
        {
            CloseShutters();
        }
        else if (!isOpen) //door is closed
        {
            OpenShutters();
        }
    }

    //Purpose:  Open shutters, they only open away from the affixed window
    //Input:    None YET, NOT FULLY IMPLEMENTED
    //Output:   None, calls DoSomeLerping on both shutters to open them
    public void OpenShutters()
    {
        if (readyToTurn)
        {
            leftScript.DoSomeLerping(counterclockwise);
            rightScript.DoSomeLerping(clockwise);
            isOpen = true;
        }
    }

    //Purpose:  Close shutters
    //Input:    None
    //Output:   None, calls DoSomeLerping on both scripts to close shutters
    public void CloseShutters()
    {
        if (readyToTurn)
        {
            leftScript.DoSomeLerping(clockwise);
            rightScript.DoSomeLerping(counterclockwise);
            isOpen = false;
        }
    }
}
