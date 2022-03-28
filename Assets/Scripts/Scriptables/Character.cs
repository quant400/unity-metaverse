using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "NFT/New Character")]
public class Character : ScriptableObject
{
    public string Name;
    public Mesh Mesh;
    public Texture Texture;
    public bool isAvailable = true;
}
