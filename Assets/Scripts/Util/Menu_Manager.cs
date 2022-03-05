using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Manager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject Menu_GUI;
    [SerializeField] private Button Button_Start;
    [SerializeField] private Button Button_LevelReset;

    [Header("Streamer Components")]
    [SerializeField] private UILoadingStreamer GO_UILoadingStreamer;
    [SerializeField] private PlayerTeleport GO_PlayerTeleport;


    void Start()
    {
        Button_Start.onClick.AddListener(ActionStart);
        GO_UILoadingStreamer.onDone.AddListener(ActionFinish);
        Button_LevelReset.onClick.AddListener(ActionReset);

    }

    private void ActionStart()
    {
       //Inicia o Loading

        
    }

    private void ActionFinish()
    {
        StartCoroutine(WaitLoading(() => { Menu_GUI.gameObject.SetActive(false); }));
    }

    private void ActionReset()
    {
        //Reseta a Cena
        Debug.Log("ActionReset");
        GO_PlayerTeleport.Teleport(true);
    }


    IEnumerator WaitLoading(Action onFinish , float time= 0.10f)
    {
        yield return new WaitForSeconds(time);
        onFinish();

    }

}
