using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tutorial.CanvasManager;

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
			int index = Random.Range (0, spawnPoints.Length);

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
			 * pack[0] = id (local player id)
			 * pack[1]= name (local player name)
			 * pack[2] = avatar
			 * pack[3] = position.x (local player position x)
			 * pack[4] = position.y (local player position ...)

			*/
			
			if (!localPlayer) {

				// take a look in NetworkPlayer.cs script
				PlayerManager newPlayer;

				// newPlayer = GameObject.Instantiate( local player avatar or model, spawn position, spawn rotation)
				newPlayer = Instantiate (localPlayerPrefab,
					new Vector3(float.Parse(pack[3]), float.Parse(pack[4]),
						float.Parse(pack[5])),Quaternion.identity).GetComponent<PlayerManager> ();


				Debug.Log(string.Format("{0} instantiated", pack[1]));
				newPlayer.id = pack [0];
				//this is local player
				newPlayer.isLocalPlayer = true;

				//now local player online in the arena
				newPlayer.isOnline = true;

				//set local player's 3D text with his name
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

					// newPlayer = GameObject.Instantiate( network player avatar or model, spawn position, spawn rotation)
					newPlayer = Instantiate (networkPlayerPrefab,
						new Vector3(float.Parse(pack[3]), float.Parse(pack[4]),
							float.Parse(pack[5])),Quaternion.identity).GetComponent<PlayerManager> ();


					Debug.Log("player spawned");

					newPlayer.id = pack [0];

				
					newPlayer.isLocalPlayer = false; //it is not the local player
				
					newPlayer.isOnline = true; //set network player online in the arena

				
					newPlayer.SetCharacterName(pack[1]); //set the network player 3D text with his name

					newPlayer.gameObject.name = pack [0];
				
					networkPlayers [pack [0]] = newPlayer; //puts the network player on the list

					//setup network player skin
					newPlayer.GetComponent<Skin_Controller>().SetUpSkin(pack[2]);
				
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
			newPlayer = GameObject.Instantiate (localPlayerPrefab,
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
		public void EmitAnimation(string _animation)
		{
			//hash table <key, value>
			Dictionary<string, string> data = new Dictionary<string, string>();

			data["local_player_id"] = localPlayer.GetComponent<PlayerManager>().id;

			data ["animation"] = _animation;

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
			*/

			var pack = data.Split (Delimiter);

			//find network player by your id
			PlayerManager netPlayer = networkPlayers[pack[0]];

			//updates current animation
			netPlayer.UpdateAnimator(pack[1]);

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

				netPlayer.UpdateAnimator ("IsAttack");

			}


		}
	
	
		//sends to server player damage
		public void EmitPhisicstDamage( string _targetId)
		{

			//hash table <key, value>
			Dictionary<string, string> data = new Dictionary<string, string>();

			data ["targetId"] = _targetId;

			JSONObject jo = new JSONObject (data);

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

				if (PlayerTarget.isLocalPlayer)// if i'm a target
				{
			     
					//Levou o dano

				}



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

    }
}

