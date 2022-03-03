using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPS_Manager : MonoBehaviour
{
    public bool active = true;
    [SerializeField] private TextMeshProUGUI _fpsText;

    private float _timer;

    private float _hudRefreshRate = 1f;

    private void Update()
    {
        if (active) { 
            if (Time.unscaledTime > _timer)
            {
                int fps = (int)(1f / Time.unscaledDeltaTime);
                _timer = Time.unscaledTime + _hudRefreshRate;
                _fpsText.text = fps.ToString();
            }
        }
    }


}
