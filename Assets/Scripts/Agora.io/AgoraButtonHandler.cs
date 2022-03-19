using UnityEngine;
using UnityEngine.UI;

public class AgoraButtonHandler : MonoBehaviour
{


    private AgoraHome agoraHome;

    public void Awake()
    {
        FindManager();
    }


    private void FindManager()
    {
        if (agoraHome != null)
        {
            AgoraHome gameController = GameObject.Find("AgoraManager").GetComponent<AgoraHome>();
            if (gameController == null)
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
