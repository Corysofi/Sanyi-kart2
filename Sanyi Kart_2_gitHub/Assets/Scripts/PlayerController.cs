using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Drive driveScript;
    float lastTimeMoving = 0;
    Vector3 lastPosition;
    Quaternion lastRotation;

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
    }

    // Update is called once per frame
    void Update()
    {
       // if (!RaceMonitor.racing) return;

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
            driveScript.rb.gameObject.transform.position = lastPosition;
            driveScript.rb.gameObject.transform.rotation = lastRotation;
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
