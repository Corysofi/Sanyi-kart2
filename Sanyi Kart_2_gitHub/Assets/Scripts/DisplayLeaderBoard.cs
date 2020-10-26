using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class DisplayLeaderBoard : MonoBehaviour
{

    public Text first;
    public Text second;

    

    
    void LateUpdate()
    {
        List<string> places = Leaderboard.GetPlaces();
        first.text = places[0];
        second.text = places[1];
    }
}
