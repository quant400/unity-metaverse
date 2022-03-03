﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPS_Display : MonoBehaviour
{
    float timeA;
    public int fps;
    public int lastFPS;
    public GUIStyle textStyle;
    
    // Use this for initialization
    void Start () {
        timeA = Time.timeSinceLevelLoad;
        DontDestroyOnLoad (this);
    }
    
    // Update is called once per frame
    void Update () {
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
        GUI.Label(new Rect( 10,10, 30,30),"FPS: "+lastFPS,textStyle);
    }


}
