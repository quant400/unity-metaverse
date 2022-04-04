using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Asn1.X509;
using UnityEngine;
using static Tutorial.CanvasManager;
using Random = UnityEngine.Random;

namespace CFC.Multiplayer
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance; //Instancia do objeto
    
        static private readonly char[] Delimiter = new char[] {':'}; 	//Variable that defines ':' character as separator
    
        [HideInInspector]
        public bool onLogged = false; //flag which is determined the player is logged in the game arena

        [HideInInspector]
        public GameObject localPlayer; //store localPlayer

        [HideInInspector]
        public string local_player_id;

        //store all players in game
        public Dictionary<string, PlayerManager> networkPlayers = new Dictionary<string, PlayerManager>();

        [Header("Local Player Prefab")]
        public GameObject localPlayerPrefab; //store the local player prefabs

        [Header("Network Player Prefab")]
        public GameObject networkPlayerPrefab; //store the remote player prefabs

        [Header("Spawn Points")]
        public Transform[] spawnPoints; //stores the spawn points

        public AgoraHome agora;

        void Awake()
        {
            Application.ExternalEval("socket.isReady = true;");
        }

        void Start()
        {
            //Cria a instancia e não permite ser destruído, ou destrói caso já exista
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
        
        #region Ping
        
        /// <summary>
        /// Prints the pong message which arrived from server.
        /// </summary>
        /// <param name="_msg">Message.</param>
        public void OnPrintPongMsg(string data)
        {

            /*
             * data.pack[0]= msg
            */

            var pack = data.Split (Delimiter);
            Debug.Log ("received message: "+pack[0] +" from server by callbackID: PONG");
        }
        
        // <summary>
        /// sends ping message to server.
        /// </summary>
        public void EmitPing() 
        {
            //hash table <key, value>
            Dictionary<string, string> data = new Dictionary<string, string>();

            //store "ping!!!" message in msg field
            data["msg"] = "ping!!!!";

            JSONObject jo = new JSONObject (data);

            //sends to the nodejs server through socket the json package
            Application.ExternalCall("socket.emit", "PING",new JSONObject(data));
        }
        
        #endregion
        
        #region Join
        
        /// <summary>
		/// Emits the player's name to server.
		/// </summary>
		/// <param name="_login">Login.</param>
		public void EmitJoin()
		{
			//hash table <key, value>
			Dictionary<string, string> data = new Dictionary<string, string>();

			//player's name
			data["name"] = instance.inputLogin.text;

			//store player's skin
	        data["avatar"] = Character_Manager.Instance.GetCurrentCharacter.Name;

	        //makes the draw of a point for the player to be spawn
	        int index = networkPlayers.Count % spawnPoints.Length; // nao usa

			//send the position point to server
			string msg = string.Empty;
			
			if(instance.inputLogin.text.Equals(string.Empty))
			{
			  int rand = Random.Range (0, 999);
			  data["name"] = "guess"+rand;
			}

			data["position"] = spawnPoints[index].position.x+":"+spawnPoints[index].position.y+":"+spawnPoints[index].position.z;

			//sends to the nodejs server through socket the json package
			Application.ExternalCall("socket.emit", "LOGIN",new JSONObject(data));

			//obs: take a look in server script.
		}

		/// <summary>
		/// Joins the local player in game.
		/// </summary>
		/// <param name="_data">Data.</param>
		public void OnJoinGame(string data)
		{
			
			Debug.Log("Login successful, joining game");
			var pack = data.Split (Delimiter);

			// the local player now is logged
			onLogged = true;

			/*
			 * pack[0] = id (network player id)
			 * pack[1]= name
			 * pack[2]= avatar
			 * pack[3] = position.x
			 * pack[4] = position.y
			 * pack[5] = position.z
			 * pack[6] = index

			*/
			
			if (!localPlayer) {

				// take a look in NetworkPlayer.cs script
				PlayerManager newPlayer;

				int getIndex = int.Parse(pack[6]);
				var newPosition = spawnPoints[getIndex].position;
				
				newPlayer = Instantiate(localPlayerPrefab,
				//new Vector3(float.Parse(pack[3]), float.Parse(pack[4]),float.Parse(pack[5]))
				newPosition, Quaternion.identity).GetComponent<PlayerManager>();


				Debug.Log(string.Format("{0} instantiated", pack[1]));
				newPlayer.id = pack [0];
				//this is local player
				newPlayer.isLocalPlayer = true;

				//now local player online in the arena
				newPlayer.isOnline = true;

				//set local player's 3D text with his name
				newPlayer.name = pack[1];
				newPlayer.SetCharacterName(pack[1]);

				//puts the local player on the list
				networkPlayers [pack [0]] = newPlayer;

				localPlayer = networkPlayers [pack[0]].gameObject;

				local_player_id =  pack [0];

				//setup local player skin
				newPlayer.GetComponent<Skin_Controller>().SetUpSkin(pack[2]);

				//hide the lobby menu (the input field and join buton)
				instance.OpenScreen(1); //Checar depois
				Debug.Log(string.Format("{0} in game", pack[1]));
			}
		}
		
		#endregion
		
		#region Spawn
		
		/// <summary>
		/// Raises the spawn player event.
		/// </summary>
		/// <param name="_msg">Message.</param>
		void OnSpawnPlayer(string data)
		{

			/*
			 * pack[0] = id (network player id)
			 * pack[1]= name
			 * pack[2]= avatar
			 * pack[3] = position.x
			 * pack[4] = position.y
			 * pack[5] = position.z
			 * pack[6] = index
			*/

			Debug.Log("received spawn network player");

			var pack = data.Split (Delimiter);

			if (onLogged ) {

				bool alreadyExist = false;

				//verify all players to avoid duplicates 
				if(networkPlayers.ContainsKey(pack [0]))
				{
					alreadyExist = true;
				}
				if (!alreadyExist) {


					PlayerManager newPlayer;
					
					int getIndex = int.Parse(pack[6]);
					Vector3 newPosition;
					if (getIndex == -1)
					{
						newPosition = new Vector3(float.Parse(pack[3]), float.Parse(pack[4]), float.Parse(pack[5]));
					}
					else
					{
						newPosition = spawnPoints[getIndex].position;
					}


					newPlayer = Instantiate(networkPlayerPrefab,
					//new Vector3(float.Parse(pack[3]), float.Parse(pack[4]),float.Parse(pack[5]))
					newPosition, Quaternion.identity).GetComponent<PlayerManager>();


					Debug.Log("player spawned");

					newPlayer.id = pack [0];

				
					newPlayer.isLocalPlayer = false; //it is not the local player
				
					newPlayer.isOnline = true; //set network player online in the arena

					newPlayer.name = pack[1];
					newPlayer.SetCharacterName(pack[1]); //set the network player 3D text with his name

					newPlayer.gameObject.name = pack [0];
				
					networkPlayers [pack [0]] = newPlayer; //puts the network player on the list

					//setup network player skin
					newPlayer.GetComponent<Skin_Controller>().SetUpSkin(pack[2]);
					
					

					newPlayer.transform.position = newPosition;
				
					Debug.Log(string.Format("{0} configured", pack[1]));

				}

			}

		}
		
		/// <summary>
	/// method to respawn  player called from client.js
	/// </summary>
	/// <param name="data">package received from server</param>
	void OnRespawPlayer(string data)
	{
		/*
		 * pack[0] = id
		 * pack[1]= name
		 * pack[2] = avatar
		 * pack[3] = position.x
		 * pack[4] = position.y
		 * pack[5] = position.z
		*/

		

		Debug.Log("Respaw successful, joining game");

		var pack = data.Split (Delimiter);

		CanvasManager.instance.OpenScreen(1);

		onLogged = true;

		if (localPlayer == null) {

			
			PlayerManager newPlayer; // take a look in PlayerManager.cs script

			// newPlayer = GameObject.Instantiate( local player avatar or model, spawn position, spawn rotation)
			newPlayer = Instantiate (localPlayerPrefab,
				new Vector3(float.Parse(pack[3]), float.Parse(pack[4]),
					float.Parse(pack[5])),Quaternion.identity).GetComponent<PlayerManager> ();


			Debug.Log("player instantiated");

			newPlayer.id = pack [0]; // set player id
			
			newPlayer.isLocalPlayer = true; //this is local player

			newPlayer.isOnline = true; //now local player online in the arena

			newPlayer.SetCharacterName(pack[1]); //set local player's 3D text with his name

			networkPlayers [pack [0]] = newPlayer; //puts the local player on the list

			localPlayer = networkPlayers [pack[0]].gameObject; // set local player variable

			local_player_id = pack [0]; // set local player id variable

			
			//setup local player skin
			newPlayer.GetComponent<Skin_Controller>().SetUpSkin(pack[2]);

			CanvasManager.instance.OpenScreen(1); //hide the lobby menu (the input field and join buton)
			
			Debug.Log(string.Format("{0} in game", pack[1]));
			
		}

	}
		
		#endregion
		
		#region Move
		
		/// <summary>
		/// send player's position and rotation to the server
		/// </summary>
		/// <param name="data"> package with player's position and rotation</param>
		public void EmitMoveAndRotate( Dictionary<string, string> data)
		{

			JSONObject jo = new JSONObject (data);

			//sends to the nodejs server through socket the json package
			Application.ExternalCall("socket.emit", "MOVE_AND_ROTATE",new JSONObject(data));


		}
		
		/// <summary>
		/// Update the network player position and rotation to local player.
		/// </summary>
		/// <param name="_msg">Message.</param>
		/// 
		void OnUpdateMoveAndRotate(string data)
		{
			/*
			 * data.pack[0] = id (network player id)
			 * data.pack[1] = position.x
			 * data.pack[2] = position.y
			 * data.pack[3] = position.z
			 * data.pack[4] = "rotation.y"
			*/

			Debug.Log("received pos and rot");
		
			var pack = data.Split (Delimiter);
		
			if (networkPlayers.ContainsKey(pack [0]))
			{
		    
				PlayerManager netPlayer = networkPlayers[pack[0]];

				//update with the new position
				netPlayer.UpdatePosition(new Vector3(
					float.Parse(pack[1]), float.Parse(pack[2]), float.Parse(pack[3])));
				
				//update new player rotation
				netPlayer.UpdateRotation(new Quaternion (netPlayer.transform.rotation.x,float.Parse(pack[4]),
					netPlayer.transform.rotation.z,netPlayer.transform.rotation.w));
		
			}
		

		}
		
		#endregion
		
		#region Animations
		
		/// <summary>
		/// Emits the local player animation to Server.js.
		/// </summary>
		/// <param name="_animation">animation's name.</param>
		public void EmitAnimation(string _animation, string _parameter)
		{
			//hash table <key, value>
			Dictionary<string, string> data = new Dictionary<string, string>();

			data["local_player_id"] = localPlayer.GetComponent<PlayerManager>().id;

			data ["animation"] = _animation;
			
			data["parameter"] = _parameter;

			JSONObject jo = new JSONObject (data);

			//sends to the nodejs server through socket the json package
			Application.ExternalCall("socket.emit", "ANIMATION",new JSONObject(data));


		}

		/// <summary>
		///  Update the network player animation to local player.
		/// </summary>
		/// <param name="data">package received from server with player id and  animation's name</param>
		void OnUpdateAnim(string data)
		{
			/*
			 * data.pack[0] = id (network player id)
			 * data.pack[1] = animation (network player animation)
			 * data.pack[2] = parameter (network animation parameter)
			*/

			var pack = data.Split (Delimiter);

			//find network player by your id
			PlayerManager netPlayer = networkPlayers[pack[0]];

			//updates current animation
			netPlayer.UpdateAnimator(pack[1], pack[2]);

		}
		
		#endregion
		
		#region Attack
		
		/// <summary>
		/// Update the network player attack animation.
		/// </summary>
		/// <param name="data">pack with remote player's attack animation.</param>
		void OnUpdateAttack(string data)
		{
			/*
			 * data.pack[0] = id (network player id)
	
			*/

			var pack = data.Split (Delimiter);

			if (networkPlayers.ContainsKey(pack [0]))
			{
				PlayerManager netPlayer = networkPlayers[pack[0]];

				//netPlayer.UpdateAnimator ("IsAttack");

			}


		}
	
	
		//sends to server player damage
		public void EmitPhisicstDamage(string _targetId)
		{
			//hash table <key, value>
			Dictionary<string, string> data = new Dictionary<string, string>();

			data ["targetId"] = _targetId;

			//sends to the nodejs server through socket the json package
			Application.ExternalCall("socket.emit", "PHISICS_DAMAGE",new JSONObject(data));
		}


		/// <summary>
		/// Update player's damage
		/// </summary>
		/// <param name="data">package received from server</param>
		void OnUpdatePlayerPhisicsDamage (string data)
		{
			/*
			 * data.pack[0] = target.id (network player id)
			 * data.pack[1] = target.health
			 */


			var pack = data.Split (Delimiter);

			if (networkPlayers.ContainsKey(pack [0]))
			{
				PlayerManager PlayerTarget = networkPlayers[pack [0]];
				
				PlayerTarget.UpdateAnimator("Play", "Hit");
				PlayerTarget.SetHP(int.Parse(pack[1]));
			}


		}

		/// <summary>
		/// Update the network player attack animation.
		/// </summary>
		/// <param name="data">pack with remote player's attack animation.</param>
		void OnPlayerDeath(string data)
		{
			/*
			 * data.pack[0] = targetid	
			*/

			var pack = data.Split (Delimiter);

			if (networkPlayers.ContainsKey(pack [0]))
			{
				networkPlayers[pack[0]].Death();
			} 
		}
		
		
		#endregion
		
		#region Disconnect
		
		/// <summary>
		/// inform the local player to destroy offline network player
		/// </summary>
		/// <param name="_msg">Message.</param>
		//desconnect network player
		void OnUserDisconnected(string data )
		{

			/*
			 * data.pack[0] = id (network player id)
			*/

			var pack = data.Split (Delimiter);

			if (networkPlayers.ContainsKey(pack [0]))
			{


				//destroy network player by your id
				Destroy( networkPlayers[pack[0]].gameObject);


				//remove from the dictionary
				networkPlayers.Remove(pack[0]);

			}

		}
		
		#endregion

		#region Chat
		
		/// <summary>
		/// Emits the player's name to server.
		/// </summary>
		/// <param name="_login">Login.</param>
		public void EmitCall(string target_id)
		{
			Dictionary<string, string> data = new Dictionary<string, string>();

			data["target_id"] = target_id;

			//sends to the nodejs server through socket the json package
			Application.ExternalCall("socket.emit", "CALL",new JSONObject(data));

			//obs: take a look in server script.
		}
		
		/// <summary>
		/// Update the network player attack animation.
		/// </summary>
		/// <param name="data">pack with remote player's attack animation.</param>
		void OnCall(string data)
		{
			/*
			 * data.pack[0] = currentid
			 * data.pack[1] = targetid 
	
			*/

			var pack = data.Split (Delimiter);

			if (networkPlayers.ContainsKey(pack [0]))
			{
				if (networkPlayers[pack[0]].isLocalPlayer)
					agora?.onJoin(false,  (pack[0] + pack[1]).Replace("-", ""));
			}
			
			if (networkPlayers.ContainsKey(pack [1]))
			{
				if (networkPlayers[pack[1]].isLocalPlayer)
					agora?.onJoin(false,  (pack[0] + pack[1]).Replace("-", ""));
			}


		}
		
		/// <summary>
		/// method to emit message to the server.
		/// </summary>
		public void EmitMessage(string _message,string _chat_box_id, string _receiver_id)
		{
			Dictionary<string, string> data = new Dictionary<string, string>();
		
			string msg = string.Empty;

			//Identifies with the name "MESSAGE", the notification to be transmitted to the server
			//data["callback_name"] = "MESSAGE";

			data["chat_box_id"] = _chat_box_id;
		
			//data ["receiver_id"] = _receiver_id;
		
			data ["message"] = _message;

			//sends to the nodejs server through socket the json package
			Application.ExternalCall("socket.emit", "MESSAGE",new JSONObject(data));
		}
		
		/// <summary>
		/// method to handle notification that arrived from the server.
		/// </summary>	
		/// <param name="data">received package from server.</param>
		void OnReceiveMessage(string data)
		{     
			/*
				 * data.pack[0] = chat_box_id
				 * data.pack[1] = writer_id
				 * data.pack[2]= message
				*/
			
			var pack = data.Split (Delimiter);

			if (pack[0].Equals("global"))
			{
				ChatGlobal_Manager.Instance.CreateMessage(pack[1], pack[2]);
			}
			else
			{
				Debug.Log("NOPE");
			}


		}
		
		#endregion
    }
}

