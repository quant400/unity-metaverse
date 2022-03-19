using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CFC
{
    public class ChatBox_Manager : MonoBehaviour
    {
        public static ChatBox_Manager Instance;

        [Header("Components")]
        [SerializeField] private Transform _chatContent;
        [SerializeField] private Message_Component _prefabMessage;
        
        [Header("UI")]
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Button _sendButton;

        private List<Chat> _chats;

        public enum ChatType
        {
            Writer,
            Receiver
        };
    
        void Awake()
        {
            
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            SetUpUI();
        }

        private void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                string receiver_id = Multiplayer.NetworkManager.Instance.networkPlayers.Last().Value.id;
                Multiplayer.NetworkManager.Instance.EmitOpenChatBox(receiver_id);
            }
        }

        #region UI

        private void SetUpUI()
        {
            _inputField.onSubmit.AddListener((text)=>OnMessageSend());
            _sendButton.onClick.AddListener(OnMessageSend);
        }

        private void OnMessageSend()
        {
            CreateMessage("MeuId", _inputField.text, ChatType.Writer);
        }
        
        private void OnMessageReceived(string playerId, string message)
        {
            CreateMessage(playerId, message, ChatType.Receiver);
        }
        
        public void CreateMessage(string playerId, string message, ChatType chatType)
        {
            Message_Component messageComponent = Instantiate(_prefabMessage, _chatContent);
            messageComponent.SetUp(GetPlayerNameById(playerId), message, chatType, GetColorById(playerId));
        }
        
        #endregion

        #region Utils

        private string GetPlayerNameById(string playerId)
        {
            //TODO: Programar essa função
            return "Test";
        }
    
        private Color GetColorById(string playerId)
        {
            //TODO: Programar essa função
            return Color.cyan;
        }
        
        #endregion
        
    }
}


