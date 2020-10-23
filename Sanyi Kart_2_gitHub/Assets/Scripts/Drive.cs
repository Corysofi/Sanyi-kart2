using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Drive : MonoBehaviour
{
    public WheelCollider[] WCs;
    public GameObject[] Wheels;
    public float torque = 200;
    public float maxSteerAngle = 30; // angulo de direccion
    public float maxBrakeTorque = 500;
    public AudioSource skidSound;
    public AudioSource highAccel;
    public Transform patinarPistaPrefab;
    Transform[] patinarPista = new Transform[4];
    public ParticleSystem smokePrefab;
    ParticleSystem[] skidSmoke = new ParticleSystem[4];
    public Rigidbody rb; // para medir que tan rapido va el vehiculo
    public float gearLength = 3;
    public float currentSpeed { get { return rb.velocity.magnitude * gearLength; } }
    public float lowPitch = 1;
    public float highPitch = 6f;
    public int numGears = 5;
    float rpm;
    int currentGear = 1;
    float currentGearPerc;
    public float maxSpeed = 200;
    public GameObject playerNamePrefab;
    public Renderer carMesh;
    string[] aiNames = { "Cory", "Eva" };


   
    public void IniciarPatinarPista(int i)
    {
        if(patinarPista[i] == null)
        {
            patinarPista[i] = Instantiate(patinarPistaPrefab);
            //  patinarPista[i].parent = WCs[i].transform;
            patinarPista[i].localRotation = Quaternion.Euler(90, 0, 0);
            patinarPista[i].localPosition = -Vector3.up * WCs[i].radius;  //pondra las marcas debajo de las llantas
        }
    }

    public void PararPatinarPista(int i)
    {
        if (patinarPista[i] == null) return;
        Transform holder = patinarPista[i];
        patinarPista[i] = null;
        holder.parent = null;
        holder.rotation = Quaternion.Euler(90, 0, 0);
        Destroy(holder.gameObject, 30);
    }

    // Start is called before the first frame update
    void Start()
    {
        
        for(int i =0;i<4;i++)
        {
            skidSmoke[i] = Instantiate(smokePrefab);
            skidSmoke[i].Stop();  
        }

        GameObject playerName = Instantiate(playerNamePrefab);
        playerName.GetComponent<NameUIController>().target = rb.gameObject.transform;

        if (this.GetComponent<AIController>().enabled)
            playerName.GetComponent<Text>().text = aiNames[Random.Range(0, aiNames.Length)];
        else
            playerName.GetComponent<Text>().text = "Human";
        playerName.GetComponent<NameUIController>().carRenderer = carMesh;
    }

    public void CalculateEngineSound()
    {
        float gearPercentage = (1 / (float)numGears);
        float targetGearFactor = Mathf.InverseLerp(gearPercentage * currentGear, gearPercentage * (currentGear + 1), Mathf.Abs(currentSpeed / maxSpeed));
        currentGearPerc = Mathf.Lerp(currentGearPerc, targetGearFactor, Time.deltaTime * 5f);

        var gearNumFactor = currentGear/ (float) numGears;
        rpm = Mathf.Lerp(gearNumFactor, 1, currentGearPerc);

        float speedPercentage = Mathf.Abs(currentSpeed / maxSpeed);
        float upperGearMax = (1 / (float)numGears) * (currentGear + 1);
        float downGearMax = (1 / (float)numGears) * currentGear;

        if (currentGear > 0 && speedPercentage < downGearMax)
            currentGear--;

        if (speedPercentage > upperGearMax && (currentGear < (numGears - 1)))
            currentGear++;

        float pitch = Mathf.Lerp(lowPitch, highPitch, rpm);
        highAccel.pitch = Mathf.Min(highPitch, pitch) * 0.25f;
        
    }

    public void Go(float accel, float steer, float brake)
    {
        accel = Mathf.Clamp(accel, -1, 1);
        steer = Mathf.Clamp(steer, -1, 1) * maxSteerAngle;
        brake = Mathf.Clamp(brake, 0, 1) * maxBrakeTorque;

        float thrustTorque = 0;
        if(currentSpeed < maxSpeed)
            thrustTorque = accel * torque;  // empuje = aceleracion * esfuerzo de torsion

        for (int i = 0; i < 4; i++)
        {
            WCs[i].motorTorque = thrustTorque;  // torque del motor en el eje de la rueda, positivo o negativo dependiendo la direccion

            if (i < 2)
                WCs[i].steerAngle = steer;   //angulo de direccion
            else
                WCs[i].brakeTorque = brake;  //torque de freno expresado en metros Newton, debe ser positivo

            Quaternion quat;
            Vector3 position;
            WCs[i].GetWorldPose(out position, out quat); // getworlpose obtiene la posición espacial del mundo de la rueda que cuenta el contacto con el suelo, los limites de suspension, el angulo de direccion y el de rotacion. pos = posicion de la llanta en el mundo, quat la rotacion
            Wheels[i].transform.position = position;
            Wheels[i].transform.localRotation = quat;
        }
    }
    public void ChecarPatinado()
    {
        int numPatinado = 0;
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelHit;
            WCs[i].GetGroundHit(out wheelHit);

            if (Mathf.Abs(wheelHit.forwardSlip) >= 0.7f || Mathf.Abs(wheelHit.sidewaysSlip) >= 0.7f)
            {
                numPatinado++;
                if (!skidSound.isPlaying)
                {
                    skidSound.Play();
                }
                // IniciarPatinarPista(i);
                skidSmoke[i].transform.position = WCs[i].transform.position - WCs[i].transform.up * WCs[i].radius;
                skidSmoke[i].Emit(1);
            }
            else
            {
               // PararPatinarPista(i);
            }
        }
        if(numPatinado == 0 && skidSound.isPlaying)
        {
            skidSound.Stop();
        }
    }

    // Update is called once per frame
  
}
