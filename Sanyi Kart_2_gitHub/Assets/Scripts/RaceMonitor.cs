using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RaceMonitor : MonoBehaviour
{

    public GameObject[] countDownItems;
    public static bool racing = false;
    public static int totalLaps = 1;
    public GameObject gameOverPanel;
    public GameObject HUD;
    CheckPointManager[] carsCheckPM;
    public GameObject[] carPrefabs;
    public Transform[] spawnPos;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject g in countDownItems)
            g.SetActive(false);

        StartCoroutine(PlayCountDown());
        gameOverPanel.SetActive(false);

        foreach(Transform t in spawnPos)
        {
            
            GameObject car = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)]);
            car.transform.position = t.position;
            car.transform.rotation = t.rotation;
        }

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

    public void RestartLevel()
    {
        racing = false;
        SceneManager.LoadScene("Game");
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
