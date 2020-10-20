using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Drive driveScript;
    // Start is called before the first frame update
    void Start()
    {
        driveScript = this.GetComponent<Drive>();
    }

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float s = Input.GetAxis("Horizontal");
        float b = Input.GetAxis("Jump");

        driveScript.Go(a, s, b);

        driveScript.ChecarPatinado();
        driveScript.CalculateEngineSound();
    }
}
