using System.Collections;
using System.Collections.Generic;
using CFC.Chatt.Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatGlobal_Manager : MonoBehaviour
{

    public static ChatGlobal_Manager Instance;
    
    [SerializeField]private Message_Component _prefabGlobalMessage;
    [SerializeField] private Transform _contentGlobalChat;

    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _buttonSend;

    private List<Message_Component> _messages = new List<Message_Component>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        SetUpUI();
    }

    private void SetUpUI()
    {
        _inputField.onSubmit.AddListener((text)=>SendMessage());
        _buttonSend.onClick.AddListener(SendMessage);
    }
    
    private void SendMessage()
    {
        CFC.Multiplayer.NetworkManager.Instance.EmitMessage(
            _inputField.text, 
            "global", 
            CFC.Multiplayer.NetworkManager.Instance.local_player_id);
    }
    
    public void CreateMessage(string playerId, string message)
    {
        Message_Component messageComponent = Instantiate(_prefabGlobalMessage, _contentGlobalChat);
        _messages.Add(messageComponent);
        messageComponent.SetUp(GetPlayerNameById(playerId), message, GetColorById(playerId));
    }
    
    private string GetPlayerNameById(string playerId) => CFC.Multiplayer.NetworkManager.Instance.networkPlayers[playerId].name;
    
    private Color GetColorById(string playerId)
    {
        //TODO: Programar essa função
        return Color.cyan;
    }
}
