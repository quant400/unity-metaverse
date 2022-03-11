using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class StartMenu : MonoBehaviour
{

    [SerializeField] private Button start_Button;
    [SerializeField] private Button select_Button;
    [SerializeField] private GameObject loading_GO;
    
    [SerializeField] private GameObject start_GO;
    [SerializeField] private GameObject select_GO;
    
    void Awake()
    {
        SetUpUI();
    }

    private void SetUpUI()
    {
        start_Button.onClick.AddListener(OnStartClick);
        select_Button.onClick.AddListener(OnSelectClick);
    }


    void OnStartClick()
    {
        start_GO.SetActive(false);
        select_GO.SetActive(true);
    }
    
    void OnSelectClick()
    {
        OpenGame();
    }

    void OpenGame()
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
