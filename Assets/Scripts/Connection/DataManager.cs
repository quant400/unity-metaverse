using System;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public string accountId;
    public string contractId;

    [SerializeField]
    private Account userAccount;

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void SetAccount(Account tempAccount)
    {
        if (userAccount == null || userAccount.id == 0)
        {
            userAccount = tempAccount;
        }
        else
        {
            Debug.Log("Unable to change account");
        }
    }

    public Account GetAccount()
    {
        return userAccount;
    }

    public void ResetAccount()
    {
        userAccount = null;
    }

    public void StartAccount(string json,Action success, Action<string> error)
    {
        Debug.Log("Starting the account");
        try
        {
            Debug.Log(json);

            var tempAccounts = JsonUtility.FromJson<RootAccount>(json);

            if (tempAccounts.accounts.Count > 0)
            {
                SetAccount(tempAccounts.accounts.FirstOrDefault());
                success();
            }
        }
        catch (System.Exception e)
        {
            error(e.Message);
        }
      
    }

}


