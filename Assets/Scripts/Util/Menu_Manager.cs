using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;

public class Menu_Manager : MonoBehaviour
{

    public static Menu_Manager Instance;
    
    [Header("UI MENU")]
    [SerializeField] private GameObject Menu_GUI;
    [SerializeField] private Button Button_LevelReset;

    [Header("UI TUTORIAL")]
    [SerializeField] private GameObject Tutorial_GUI;

    [Header("UI K.O")]
    [SerializeField] private GameObject KO_GUI;
    [SerializeField] Button Button_KO;

    [Header("UI CHICKEN_RUN")]
    [SerializeField] private GameObject MenuCR_GUI;
    [SerializeField] private Button ButtonCR_Start;

    [Header("UI STREAMER")]
    [SerializeField] private UILoadingStreamer GO_UILoadingStreamer;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GO_UILoadingStreamer.onDone.AddListener(ActionFinish);
        Button_LevelReset.onClick.AddListener(ActionReset);
        Button_KO.onClick.AddListener(ActionReset);
        ButtonCR_Start.onClick.AddListener(()=> { ActionOpenLink("https://play.cryptofightclub.io/"); });
    }

    public void ShowCR(bool show)
    {
        if(MenuCR_GUI != null)
        {
            MenuCR_GUI.SetActive(show);
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
    public void ShowKO(bool show = true)
    {
        KO_GUI.SetActive(show);
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
        ShowKO(false);
        
        SceneManager.LoadScene("Game");
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
