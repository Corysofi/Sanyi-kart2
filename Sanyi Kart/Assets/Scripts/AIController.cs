using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{

    public Circuit circuit;
    Drive driveScript;
    public float steeringSensitivity = 0.01f;
    Vector3 target;
    Vector3 nextTarget;
    int currentWPoint = 0;
    float totalDistanceToTarget;
    public float brakingSensitivity = 3f;
    public float accelSensitivity = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        driveScript = this.GetComponent<Drive>();
        target = circuit.waypoints[currentWPoint].transform.position;
        nextTarget = circuit.waypoints[currentWPoint + 1].transform.position;
        totalDistanceToTarget = Vector3.Distance(target, driveScript.rb.gameObject.transform.position);
      
    }
    bool isJump = false;
    // Update is called once per frame
    void Update()
    {
        Vector3 localTarget = driveScript.rb.gameObject.transform.InverseTransformPoint(target);
        Vector3 nextLocalTarget = driveScript.rb.gameObject.transform.InverseTransformPoint(nextTarget);
        float distanceToTarget = Vector3.Distance(target, driveScript.rb.gameObject.transform.position);

        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float nextTargetAngle = Mathf.Atan2(nextLocalTarget.x, nextLocalTarget.z) * Mathf.Rad2Deg;
        float steer = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(driveScript.currentSpeed);

        float distanceFactor = distanceToTarget / totalDistanceToTarget;
        float speedFactor = driveScript.currentSpeed / driveScript.maxSpeed;

        float accel = Mathf.Lerp(accelSensitivity, 1, distanceFactor);
        float brake = Mathf.Lerp((-1 - Mathf.Abs(nextTargetAngle)) * brakingSensitivity, 1 + speedFactor, 1 - distanceFactor);

        if(Mathf.Abs(nextTargetAngle) >20) // dependiendo el angulo de la curva ajustará estos valores
        {
            brake += 0.8f;
            accel -= 0.8f;
        }

        if(isJump)
        {
            accel = 1;
            brake = 0;
        }



       // if(distanceToTarget < 3) { brake = 0.8f;accel = 0.1f; } // para frenar y acelerar en la vueltas

        driveScript.Go(accel, steer, brake);

        if(distanceToTarget < 4) // iniciamos con 2 pero si empieza a dar vueltas lo podemos incrementar
        {
            currentWPoint++;
            if (currentWPoint >= circuit.waypoints.Length)
                currentWPoint = 0;
            target = circuit.waypoints[currentWPoint].transform.position;

            if(currentWPoint == circuit.waypoints.Length -1)  // indica si es el último waypoint
            nextTarget = circuit.waypoints[0].transform.position;
            else
                nextTarget = circuit.waypoints[currentWPoint + 1].transform.position;
            totalDistanceToTarget = Vector3.Distance(target, driveScript.rb.gameObject.transform.position);

            if (driveScript.rb.gameObject.transform.InverseTransformPoint(target).y > 5)  // para saber si el carro esta volando
            {
                isJump = true;
            }
            else isJump = false;

        }

        driveScript.ChecarPatinado();
        driveScript.CalculateEngineSound();
    }
}
