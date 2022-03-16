using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.Characters.ThirdPerson;

namespace CFC.Multiplayer
{
    public class PlayerManager : MonoBehaviour
    {
        public string	id;

        public string name;

        public string avatar;

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
            myTPUserControlr.enabled = isLocalPlayer;

            if (!isLocalPlayer) return;
            
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
            if (IsMoving)
            {
                if (IsJumping)
                {
                    UpdateAnimator("IsJump");
                }
                else
                {
                    UpdateAnimator("IsWalk");
                }

                if (isLocalPlayer)
                {
                    UpdateStatusToServer();
                }
            }
            else
            {
                UpdateAnimator("IsIdle");
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
        
        public void UpdateAnimator(string _animation)
        {
            switch (_animation) { 
                case "IsWalk":
                    if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Walk"))
                    {
                        myAnim.SetTrigger ("IsWalk");
                    }
                    break;

                case "IsIdle":

                    if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                    {
                        myAnim.SetTrigger ("IsIdle");
                    }
                    break;
                
                case "IsJump":

                    if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Jump")
                    || !myAnim.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
                    {
                        myAnim.SetTrigger ("IsJump");
                    }
                    break;

                case "IsDamage":
                    if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Damage") ) 
                    {
                        myAnim.SetTrigger ("IsDamage");
                    }
                    break;

                case "IsAttack":
                    if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                    {  
				
                        myAnim.SetTrigger ("IsAttack");
	
                        
                        Debug.Log("Ver isso depois");
                        if (!isLocalPlayer)
                        {
			
                            StartCoroutine ("StopAttack");
                        }
                    }
                    break;
                
            }
        }
		
        public void SetCharacterName(string name)
        {
            GetComponentInChildren<TextMesh> ().text = name;
        }
    }
}

