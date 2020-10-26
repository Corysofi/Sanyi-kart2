using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceMonitor : MonoBehaviour
{

    public GameObject[] countDownItems;
    public static bool racing = false;
    public static int totalLaps = 1;
    public GameObject gameOverPanel;
    public GameObject HUD;
    CheckPointManager[] carsCheckPM;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject g in countDownItems)
            g.SetActive(false);

        StartCoroutine(PlayCountDown());
        gameOverPanel.SetActive(false);

        GameObject[] cars = GameObject.FindGameObjectsWithTag("car");
        carsCheckPM = new CheckPointManager[cars.Length];
        for (int i = 0; i < cars.Length; i++)
            carsCheckPM[i] = cars[i].GetComponent<CheckPointManager>();
    }

    IEnumerator PlayCountDown()
    {
        yield return new WaitForSeconds(2);
        foreach(GameObject g in countDownItems)
        {
            g.SetActive(true);
            yield return new WaitForSeconds(1);
            g.SetActive(false);
        }
        racing = true;
    }

    
    void LateUpdate()
    {
        int finishedCount = 0;
        foreach(CheckPointManager cpm in carsCheckPM)
        {
            if (cpm.lap == totalLaps + 1)
                finishedCount++;
        }
        if(finishedCount == carsCheckPM.Length)
        {
            HUD.SetActive(false);
            gameOverPanel.SetActive(true);
        }
    }
}
