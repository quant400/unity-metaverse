using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Character_Manager : MonoBehaviour
{

    public static Character_Manager Instance;
    
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

        GetCharacterFromResources();

        if(_selectedCharacter == null)
            _selectedCharacter = GetCharacter();
    }

    private void GetCharacterFromResources()
    {
        foreach (var character in Resources.LoadAll("Characters", typeof(Character)))
        {
            _characters.Add(character as Character);
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
