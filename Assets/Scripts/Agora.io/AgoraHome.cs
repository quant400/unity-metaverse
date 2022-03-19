using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
#if(UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif
using System.Collections;

/// <summary>
///    TestHome serves a game controller object for this application.
/// </summary>
public class AgoraHome : MonoBehaviour
{

    // Use this for initialization
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList();
#endif
    public static AgoraHomeUnityVideo app = null;
    [SerializeField]
    private string PlaySceneName = "AgoraHomeUnityVideo";

    // PLEASE KEEP THIS App ID IN SAFE PLACE
    // Get your own App ID at https://dashboard.agora.io/
    [SerializeField]
    private string AppID = "603f9b1a7dbc49409d6f6a225e106cbe";
    [SerializeField]
    private string ChannelName = "unity3d";


    void Awake()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
		permissionList.Add(Permission.Microphone);         
		permissionList.Add(Permission.Camera);               
#endif
        // keep this alive across scenes
        DontDestroyOnLoad(this.gameObject);

    }

    void Update()
    {
        CheckPermissions();
    }

    /// <summary>
    ///   Checks for platform dependent permissions.
    /// </summary>
    private void CheckPermissions()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        foreach(string permission in permissionList)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {                 
				Permission.RequestUserPermission(permission);
			}
        }
#endif
    }


    public void onJoin()
    {
        onJoinButtonClicked(true);
    }

    private void onJoinAudience()
    {
        // create app if nonexistent
        if (ReferenceEquals(app, null))
        {
            app = new AgoraHomeUnityVideo(); // create app
            app.loadEngine(AppID); // load engine
        }

        app.joinAudience(ChannelName);
        SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
        SceneManager.LoadScene(PlaySceneName, LoadSceneMode.Single);
    }

    private void onJoinButtonClicked(bool enableVideo, bool muted = false)
    {
        // create app if nonexistent
        if (ReferenceEquals(app, null))
        {
            app = new AgoraHomeUnityVideo(); // create app
            app.loadEngine(AppID); // load engine
        }

        // join channel and jump to next scene
        app.join(ChannelName, enableVideo, muted);
        
        
        SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
        SceneManager.LoadScene(PlaySceneName, LoadSceneMode.Single);
    }

    public void onLeaveButtonClicked()
    {
        if (!ReferenceEquals(app, null))
        {
            app.leave(); // leave channel
            app.unloadEngine(); // delete engine
            app = null; // delete app
            SceneManager.UnloadSceneAsync(PlaySceneName);
        }
        //Destroy(gameObject);
    }

    public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == PlaySceneName)
        {
            if (!ReferenceEquals(app, null))
            {
                app.onSceneHelloVideoLoaded(); // call this after scene is loaded
            }
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }
    }

    void OnApplicationPause(bool paused)
    {
        if (!ReferenceEquals(app, null))
        {
            app.EnableVideo(paused);
        }
    }

    void OnApplicationQuit()
    {
        if (!ReferenceEquals(app, null))
        {
            app.unloadEngine();
        }
    }
}
