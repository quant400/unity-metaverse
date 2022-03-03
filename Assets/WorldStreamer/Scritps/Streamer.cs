﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

/// <summary>
/// Streams async scene tiles
/// </summary>
[HelpURL ("http://indago.homenko.pl/world-streamer/")]
public class Streamer : MonoBehaviour
{
	/// <summary>
	/// Activates/deactivates streamer.
	/// </summary>
	[Tooltip ("This checkbox deactivates streamer and unload or doesn't load it's data.")]
	public bool streamerActive = true;
	
	/// <summary>
	/// The streamer tag.
	/// </summary>
	public static string STREAMERTAG = "SceneStreamer";
	[Header ("Scene Collection")]
	[Tooltip ("Drag and drop here your scene collection prefab. You could find it in catalogue with scenes which were generated by scene splitter.")]
	/// <summary>
	/// The scene collection of tiles.
	/// </summary>
	public SceneCollection sceneCollection;

	/// <summary>
	/// The splits of scene collection.
	/// </summary>
	public SceneSplit[] splits;

	[Header ("Ranges")]
	[Tooltip ("Distance in grid elements that you want hold loaded.")]
	/// <summary>
	/// The loading range of new tiles.
	/// </summary>
	public Vector3 loadingRange = new Vector3 (3, 3, 3);

	[Tooltip ("Enables ring streaming.")]
	/// <summary>
	/// The use loading minimum range.
	/// </summary>
	public bool useLoadingRangeMin = false;
	[Tooltip ("Area that you want to cutout from loading range.")]
	/// <summary>
	/// The loading minimum range.
	/// </summary>
	public Vector3 loadingRangeMin = new Vector3 (2, 2, 2);
	[Tooltip ("Distance in grid elements after which you want to unload assets.")]
	/// <summary>
	/// The deloading range of tiles.
	/// </summary>
	public Vector3 deloadingRange = new Vector3 (3, 3, 3);


	[Header ("Settings")]
	[Tooltip ("Frequancy in seconds in which you want to check if grid element is close /far enough to load/unload.")]
	/// <summary>
	/// How often streamer checks player position.
	/// </summary>
	public float positionCheckTime = 0.1f;
	[Tooltip ("Time in seconds after which grid element will be unloaded.")]
	/// <summary>
	/// Destroys unloaded tiles after seconds.
	/// </summary>
	public float destroyTileDelay = 2;
	[Tooltip ("Amount of max grid elements that you want to start loading in one frame.")]
	/// <summary>
	/// The max parallel scene loading.
	/// </summary>
	public int maxParallelSceneLoading = 1;
	[Tooltip ("Number of empty frames between loading actions.")]
	/// <summary>
	/// The async scene load wait frames.
	/// </summary>
	public int sceneLoadWaitFrames = 2;

	[Space (10)]
	[Tooltip ("If you want to fix small holes from LODs system at unity terrain borders, drag and drop object here from scene hierarchy that contains our \"Terrain Neighbours\" script.")]
	/// <summary>
	/// The terrain neighbours manager.
	/// </summary>
	public TerrainNeighbours terrainNeighbours;

	[Space (10)]
	[Tooltip ("Enable looping system, each layer is streamed independently, so if you want to synchronize them, they should have the same XYZ size. More info at manual.")]
	/// <summary>
	/// Is world looping on.
	/// </summary>
	public bool looping = false;

	[Space (10)]
	[Header ("Player Settings")]
	[Tooltip ("Drag and drop here, an object that system have to follow during streaming process.")]
	/// <summary>
	/// The player transform.
	/// </summary>
	public Transform player;

	[Tooltip ("Streamer will wait for player spawn and fill it automatically")]
	/// <summary>
	/// Streamer will wait for player spawn and fill it automatically
	/// </summary>
	public bool spawnedPlayer;

	[HideInInspector]
	public string playerTag = "Player";

	[HideInInspector]
	/// <summary>
	/// The show loading screen on start?
	/// </summary>
	public bool showLoadingScreen = true;


	[HideInInspector]
	/// <summary>
	/// The loading screen UI.
	/// </summary>
	public UILoadingStreamer loadingStreamer;

	[HideInInspector]
	public bool initialized = false;

	[HideInInspector]
	/// <summary>
	/// The tiles to load.
	/// </summary>
	public int tilesToLoad = int.MaxValue;

	[HideInInspector]
	/// <summary>
	/// The tiles loaded.
	/// </summary>
	public int tilesLoaded;

	/// <summary>
	/// Gets the loading progress.
	/// </summary>
	/// <value>The loading progress.</value>
	public float LoadingProgress {
		get{ return  (tilesToLoad > 0) ? tilesLoaded / (float)tilesToLoad : 1; }
	}

	/// <summary>
	/// The world mover.
	/// </summary>
	[HideInInspector]
	public WorldMover
		worldMover;

	[HideInInspector]
	/// <summary>
	/// The current move.
	/// </summary>
	public Vector3 currentMove = Vector3.zero;

	/// <summary>
	/// The x position.
	/// </summary>
	int xPos = 0;
	/// <summary>
	/// The y position.
	/// </summary>
	int yPos = 0;
	/// <summary>
	/// The z position.
	/// </summary>
	int zPos = 0;


	/// <summary>
	/// The scenes array.
	/// </summary>
	public Dictionary<int[],SceneSplit> scenesArray;

	[HideInInspector]
	/// <summary>
	/// The loaded scenes.
	/// </summary>
	public List<SceneSplit> loadedScenes = new List<SceneSplit> ();

	/// <summary>
	/// The currently scene loading.
	/// </summary>
	int currentlySceneLoading = 0;

	/// <summary>
	/// The scenes to load.
	/// </summary>
	List<SceneSplit> scenesToLoad = new List<SceneSplit> ();

	/// <summary>
	/// The scene load frame next.
	/// </summary>
	int sceneLoadFrameNext = 0;

	/// <summary>
	/// The scene load frames next waited.
	/// </summary>
	bool sceneLoadFramesNextWaited = false;

	//Looping variables

	int xLimity;
	int xLimitx;
	int xRange;

	int yLimity;
	int yLimitx;
	int yRange;


	int zLimity;
	int zLimitx;
	int zRange;


	static bool canUnload = true;
	static float waitTillNextUnload = 20;
	static bool unloadNext = false;

	/// <summary>
	/// Awakes this instance and resets player position;
	/// </summary>
	void Awake ()
	{
		if (spawnedPlayer) {
			player = null;
		}
		
		xPos = int.MinValue;
		yPos = int.MinValue;
		zPos = int.MinValue;

	}


	/// <summary>
	/// Start this instance, prepares scene collection into scene array, starts player position checker
	/// </summary>
	void Start ()
	{

		if (sceneCollection != null) {

			PrepareScenesArray ();

			xLimity = sceneCollection.xLimitsy;
			xLimitx = sceneCollection.xLimitsx;
			xRange = xLimity + Mathf.Abs (xLimitx) + 1;

			yLimity = sceneCollection.yLimitsy;
			yLimitx = sceneCollection.yLimitsx;
			yRange = yLimity + Mathf.Abs (yLimitx) + 1;


			zLimity = sceneCollection.zLimitsy;
			zLimitx = sceneCollection.zLimitsx;
			zRange = zLimity + Mathf.Abs (zLimitx) + 1;

			StartCoroutine (PositionChecker ());
			canUnload = true;
		} else
			Debug.LogError ("No scene collection in streamer");

	}

	int mod (int x, int m)
	{
		return (x % m + m) % m;
	}


	/// <summary>
	/// Adds the scene game object to collection
	/// </summary>
	/// <param name="sceneName">Scene name.</param>
	/// <param name="sceneGO">Scene Game object</param>
	public void AddSceneGO (string sceneName, GameObject sceneGO)
	{
		int posX = 0;
		int posY = 0;
		int posZ = 0;
		
		SceneNameToPos (sceneCollection, sceneName, out posX, out posY, out posZ);
		int[] posInt = new int[] { posX, posY, posZ };

		if (scenesArray.ContainsKey (posInt)) {
			scenesArray [posInt].sceneGo = sceneGO;

			//Debug.Log (currentMove + " " + new Vector3 (scenesArray [posInt].posXLimitMove, 0, 0));
			sceneGO.transform.position += currentMove + new Vector3 (scenesArray [posInt].posXLimitMove, scenesArray [posInt].posYLimitMove, scenesArray [posInt].posZLimitMove);

		}

		tilesLoaded++;
		currentlySceneLoading--;
		if (terrainNeighbours)
			terrainNeighbours.CreateNeighbours ();
	}

	#region update functions

	/// <summary>
	/// Update this instance, starts load level async
	/// </summary>
	void Update ()
	{
		LoadLevelAsyncManage ();
	}

	/// <summary>
	/// Manages async scene loading
	/// </summary>
	void LoadLevelAsyncManage ()
	{
		if (scenesToLoad.Count > 0 && currentlySceneLoading <= 0) {

			if (LoadingProgress < 1 || sceneLoadFramesNextWaited && sceneLoadFrameNext <= 0) {
				sceneLoadFramesNextWaited = false;
				sceneLoadFrameNext = sceneLoadWaitFrames;
				while (currentlySceneLoading < maxParallelSceneLoading && scenesToLoad.Count > 0) {
					SceneSplit split = scenesToLoad [0];

					//if (!Application.is || Application.isWebPlayer && Application.CanStreamedLevelBeLoaded (split.sceneName)) {
					scenesToLoad.Remove (split);
					currentlySceneLoading++;
					//Application.LoadLevelAdditiveAsync ();
					SceneManager.LoadSceneAsync (split.sceneName, LoadSceneMode.Additive);
					//}

				}
			} else {
				sceneLoadFramesNextWaited = true;
				sceneLoadFrameNext--;
			}
		}
	}

	/// <summary>
	/// Coroutine checks player position
	/// </summary>
	/// <returns>The checker.</returns>
	IEnumerator PositionChecker ()
	{
		while (true) {
			
			if (spawnedPlayer && player == null && !string.IsNullOrEmpty (playerTag)) {
				GameObject playerGO = GameObject.FindGameObjectWithTag (playerTag);
				if (playerGO != null)
					player = playerGO.transform;
			}
			
			if (streamerActive && player != null) {
				
				CheckPositionTiles ();
			} else if (loadedScenes.Count > 0) {
				UnloadAllScenes ();
				xPos = int.MinValue;
				yPos = int.MinValue;
				zPos = int.MinValue;

			}
				

			yield return new WaitForSeconds (positionCheckTime);
		}
	}

	/// <summary>
	/// Checks the position of player in tiles.
	/// </summary>
	public void CheckPositionTiles ()
	{
		Vector3 pos = player.position;

		pos -= currentMove;
	
		int xPosCurrent = (sceneCollection.xSize != 0) ? (int)(Mathf.FloorToInt (pos.x / sceneCollection.xSize)) : 0;
		int yPosCurrent = (sceneCollection.ySize != 0) ? (int)(Mathf.FloorToInt (pos.y / sceneCollection.ySize)) : 0;
		int zPosCurrent = (sceneCollection.zSize != 0) ? (int)(Mathf.FloorToInt (pos.z / sceneCollection.zSize)) : 0;
		if (xPosCurrent != xPos || yPosCurrent != yPos || zPosCurrent != zPos) {

			xPos = xPosCurrent;
			yPos = yPosCurrent;
			zPos = zPosCurrent;

			SceneLoading ();
			Invoke ("SceneUnloading", destroyTileDelay);

			if (worldMover != null) {
				worldMover.CheckMoverDistance (xPosCurrent, yPosCurrent, zPosCurrent);
			}
		}
	}

	#endregion

	#region loading and unloading


	/// <summary>
	/// Loads tiles in range
	/// </summary>
	void SceneLoading ()
	{
		//show splash screen
		if (showLoadingScreen && loadingStreamer != null) {
			showLoadingScreen = false;
			if (tilesLoaded >= tilesToLoad) {
				tilesToLoad = int.MaxValue;
				tilesLoaded = 0;
			}
		}


		int tilesToLoadNew = 0;

	
//		int[] sceneIDPlayer = new int[] {
//			xPos,
//			yPos,
//			zPos
//		};

		if (!useLoadingRangeMin) {
			int x = xPos;
			int y = yPos;
			int z = zPos;

			int[] sceneID = new int[] {
				x,
				y,
				z
			};
			float xMoveLimit = 0;
			int xDeloadLimit = 0;

			float yMoveLimit = 0;
			int yDeloadLimit = 0;

			float zMoveLimit = 0;
			int zDeloadLimit = 0;

			//set scene possition according to looping
			if (looping) {

				if (sceneCollection.xSplitIs) {

					int xFinal = mod ((x + Mathf.Abs (xLimitx)), xRange) + xLimitx;

					xDeloadLimit = (int)Math.Ceiling ((x - xLimity) / (float)xRange) * xRange;
					xMoveLimit = xDeloadLimit * sceneCollection.xSize;

					sceneID [0] = xFinal;
				}

				if (sceneCollection.ySplitIs) {

					int yFinal = mod ((y + Mathf.Abs (yLimitx)), yRange) + yLimitx;

					yDeloadLimit = (int)Math.Ceiling ((y - yLimity) / (float)yRange) * yRange;
					yMoveLimit = yDeloadLimit * sceneCollection.ySize;

					sceneID [1] = yFinal;
				}


				if (sceneCollection.zSplitIs) {
					int zFinal = mod ((z + Mathf.Abs (zLimitx)), zRange) + zLimitx;

					zDeloadLimit = (int)Math.Ceiling ((z - zLimity) / (float)zRange) * zRange;
					zMoveLimit = zDeloadLimit * sceneCollection.zSize;
					sceneID [2] = zFinal;
				}

			}


			//load scene if scene array contains it and set up scene offset position according to looping
			if (scenesArray.ContainsKey (sceneID)) {
				SceneSplit split = scenesArray [sceneID];
				if (!split.loaded) {
					split.loaded = true;

					split.posXLimitMove = xMoveLimit;
					split.xDeloadLimit = xDeloadLimit;

					split.posYLimitMove = yMoveLimit;
					split.yDeloadLimit = yDeloadLimit;

					split.posZLimitMove = zMoveLimit;
					split.zDeloadLimit = zDeloadLimit;

					scenesToLoad.Add (split);
					loadedScenes.Add (split);
					tilesToLoadNew++;
				}
			}

		}



		// load new scenes 
		for (int x = -(int)loadingRange.x + xPos; x <= (int)loadingRange.x + xPos; x++) {
			for (int y = -(int)loadingRange.y + yPos; y <= (int)loadingRange.y + yPos; y++) {
				for (int z = -(int)loadingRange.z + zPos; z <= (int)loadingRange.z + zPos; z++) {


					if (useLoadingRangeMin)
					if (x - xPos >= -loadingRangeMin.x && x - xPos <= loadingRangeMin.x &&
					    y - yPos >= -loadingRangeMin.y && y - yPos <= loadingRangeMin.y &&
					    z - zPos >= -loadingRangeMin.z && z - zPos <= loadingRangeMin.z) {
						continue;

					}


					int[] sceneID = new int[] {
						x,
						y,
						z
					};
					float xMoveLimit = 0;
					int xDeloadLimit = 0;

					float yMoveLimit = 0;
					int yDeloadLimit = 0;

					float zMoveLimit = 0;
					int zDeloadLimit = 0;

					//set scene possition according to looping
					if (looping) {

						if (sceneCollection.xSplitIs) {
							
							int xFinal = mod ((x + Mathf.Abs (xLimitx)), xRange) + xLimitx;

							xDeloadLimit = (int)Math.Ceiling ((x - xLimity) / (float)xRange) * xRange;
							xMoveLimit = xDeloadLimit * sceneCollection.xSize;

							sceneID [0] = xFinal;
						}

						if (sceneCollection.ySplitIs) {

							int yFinal = mod ((y + Mathf.Abs (yLimitx)), yRange) + yLimitx;

							yDeloadLimit = (int)Math.Ceiling ((y - yLimity) / (float)yRange) * yRange;
							yMoveLimit = yDeloadLimit * sceneCollection.ySize;
							sceneID [1] = yFinal;
						}


						if (sceneCollection.zSplitIs) {
							int zFinal = mod ((z + Mathf.Abs (zLimitx)), zRange) + zLimitx;

							zDeloadLimit = (int)Math.Ceiling ((z - zLimity) / (float)zRange) * zRange;
							zMoveLimit = zDeloadLimit * sceneCollection.zSize;
							sceneID [2] = zFinal;
						}

					}



					//load scene if scene array contains it and set up scene offset position according to looping
					if (scenesArray.ContainsKey (sceneID)) {
						SceneSplit split = scenesArray [sceneID];
						if (!split.loaded) {
							split.loaded = true;

							split.posXLimitMove = xMoveLimit;
							split.xDeloadLimit = xDeloadLimit;

							split.posYLimitMove = yMoveLimit;
							split.yDeloadLimit = yDeloadLimit;

							split.posZLimitMove = zMoveLimit;
							split.zDeloadLimit = zDeloadLimit;

							scenesToLoad.Add (split);
							loadedScenes.Add (split);
							tilesToLoadNew++;
						}
					}
				}
			}
		}


		tilesToLoad = tilesToLoadNew;

		initialized = true;

	}

	/// <summary>
	/// Unloads tiles out of range
	/// </summary>
	void SceneUnloading ()
	{
		
		List<SceneSplit> scenesToDestroy = new List<SceneSplit> ();
		foreach (var item in loadedScenes) {

			if (Mathf.Abs (item.posX + item.xDeloadLimit - xPos) > (int)deloadingRange.x
			    || Mathf.Abs (item.posY + item.yDeloadLimit - yPos) > (int)deloadingRange.y
			    || Mathf.Abs (item.posZ + item.zDeloadLimit - zPos) > (int)deloadingRange.x)
			if (item.sceneGo != null)
				scenesToDestroy.Add (item);

			if (useLoadingRangeMin)
			if (Mathf.Abs (item.posX + item.xDeloadLimit - xPos) <= loadingRangeMin.x &&
			    Mathf.Abs (item.posY + item.yDeloadLimit - yPos) <= loadingRangeMin.y &&
			    Mathf.Abs (item.posZ + item.zDeloadLimit - zPos) <= loadingRangeMin.z)
			if (item.sceneGo != null)
				scenesToDestroy.Add (item);
				

		}

		foreach (var item in scenesToDestroy) {

			loadedScenes.Remove (item);

			if (item.sceneGo != null) {
				Terrain childTerrain = item.sceneGo.GetComponentInChildren<Terrain> ();
				if (childTerrain) {
					GameObject childTerrainGO = childTerrain.gameObject;

					Destroy (childTerrain);
					childTerrain = null;
					Destroy (childTerrainGO);
					childTerrainGO = null;

				}
			}
		
			#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			try {
				SceneManager.UnloadSceneAsync (item.sceneGo.scene.name);
			} catch (System.Exception ex) {
				Debug.Log (item.sceneName);
				Debug.Log (item.sceneGo.name);
				Debug.Log (item.sceneGo.scene.name);
				Debug.LogError (ex.Message);
			}


			#else
			GameObject.Destroy (item.sceneGo);
			#endif

			item.sceneGo = null;
			item.loaded = false;
			
		}
		scenesToDestroy.Clear ();



		if (terrainNeighbours)
			terrainNeighbours.CreateNeighbours ();

		Streamer.UnloadAssets (this);

	}

	/// <summary>
	/// Unloads all tiles of streamer
	/// </summary>
	public void UnloadAllScenes ()
	{

	

		foreach (var item in scenesArray) {

		

			if (item.Value.sceneGo != null) {
				Terrain childTerrain = item.Value.sceneGo.GetComponentInChildren<Terrain> ();
				if (childTerrain) {
					GameObject childTerrainGO = childTerrain.gameObject;

					Destroy (childTerrain);
					childTerrain = null;
					Destroy (childTerrainGO);
					childTerrainGO = null;

				}
			

				#if UNITY_5_3 || UNITY_5_3_OR_NEWER
				try {
					SceneManager.UnloadSceneAsync (item.Value.sceneGo.scene.name);
				} catch (System.Exception ex) {
					Debug.Log (item.Value.sceneName);
					Debug.Log (item.Value.sceneGo.name);
					Debug.Log (item.Value.sceneGo.scene.name);
					Debug.LogError (ex.Message);
				}


				#else
			GameObject.Destroy (item.Value.sceneGo);
				#endif


			}
		

	
			item.Value.loaded = false;
			item.Value.sceneGo = null;
		}
		loadedScenes.Clear ();
	
		if (terrainNeighbours)
			terrainNeighbours.CreateNeighbours ();

		Streamer.UnloadAssets (this);

	}


	public static void UnloadAssets (Streamer streamer)
	{
		if (Streamer.canUnload) {
			
			Streamer.canUnload = false;

			streamer.StartCoroutine (streamer.UnloadAssetsWait ());
		} else
			unloadNext = true;
	}

	public IEnumerator UnloadAssetsWait ()
	{
		do {
			unloadNext = false;
			Resources.UnloadUnusedAssets ();
			yield return new WaitForSeconds (waitTillNextUnload);

		} while (unloadNext);
	
		canUnload = true;
	}


	#endregion

	#region prepare scene

	/// <summary>
	/// Prepares the scenes array from collection
	/// </summary>
	void PrepareScenesArray ()
	{
		scenesArray = new Dictionary<int[], SceneSplit> (new IntArrayComparer ());


		foreach (var sceneName in sceneCollection.names) {
		
			int posX = 0;
			int posY = 0;
			int posZ = 0;

			SceneNameToPos (sceneCollection, sceneName, out posX, out posY, out posZ);

			SceneSplit sceneSplit = new SceneSplit ();
			sceneSplit.posX = posX;
			sceneSplit.posY = posY;
			sceneSplit.posZ = posZ;
			sceneSplit.sceneName = sceneName.Replace (".unity", "");
			scenesArray.Add (new int[] {
				posX,
				posY,
				posZ
			}, sceneSplit);
		}
	}

	/// <summary>
	/// Converts scene name into position
	/// </summary>
	/// <param name="sceneName">Scene name.</param>
	/// <param name="posX">Position x.</param>
	/// <param name="posY">Position y.</param>
	/// <param name="posZ">Position z.</param>
	public static void SceneNameToPos (SceneCollection sceneCollection, string sceneName, out int posX, out int posY, out int posZ)
	{
		posX = 0;
		posY = 0;
		posZ = 0;

		string[] values = sceneName.Replace (sceneCollection.prefixScene, "").Replace (".unity", "").Split (new char[] {
			'_'
		}, System.StringSplitOptions.RemoveEmptyEntries);

		foreach (var item in values) {
			if (item [0] == 'x') {
				posX = int.Parse (item.Replace ("x", ""));
			}
			if (item [0] == 'y') {
				posY = int.Parse (item.Replace ("y", ""));
			}
			if (item [0] == 'z') {
				posZ = int.Parse (item.Replace ("z", ""));
			}
		}

	}

	#endregion

	void  OnDrawGizmosSelected ()
	{
		if (sceneCollection) {
			// Display the explosion radius when selected
			Gizmos.color = sceneCollection.color;
			Vector3 size = new Vector3 (sceneCollection.xSize == 0 ? 2 : sceneCollection.xSize, sceneCollection.ySize == 0 ? 2 : sceneCollection.ySize, sceneCollection.zSize == 0 ? 2 : sceneCollection.zSize);

			for (int x = -(int)loadingRange.x + xPos; x <= (int)loadingRange.x + xPos; x++) {
				for (int y = -(int)loadingRange.y + yPos; y <= (int)loadingRange.y + yPos; y++) {
					for (int z = -(int)loadingRange.z + zPos; z <= (int)loadingRange.z + zPos; z++) {
						//Debug.Log (new Vector3 (x * size.x, y * size.y, z * size.z));

						
						if (useLoadingRangeMin && x - xPos >= -loadingRangeMin.x && x - xPos <= loadingRangeMin.x &&
						    y - yPos >= -loadingRangeMin.y && y - yPos <= loadingRangeMin.y &&
						    z - zPos >= -loadingRangeMin.z && z - zPos <= loadingRangeMin.z) {

							continue;

						} else
							Gizmos.DrawWireCube (new Vector3 (x * size.x, y * size.y, z * size.z) + size * 0.5f + currentMove, size);

					}
				}
			}

			Gizmos.color = Color.green;


			Gizmos.DrawWireCube (new Vector3 (xPos * size.x, yPos * size.y, zPos * size.z) + size * 0.5f + currentMove, size);


		}
	}
}