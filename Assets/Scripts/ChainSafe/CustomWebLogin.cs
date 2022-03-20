using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_WEBGL
public class CustomWebLogin : MonoBehaviour
{
    #region DLL
    [DllImport("__Internal")]
    private static extern void Web3Connect();

    [DllImport("__Internal")]
    private static extern string ConnectAccount();

    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);

    #endregion

    private int expirationTime;
    private string account;
    [SerializeField]
    private GameObject panelStart;
    [SerializeField]
    private GameObject panelSelection;

    public void OnLogin()
    {
        Web3Connect();
        OnConnected();
    }

    async private void OnConnected()
    {

        await new WaitForSeconds(1.5f);

        account = ConnectAccount();
        while (account == "") {
            await new WaitForSeconds(1.5f);
            account = ConnectAccount();
        };

        // reset login message
        SetConnectAccount("");
        // load next scene

        Debug.Log("account" + account);
        this.gameObject.GetComponent<CustomWalletLogin>().OnSignIn(OnEnter);
    }

    public void OnGuest()
    {
        OnEnter();
    }


    private void OnEnter()
    {
        panelStart.SetActive(false);
        panelSelection.SetActive(true);
    }
}
#endif
