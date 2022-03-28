using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CFC.Multiplayer;

public class Skin_Controller : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _meshRenderer;

    private bool _isLocalPlayer;
    

    void Awake()
    {
        var playerManager = GetComponent<PlayerManager>();
        _isLocalPlayer = playerManager == null || playerManager.isLocalPlayer;
    }

    void OnDisable()
    {
        if (_isLocalPlayer)
        {
            Character_Manager.Instance?.OnCharacterChanged.RemoveListener(SetUpSkin);
        }

    }

    void Start()
    {
        if (_isLocalPlayer)
        {
            Character_Manager.Instance?.OnCharacterChanged.AddListener(SetUpSkin);
        }
    }

    public void SetUpSkin()
    {
        ChangeSkin(Character_Manager.Instance.GetCurrentCharacter);
    }
    
    public void SetUpSkin(string skinName)
    {
        var currentCharacter = Character_Manager.Instance.GetCharacters.FirstOrDefault(
            auxChar => auxChar.Name.ToLower().Equals(skinName.ToLower()));

        if (currentCharacter != null)
        {
            ChangeSkin(currentCharacter);
        }
    }

    public void ChangeSkin(Character character)
    {
        if(character != null)
        {
            _meshRenderer.sharedMesh = character.Mesh;
            _meshRenderer.material.mainTexture = character.Texture;
        }
        else
        {
            _meshRenderer.sharedMesh = Character_Manager.Instance.lockCharacter.Mesh;
            _meshRenderer.material.mainTexture = Character_Manager.Instance.lockCharacter.Texture;
        }
       
    }
}
