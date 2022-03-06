using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class StartMenu : MonoBehaviour
{

    [SerializeField] private Button start_Button;
    [SerializeField] private GameObject loading_GO;
    
    void Awake()
    {
        SetUpUI();
    }

    private void SetUpUI()
    {
        start_Button.onClick.AddListener(OnStartClick);
    }


    void OnStartClick()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        loading_GO.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        loading_GO.SetActive(false);
    }

}
