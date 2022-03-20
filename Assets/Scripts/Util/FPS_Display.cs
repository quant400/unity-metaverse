using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPS_Display : MonoBehaviour
{
    float timeA;
    public int fps;
    public int lastFPS;
    public GUIStyle textStyle;

    private GameObject player;
    
    // Use this for initialization
    void Start () {
        timeA = Time.timeSinceLevelLoad;
        DontDestroyOnLoad (this);
    }
    
    // Update is called once per frame
    void Update () {
    
        if(player == null) player = GameObject.FindWithTag("Player");
        //Debug.Log(Time.timeSinceLevelLoad+" "+timeA);
        if(Time.timeSinceLevelLoad  - timeA <= 1)
        {
            fps++;
        }
        else
        {
            lastFPS = fps + 1;
            timeA = Time.timeSinceLevelLoad;
            fps = 0;
        }
    }
    void OnGUI()
    {
        if(player == null)
            GUI.Label(new Rect( 10,10, 30,30),"FPS: "+lastFPS,textStyle);
        else
            GUI.Label(new Rect( 10,10, 30,30),$"FPS: {lastFPS} \nPosition: {player.transform.position}", textStyle);
    }


}
