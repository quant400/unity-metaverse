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

    [Header("Start")]
    [SerializeField]
    private GameObject panelStart;
    [SerializeField]
    private GameObject panelSelection;
    [Header("Error")]
    [SerializeField]
    private GameObject panelError;
    [SerializeField]
    private TMPro.TextMeshProUGUI textError;

    private string _account;

    private CustomWalletLogin walletLogin => this.gameObject.GetComponent<CustomWalletLogin>();

    public void OnLogin()
    {
        try
        {
            Web3Connect();
            OnConnected();
        }
        catch (Exception e)
        {
            OnFailToSignIn(e.Message);
            Debug.LogException(e, this);
        }
      
    }

    async private void OnConnected()
    {
        try
        {
            _account = ConnectAccount();
            while (_account == "")
            {
                await new WaitForSeconds(1.5f);
                _account = ConnectAccount();
            };

            // reset login message
            SetConnectAccount("");
            // load next scene
            walletLogin.OnSignIn(OnEnter, OnFailToSignIn);
        }
        catch (Exception e)
        {
            OnFailToSignIn(e.Message);
        }
    
    }

    public void OnGuest()
    {
        //account0x846b257a244141ecb5c65d7c8a122a72a5564c38
        _account = "account0x846b257a244141ecb5c65d7c8a122a72a5564c38";
        OnEnter("Guest");
    }


    private void OnEnter(string contract)
    {
        try
        {
          
            DataManager.Instance.contractId = contract;
            Debug.Log("contract -> " + contract);
            DataManager.Instance.accountId = ConvertIdMetaMask(_account);
            Debug.Log("account -> " + _account);

            //Tratar o ID do metaMask
            walletLogin.GetAccount((json) => {
                DataManager.Instance.StartAccount(json, OnSuccessToSingIn, OnFailToSignIn);}
                ,OnFailToSignIn);
        }
        catch (Exception e)
        {
            OnFailToSignIn(e.Message);
        }
    }


    private string ConvertIdMetaMask(string value)
    {
        //Ex value = account0x846b257a244141ecb5c65d7c8a122a72a5564c38

        if (value.Contains("account"))
        {
            return value.Remove(0, 7); ;
        }
        else
        {
            return value;

        }      
    }


    private void OnSuccessToSingIn()
    {
        panelStart.SetActive(false);
        panelSelection.SetActive(true);
        panelError.SetActive(false);
        BGM_Manager.Instance.PlaySong();
    }

    private void OnFailToSignIn(string error)
    {
        Debug.Log(error);
        textError.text = error;
        panelStart.SetActive(true);
        panelSelection.SetActive(false);
        panelError.SetActive(true);
    }
}
#endif
