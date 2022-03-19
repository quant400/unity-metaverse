using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgoraHomeUnityManager : MonoBehaviour
{

    public static AgoraHomeUnityManager Instance;

    private AgoraHome agoraHome;

    public List<GameObject> videosSurface;
    public Text msgText;





    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
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
    }
}
