using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameUIController : MonoBehaviour
{

    public Text playerName;
    public Text lapDisplay;
    public Transform target;
    CanvasGroup canvasGroup;
    public Renderer carRenderer;
    CheckPointManager cpManager;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        playerName = this.GetComponent<Text>();
        canvasGroup = this.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(!RaceMonitor.racing) { canvasGroup.alpha = 0; return; }
        if (carRenderer == null) return;
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        bool carInView = GeometryUtility.TestPlanesAABB(planes, carRenderer.bounds);
        canvasGroup.alpha = carInView ? 1 : 0;
        this.transform.position = Camera.main.WorldToScreenPoint(target.position + Vector3.up * 2.0f);

        if (cpManager == null)
            cpManager = target.GetComponent<CheckPointManager>();

        lapDisplay.text = "Lap: " + cpManager.lap + "(CP: " + cpManager.checkPoint + ")";
        
    }
}
