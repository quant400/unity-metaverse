using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AgoraHomeUnityManager : MonoBehaviour
{

    public static AgoraHomeUnityManager Instance;

    private AgoraHome agoraHome;

    public GameObject friendVideoPanel;
    public GameObject yourVideoPanel;
    public Text msgText;

    public void Awake()
    {
        Instance = this;
        FindHomeManager();
    }

    private void FindHomeManager()
    {
        if (agoraHome != null)
        {
            agoraHome = GameObject.Find("AgoraManager").GetComponent<AgoraHome>();
            if (agoraHome == null)
            {
                Debug.LogError("Missing game controller...");
                return;
            }
        }
    }


    public void Leave()
    {
        agoraHome.onLeaveButtonClicked();

        Debug.Log("Exit Scene ->" + this.gameObject.scene.name);
        SceneManager.UnloadSceneAsync(this.gameObject.scene);
    }
}
