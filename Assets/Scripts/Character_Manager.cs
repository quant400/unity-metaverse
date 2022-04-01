﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Character_Manager : MonoBehaviour
{

    public static Character_Manager Instance;

    [SerializeField] public Character lockCharacter;
    [SerializeField] private List<Character> _characters = new List<Character>();
    [SerializeField] private Character _selectedCharacter;

    public List<Character> GetCharacters => _characters;
    public Character GetCurrentCharacter => _selectedCharacter;
    public int GetCurrentCharacterIndex => _characters.IndexOf(_selectedCharacter);

    public UnityEvent OnCharacterChanged;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void StartCharacter(List<Account> accCharacter)
    {
        GetCharacterFromResources(accCharacter);

        if (_selectedCharacter == null)
            _selectedCharacter = GetCharacter();
    }


    private void GetCharacterFromResources(List<Account> accCharacter)
    {
        try
        {
            var loaderCharacter = Resources.LoadAll("Characters", typeof(Character));
            List<Character> allCharacterAvailable = loaderCharacter.Cast<Character>().ToList();

            foreach (Account item in accCharacter)
            {
                var containCharacter = allCharacterAvailable.Where(aux => aux.Name.ToLower().Equals(item.name.ToLower())).FirstOrDefault();
                Character character;

                if (containCharacter != null)
                {
                    character = containCharacter;
                }
                else
                {
                    character = new Character()
                    {
                        Mesh = allCharacterAvailable[Random.Range(0, allCharacterAvailable.Count)].Mesh,
                        Texture = lockCharacter.Texture,
                        Name = item.name,
                        isAvailable = false
                    };
                }

                _characters.Add(character);
            }
        }
        catch (System.Exception e )
        {
            Debug.Log(e.Message);
        }
       

    }

    private Character GetCharacter()
    {
        return _characters[0];
    }

    public void ChangeCharacter(string name)
    {
        _selectedCharacter = _characters.FirstOrDefault(auxCharacter => auxCharacter.Name.Equals(name));
        OnCharacterChanged?.Invoke();
    }

}
