using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Drive driveScript;
    float lastTimeMoving = 0;
    Vector3 lastPosition;
    Quaternion lastRotation;
    CheckPointManager cpm;
    float finishSteer;

    void ResetLayer()
    {
        driveScript.rb.gameObject.layer = 0;
        this.GetComponent<Ghost>().enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        driveScript = this.GetComponent<Drive>();
        this.GetComponent<Ghost>().enabled = false;
        lastPosition = driveScript.rb.gameObject.transform.position;
        lastRotation = driveScript.rb.gameObject.transform.rotation;
        finishSteer = Random.Range(-1.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //if (!RaceMonitor.racing) return;

        if (cpm == null)
            cpm = driveScript.rb.GetComponent<CheckPointManager>();

        if(cpm.lap == RaceMonitor.totalLaps + 1)
        {
            driveScript.highAccel.Stop();
            driveScript.Go(0, finishSteer, 0);
            return;
        }

        float a = Input.GetAxis("Vertical");
        float s = Input.GetAxis("Horizontal");
        float b = Input.GetAxis("Jump");

        if (driveScript.rb.velocity.magnitude > 1 || !RaceMonitor.racing)
            lastTimeMoving = Time.time;

        RaycastHit hit;
        if(Physics.Raycast(driveScript.rb.gameObject.transform.position, -Vector3.up, out hit, 10))
        {
            if (hit.collider.gameObject.tag == "road")
            {
                lastPosition = driveScript.rb.gameObject.transform.position;
                lastRotation = driveScript.rb.gameObject.transform.rotation;
            }
                
        }

        if(Time.time > lastTimeMoving + 4)
        {
            

            driveScript.rb.gameObject.transform.position = cpm.lastCP.transform.position + Vector3.up * 2;
            driveScript.rb.gameObject.transform.rotation = cpm.lastCP.transform.rotation;
            driveScript.rb.gameObject.layer = 9;
            this.GetComponent<Ghost>().enabled = true;
            Invoke("ResetLayer", 3);
        }
        if (!RaceMonitor.racing) a = 0;

        if (!RaceMonitor.racing) a = 0;

        driveScript.Go(a, s, b);

        driveScript.ChecarPatinado();
        driveScript.CalculateEngineSound();
    }
}
