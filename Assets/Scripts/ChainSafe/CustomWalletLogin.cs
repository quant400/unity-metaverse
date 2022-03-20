using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomWalletLogin: MonoBehaviour
{
    async public void OnSignIn(Action onSuccess, Action onFail)
    {
        try
        {
            string message = "Connecting MetaMask in CFC...";
            string response = await Web3GL.Sign(message);

            int tries = 0;
            
            while (response == "" && tries < 10)
            {
                Debug.Log(response);
                await new WaitForSeconds(1.0f);
                tries++;
            };

            if(tries<10)
                onSuccess?.Invoke();
            else
                onFail?.Invoke();

        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
        }
    }
}
