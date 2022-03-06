using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLock : MonoBehaviour
{

    [SerializeField] private CursorLockMode currentLockMode;
    
    void Start()
    {
        Cursor.lockState = currentLockMode;
    }
}
