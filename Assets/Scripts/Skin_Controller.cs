using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin_Controller : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _meshRenderer;

    void OnDisable()
    {
        Character_Manager.Instance.OnCharacterChanged.RemoveListener(SetUpSkin);
    }

    void Start()
    {
        Character_Manager.Instance.OnCharacterChanged.AddListener(SetUpSkin);
        SetUpSkin();
    }

    public void SetUpSkin()
    {
        _meshRenderer.sharedMesh = Character_Manager.Instance.GetCurrentCharacter.Mesh;
        _meshRenderer.material.mainTexture = Character_Manager.Instance.GetCurrentCharacter.Texture;
    }
}
