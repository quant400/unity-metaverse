using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomWalletLogin: MonoBehaviour
{

    async public void OnLoginVerify(Action action)
    {
        // get current timestamp
        int timestamp = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
        // set expiration time
        int expirationTime = timestamp + 60;
        // set message
        string message = expirationTime.ToString();
        // sign message
        string signature = await Web3Wallet.Sign(message);
        // verify account
        string account = await EVM.Verify(message, signature);

        int now = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
        // validate
        if (account.Length == 42 && expirationTime >= now) 
        {
            OnSignIn(action);
           
        }
    }

    async public void OnSignIn(Action action)
    {
        try
        {
            string message = "Connecting MetaMask in CFC...";
            string response = await Web3GL.Sign(message);
            
            while (response == "")
            {
                Debug.Log(response);
                await new WaitForSeconds(1.0f);
            };

            action();

        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
        }
    }
}
