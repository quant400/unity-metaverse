using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        public Vector3 m_Move;
        public bool isDead = false;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        private bool m_Punch;
        private bool m_Kick;
        private bool m_Call;
        

        
        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
            
            currentEventSys = EventSystem.current;
        }

        public void SetCamera(Transform camera)
        {
            m_Cam = camera;
        }

        private EventSystem currentEventSys = EventSystem.current;
        
        private void Update()
        {
            if (isDead) return;
            
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
            
            m_Punch = Input.GetMouseButtonDown(0);
            m_Kick = Input.GetMouseButtonDown(1);

            if (m_Punch)
            {
                m_Character.Punch();
                m_Punch = false;
            }


            if (m_Kick)
            {
                m_Character.Kick();
                m_Kick = false;
            }
            
            if (Input.GetKeyDown(KeyCode.V))
            {
                m_Character.CheckCall();
            }
        }
        
        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            if (isDead) return;
            
            // read inputs
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);
            
            
    

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v*m_CamForward + h*m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v*Vector3.forward + h*Vector3.right;
            }
#if !MOBILE_INPUT
			// walk speed multiplier
	        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            if (currentEventSys?.currentSelectedGameObject?.GetComponent<TMP_InputField>() != null)
            {
                m_Move = Vector3.zero;
                crouch = false;
                m_Jump = false;
                m_Punch = false;
                m_Kick = false;
            }

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            
            m_Punch = false;
            m_Kick = false;
            m_Jump = false;
        }
    }
    
    
}
