﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class PlatformerExampleLogic : MonoBehaviour {

	public GameObject playerPrefab;

	public Dictionary<int, PlayerController> players = new Dictionary<int, PlayerController> (); 

public float spawnHeight = 2f;

	void Awake () {
		AirConsole.instance.onMessage += OnMessage;		
		AirConsole.instance.onReady += OnReady;		
		AirConsole.instance.onConnect += OnConnect;		
	}

	void OnReady(string code){
		//Since people might be coming to the game from the AirConsole store once the game is live, 
		//I have to check for already connected devices here and cannot rely only on the OnConnect event 
		List<int> connectedDevices = AirConsole.instance.GetControllerDeviceIds();
		foreach (int deviceID in connectedDevices) {
			AddNewPlayer (deviceID);
		}
	}

	void OnConnect (int device){
		AddNewPlayer (device);
	}

	private void AddNewPlayer(int deviceID){

		if (players.ContainsKey (deviceID)) {
			return;
		}

		//Instantiate player prefab, store device id + player script in a dictionary
		GameObject newPlayer = Instantiate (playerPrefab, transform.position + new Vector3(0,spawnHeight,0), transform.rotation) as GameObject;
		players.Add(deviceID, newPlayer.GetComponent<PlayerController>());
	}

	void OnMessage (int from, JToken data){
		Debug.Log ("message: " + data);

		//When I get a message, I check if it's from any of the devices stored in my device Id dictionary
		if (players.ContainsKey (from) && data["action"] != null) {
			//I forward the command to the relevant player script, assigned by device ID
			players [from].ButtonInput (data["action"].ToString());
		}
	}

	void OnDestroy () {
		if (AirConsole.instance != null) {
			AirConsole.instance.onMessage -= OnMessage;		
			AirConsole.instance.onReady -= OnReady;		
			AirConsole.instance.onConnect -= OnConnect;		
		}
	}
}
