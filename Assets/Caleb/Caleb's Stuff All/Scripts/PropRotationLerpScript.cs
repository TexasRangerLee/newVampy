using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRotationLerpScript : MonoBehaviour
{
    //This value used for debug lerp test; also compatible with deprecated keypress code
    Vector3 dumpy;

    //reference to object this is on
    public GameObject self;

    //set up start/end rotations for lerping
    Quaternion startRotation;
    Quaternion endRotation;

    //set up increment value for lerping (MIGHT BE DEPRECATED SOON)
    //NO LONGER IN USE
    //public float lerpRate;

    //boolean to turn on/off lerping and prevent multiple lerp calls
    public bool readyToTurn;

    //boolean added so you can force an object to lerp to show it works without having to have
    //something call this script on the object
    public bool debugLerpTest;

    //float that keeps track of total time passed while lerping; alleviates frame dependence
    public float totalDeltaTime;


    // Use this for initialization
    void Start()
    {
        //sets dumpy; currently used in debug lerp test only
        dumpy = new Vector3(0, 90f, 0);

        //set lerprate to (hopefully) work over 1 second
        //NO LONGER IN USE
        //lerpRate = 1.00f / 60.00f;

        //make reference to attached object
        self = this.gameObject;

        //prop should start off ready to turn
        readyToTurn = true;

        //set starting rotation
        startRotation = self.transform.rotation;

        //set lerp test to off by default
        debugLerpTest = false;

        //total delta time should start as 0
        totalDeltaTime = 0.00f;
    }


    // Update is called once per frame
    void Update()
    {
        //Code below for triggering lerp on keycode; deprecated with new function system below
        if (Input.GetKeyDown(KeyCode.K) && readyToTurn)
        {
            //old code to turn props on keypress

            //attached object no longer ready to turn; is turning
            //readyToTurn = false;

            //clear old start value and ensure it is current
            //startRotation = self.transform.rotation;

            //clear old end value and recalculate
            //endRotation = startRotation * Quaternion.Euler(dumpy);
        }

        //lerp if statement (loops on update), SCREW COROUTINES; if it's not ready to turn, it's trying to turn
        if (debugLerpTest)
        {
            DoSomeLerping(dumpy);
            debugLerpTest = false;
        }

        //NEW FRAME INDEPENDENT LERPING STUFF BELOW

        //if the prop is trying to turn, increases total delta time by delta time and lerps to
        //new location
        if (readyToTurn == false)
        {
            //if totaldeltatime would be greater than 1, sets it to 1 to avoid lerping weirdness
            //otherwise, increases totaldeltatime by deltatime and as usual
            if (totalDeltaTime+Time.deltaTime >= 1)
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

        //OLD FRAME DEPENDENT LERPING STUFF, KEPT IN CASE WE NEED IT
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


    //Screw this enumerator/corouting stuff; I did this in update with not even 1/4 the headache!
    //IEnumerator DoTurnStuff(Vector3 angle)
    //{
    //    //this should make it lerp until lerping finishes and object is ready to turn again
    //    yield return new WaitUntil(() => readyToTurn); //WHAT DOES ()=> MEAN!? I'M CONFUSED! WHY AM I YELLING!? AAAAAAAAAAA!

    //    Debug.Log("AFTER THE YIELD");

    //    //no idea if this is right, saw this online; should give the proper angle to increase current angle by input angle
    //    endRotation = startRotation * Quaternion.Euler(angle);

    //    //this does the lerping; while attached object's angle doesn't match end angle, do/continue lerp
    //    while (self.transform.rotation != endRotation)
    //    {
    //        Debug.Log("WHILE");
    //        self.transform.rotation = Quaternion.Slerp(startRotation, endRotation, lerpRate);
    //        lerpRate += (1f / 60f);

    //        //special catch, just in case things go screwy; cannot lerp infinitely
    //        if (lerpRate > 1)
    //        {
    //            lerpRate = 1;
    //            Debug.Log("Lerping gone mad, lerpRate should be 1 at max; lerpRate is now: " + lerpRate);
    //        }
    //    }

    //    //reset starting values; now that lerping is done and coroutine will fall out, set object ready to turn again
    //    startRotation = self.transform.rotation;
    //    lerpRate = (1f / 60f);
    //    readyToTurn = true;
    //}
}
