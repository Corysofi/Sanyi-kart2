using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class DisplayLeaderBoard : MonoBehaviour
{

    public Text first;
    public Text second;

    private void Start()
    {
        Leaderboard.Reset();
    }


    void LateUpdate()
    {
        List<string> places = Leaderboard.GetPlaces();
        if(places.Count > 0)
        first.text = places[0];
        if(places.Count > 1)
        second.text = places[1];
    }
}
