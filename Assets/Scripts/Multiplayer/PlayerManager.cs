using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.Characters.ThirdPerson;

namespace CFC.Multiplayer
{
    public class PlayerManager : MonoBehaviour
    {

        [Header("Player")]

        public string id;

        public string name;

        public string avatar;

        public Color color;

        public bool isOnline;
        
        public bool isLocalPlayer;
        
        private Animator myAnim;
        private Rigidbody myRigidbody;
        private ThirdPersonCharacter myTPCharacter;
        private ThirdPersonUserControl myTPUserControlr;
        private FreeLookCam myCamera;
        private Streamer[] myStreamers;

        private bool IsMoving => myTPUserControlr.m_Move.magnitude != 0;
        private bool IsJumping => !myTPCharacter.m_IsGrounded;


        [Header("Name")]
        [SerializeField] private TMP_Text myName_Text;
        [Header("HP")]
        [Range(0.0f, 1.0f)] public float myHP_Percent = 1.0f;
        [SerializeField] private Image myHP_Bar;
        [SerializeField] private TMP_Text myHP_Text;

        public void SetHP(float value)
        {
            myHP_Percent = (value/100);
            myHP_Bar.fillAmount = myHP_Percent;
            myHP_Text.text = (value) + "%";
        }


        void Start()
        {
            myRigidbody = GetComponent<Rigidbody>();
            myAnim = GetComponent<Animator>();
            myTPCharacter = GetComponent<ThirdPersonCharacter>();
            myTPUserControlr = GetComponent<ThirdPersonUserControl>();
            myCamera = FindObjectOfType<FreeLookCam>();
            myStreamers = FindObjectsOfType<Streamer>();

            SetUpLocalPlayer();
        }

        void SetUpLocalPlayer()
        {

            if (NetworkManager.Instance == null) return;
            
            myTPUserControlr.enabled = isLocalPlayer;

            do
            {
                color = Color_Manager.pallete.RandomPlayerColor();
            } while (!NetworkManager.Instance.networkPlayers.Any(player => player.Value.color == color));

            //Nome olhar para camera
            GetComponentInChildren<BillboardFX>().camTransform = myCamera.transform;

            if (!isLocalPlayer)
            {
                myAnim.applyRootMotion = false;
                return;
            }
            
            //Seta alvo da camera
            myCamera.SetTarget(transform);
            myTPUserControlr.SetCamera(myCamera.GetComponentInChildren<Camera>().transform);

            //Seta player nos assets em loop
            foreach (var myStreamer in myStreamers)
            {
                myStreamer.player = transform;
            }
        }

        void FixedUpdate()
        {
            if (IsMoving||IsJumping)
            {
                if (isLocalPlayer)
                {
                    UpdateStatusToServer();
                }
            }
        }
        
        void UpdateStatusToServer ()
        {
            //hash table <key, value>
            Dictionary<string, string> data = new Dictionary<string, string>();

            data["local_player_id"] = id;

            data["position"] = transform.position.x+":"+transform.position.y+":"+transform.position.z;

            data["rotation"] = transform.rotation.y.ToString();

            NetworkManager.Instance.EmitMoveAndRotate(data);
        }

        public void UpdatePosition(Vector3 position)
        {
            if (!isLocalPlayer) 
            {
                transform.position = new Vector3 (position.x, position.y, position.z);
            }
        }

        public void UpdateRotation(Quaternion _rotation)
        {
            if (!isLocalPlayer)
            {
                transform.rotation = _rotation;
            }
        }
        
        public void UpdateAnimator(string _animation, string _parameter)
        {
            if (_animation.Equals("AnimatorSpeed"))
            {
                myAnim.speed = float.Parse(_parameter);
                return;
            }

            if(_animation.Equals("Forward") ||
               _animation.Equals("Turn") ||
               _animation.Equals("Jump") ||
               _animation.Equals("JumpLeg"))
            {
                myAnim.SetFloat(_animation, float.Parse(_parameter));
                return;
            }

            if (_animation.Equals("Crouch") ||
                _animation.Equals("OnGround"))
            {
                myAnim.SetBool(_animation, Boolean.Parse(_parameter));
                return;
            }

            if (_animation.Equals("Play"))
            {
                myAnim.Play(_parameter);
                return;
            }
        }

        public void Death()
        {
            Debug.Log("E morreu");
            UpdateAnimator("Play", "Death");
            myTPUserControlr.isDead = true;
        }

        public void Revive()
        {
            
        }

        public void SetCharacterName(string name)
        {
            myName_Text.text = name;
        }
    }
}

