using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Character_Manager : MonoBehaviour
{

    public static Character_Manager Instance;
    
    [SerializeField] private List<Character> _characters;
    [SerializeField] private Character _selectedCharacter;
    
    public List<Character> GetCharacters => _characters;
    public Character GetCurrentCharacter => _selectedCharacter;
    public int GetCurrentCharacterIndex => _characters.IndexOf(_selectedCharacter);

    public UnityEvent OnCharacterChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        
        if(_selectedCharacter == null)
            _selectedCharacter = GetCharacter();
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
