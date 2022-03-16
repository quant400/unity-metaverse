using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerManagerOnline : MonoBehaviour
{

	public string id;

	public string name;


	public bool isOnline;

	public bool isLocalPlayer;

	public Transform cameraToTarget;

	private Rigidbody rbPlayer;

	void Start()
	{
		rbPlayer = GetComponent<Rigidbody>();
	}

	// Use this for initialization
	public void Set3DName(string name)
	{
		GetComponentInChildren<TextMesh>().text = name;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (isLocalPlayer && rbPlayer.velocity.magnitude != 0)
		{
			UpdateStatusToServer();
		}
	}

	void UpdateStatusToServer()
	{
		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data["local_player_id"] = id;

		data["position"] = transform.position.x + ":" + transform.position.y + ":" + transform.position.z;

		data["rotation"] = transform.rotation.y.ToString();

		NetworkManager.instance.EmitMoveAndRotate(data);
	}



	public void UpdatePosition(Vector3 position)
	{

		transform.position = new Vector3(position.x, position.y, position.z);

	}

	public void UpdateRotation(Quaternion _rotation)
	{
		transform.rotation = _rotation;

	}

}