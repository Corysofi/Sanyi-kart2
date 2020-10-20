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
    GameObject tracker;
    int currentTrackerWpoint = 0;
    public float lookAhead = 12;
    float lastTimeMoving = 0;
    // Start is called before the first frame update
    void Start()
    {
        driveScript = this.GetComponent<Drive>();
        target = circuit.waypoints[currentWPoint].transform.position;
        nextTarget = circuit.waypoints[currentWPoint + 1].transform.position;
        totalDistanceToTarget = Vector3.Distance(target, driveScript.rb.gameObject.transform.position);

        tracker = GameObject.CreatePrimitive(PrimitiveType.Capsule);  // creamos una capsula como tracker para que lo siga el carrito
        DestroyImmediate(tracker.GetComponent<Collider>());           // destruimos el rigibody de la capsula para que no interfiera con el del carrito
        tracker.GetComponent<MeshRenderer>().enabled = false;         // desactivar el mesh
        tracker.transform.position = driveScript.rb.gameObject.transform.position;
        tracker.transform.rotation = driveScript.rb.gameObject.transform.rotation;

        this.GetComponent<Ghost>().enabled = false;
    }


    void ProgressTracker()
    {
        Debug.DrawLine(driveScript.rb.gameObject.transform.position, tracker.transform.position);

        if (Vector3.Distance(driveScript.rb.gameObject.transform.position, tracker.transform.position) > lookAhead) return; // es para que el tracker no se vaya tan rápido y espere el carrito
        tracker.transform.LookAt(circuit.waypoints[currentTrackerWpoint].transform.position);
        tracker.transform.Translate(0, 0, 1.0f); // la velocidad del tracker


        if (Vector3.Distance(tracker.transform.position, circuit.waypoints[currentTrackerWpoint].transform.position) < 1) // si empieza a dar vueltas para buscar un waypoint se debe subir el numero
        {
            currentTrackerWpoint++;
            if (currentTrackerWpoint >= circuit.waypoints.Length)
                currentTrackerWpoint = 0;
        }
    }
    //bool isJump = false;

    void ResetLayer()
    {
        driveScript.rb.gameObject.layer = 0;
        this.GetComponent<Ghost>().enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        ProgressTracker();
        Vector3 localTarget;
        float targetAngle;

        if (driveScript.rb.velocity.magnitude > 1)
            lastTimeMoving = Time.time;

        if(Time.time > lastTimeMoving + 4)
        {
            driveScript.rb.gameObject.transform.position = circuit.waypoints[currentTrackerWpoint].transform.position + Vector3.up * 2 + new Vector3(Random.Range(-1,1), 0, Random.Range(-1,1));
            tracker.transform.position = driveScript.rb.gameObject.transform.position;
            driveScript.rb.gameObject.layer = 9;

            this.GetComponent<Ghost>().enabled = true;
            Invoke("ResetLayer", 3);
        }

        if (Time.time < driveScript.rb.GetComponent<AvoidDetector>().avoidTime)
        {
            localTarget = tracker.transform.right * driveScript.rb.GetComponent<AvoidDetector>().avoidPath;
            
        }
        else
        {
            localTarget = driveScript.rb.gameObject.transform.InverseTransformPoint(tracker.transform.position);
            
        }
        targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        float steer = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(driveScript.currentSpeed);
        float brake = 0;
        float accel = 1f;
        float speedFactor = driveScript.currentSpeed / driveScript.maxSpeed;
        float corner = Mathf.Clamp(Mathf.Abs(targetAngle), 0, 90);
        float cornerFactor = corner / 90.0f;

        if(corner > 10 && speedFactor > 0.1f)
            brake = Mathf.Lerp(0, 1 + speedFactor * brakingSensitivity, cornerFactor);


        if (corner > 20 && speedFactor > 0.2f)
            accel = Mathf.Lerp(0, 1 * accelSensitivity, 1 - cornerFactor);

        driveScript.Go(accel, steer, brake);

        driveScript.ChecarPatinado();
        driveScript.CalculateEngineSound();



        //Vector3 nextLocalTarget = driveScript.rb.gameObject.transform.InverseTransformPoint(nextTarget);
        //float distanceToTarget = Vector3.Distance(target, driveScript.rb.gameObject.transform.position);


        //float nextTargetAngle = Mathf.Atan2(nextLocalTarget.x, nextLocalTarget.z) * Mathf.Rad2Deg;
       

        //float distanceFactor = distanceToTarget / totalDistanceToTarget;
        //float speedFactor = driveScript.currentSpeed / driveScript.maxSpeed;

        //float accel = Mathf.Lerp(accelSensitivity, 1, distanceFactor);
        //float brake = Mathf.Lerp((-1 - Mathf.Abs(nextTargetAngle)) * brakingSensitivity, 1 + speedFactor, 1 - distanceFactor);

        //if(Mathf.Abs(nextTargetAngle) >20) // dependiendo el angulo de la curva ajustará estos valores
        //{
        //    brake += 0.8f;
        //    accel -= 0.8f;
        //}

        //if(isJump)
        //{
        //    accel = 1;
        //    brake = 0;
        //}



        // if(distanceToTarget < 3) { brake = 0.8f;accel = 0.1f; } // para frenar y acelerar en la vueltas



        //if(distanceToTarget < 4) // iniciamos con 2 pero si empieza a dar vueltas lo podemos incrementar
        //{
        //    currentWPoint++;
        //    if (currentWPoint >= circuit.waypoints.Length)
        //        currentWPoint = 0;
        //    target = circuit.waypoints[currentWPoint].transform.position;

        //    if(currentWPoint == circuit.waypoints.Length -1)  // indica si es el último waypoint
        //    nextTarget = circuit.waypoints[0].transform.position;
        //    else
        //        nextTarget = circuit.waypoints[currentWPoint + 1].transform.position;
        //    totalDistanceToTarget = Vector3.Distance(target, driveScript.rb.gameObject.transform.position);

        //    if (driveScript.rb.gameObject.transform.InverseTransformPoint(target).y > 5)  // para saber si el carro esta volando
        //    {
        //        isJump = true;
        //    }
        //    else isJump = false;

        //}


    }
}
