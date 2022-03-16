using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.Characters.ThirdPerson;

public class CheckOwner : MonoBehaviour
{
    private Tutorial.PlayerManager player;

    private ThirdPersonUserControl playerController;

    private Streamer[] _streamerList;
    
    private FreeLookCam _camera;

    void Awake()
    {
        player = GetComponent<Tutorial.PlayerManager>();
        playerController = GetComponentInChildren<ThirdPersonUserControl>();

        _streamerList = FindObjectsOfType<Streamer>();
        _camera = FindObjectOfType<FreeLookCam>();
    }

    void Start()
    {
        playerController.enabled = player.isLocalPlayer;

        if (player.isLocalPlayer)
        {
            SetStreamers();
            SetCamera();
        }
    }

    void SetStreamers()
    {
        foreach (var streamer in _streamerList)
        {
            streamer.player = transform;
        }
    }

    void SetCamera()
    {
        _camera.SetTarget(transform);
    }

}
