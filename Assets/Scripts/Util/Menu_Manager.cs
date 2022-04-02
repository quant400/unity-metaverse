using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;

public class Menu_Manager : MonoBehaviour
{
    [Header("UI MENU")]
    [SerializeField] private GameObject Menu_GUI;
    [SerializeField] private Button Button_LevelReset;

    [Header("UI MENU")]
    [SerializeField] private GameObject Tutorial_GUI;

    [Header("UI CHICKEN_RUN")]
    [SerializeField] private GameObject MenuCR_GUI;
    [SerializeField] private Button ButtonCR_Start;

    [Header("Streamer Components")]
    [SerializeField] private UILoadingStreamer GO_UILoadingStreamer;
    [SerializeField] private PlayerTeleport GO_PlayerTeleport;
    

    void Start()
    {
        GO_UILoadingStreamer.onDone.AddListener(ActionFinish);
        Button_LevelReset.onClick.AddListener(ActionReset);
        ButtonCR_Start.onClick.AddListener(()=> { ActionOpenLink("https://play.cryptofightclub.io/"); });
    }

    public void OpenPanelCR()
    {
        if(MenuCR_GUI != null)
        {
            MenuCR_GUI.SetActive(true);
        }
       
    }

    public void ShowTutorial()
    {
        if (Tutorial_GUI != null)
        {
            Tutorial_GUI.SetActive(true);
            StartCoroutine(WaitLoading(() => { Tutorial_GUI.SetActive(false); }, 18.00f));
        }
    }

    private void ActionFinish()
    {
        if (Menu_GUI != null)
        {
            StartCoroutine(WaitLoading(() => { Menu_GUI.gameObject.SetActive(false); }));
        }
           
    }

    private void ActionReset()
    {
        //Reseta a Cena
        Debug.Log("ActionReset");
        if(GO_PlayerTeleport != null)
        {
            GO_PlayerTeleport.Teleport(true);
        }
        
    }

    private void ActionOpenLink(string url)
    {
        float startTime = Time.timeSinceLevelLoad;
        if (Time.timeSinceLevelLoad - startTime <= 1f)
        {
            Application.OpenURL(url);
        }
    }

    private IEnumerator WaitLoading(Action onFinish , float time= 0.10f)
    {
        yield return new WaitForSeconds(time);
        onFinish();

    }

}
