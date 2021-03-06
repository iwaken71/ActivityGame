﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomMaking2 : Photon.MonoBehaviour {

	//	public InputField field;
	//bool inRoom = false; //roomに入ったかどうか
	//public static int ID; //PlayerのID
	private bool isStart = false;
	string name = "player"; //名前
	//public GameObject loadingPanel; //loadingPanel

	void Start () {
		PhotonNetwork.ConnectUsingSettings ("v0.1");
	}

	// Update is called once per frame
	void Update () {

	}

	public string GetName(){
		return name;
	}

	//  ランダムでルームを選び入る
	void OnJoinedLobby(){
		if (SceneManager.GetActiveScene ().name == "Unitychan") {
			PhotonNetwork.JoinRandomRoom ();
		}
	}

	//  JoinRandomRoom()が失敗した(false)時に呼ばれる
	void OnPhotonRandomJoinFailed(){
		PhotonNetwork.CreateRoom (null);
	}

	//  ルームに入れた時に呼ばれる（自分の作ったルームでも）
	void OnJoinedRoom(){
		GameStart ();
	}

	public void GameStart(){
		if (PhotonNetwork.inRoom) {
			Vector3 spawnPosition = new Vector3 (Random.Range (-10, 10), 5, Random.Range (-10, 10)); //生成位置

			// playerの生成、場所はランダム
			GameObject player = PhotonNetwork.Instantiate ("unitychanPrefab", spawnPosition, this.transform.rotation, 0) as GameObject;

			// ScriptをOn
			player.GetComponent<UnityChanControlScriptWithRgidBody> ().enabled = true;

			// Playerを追いかけるカメラを生成、同期はしない
			GameObject camera = Instantiate (Resources.Load("Camera1"), spawnPosition, this.transform.rotation) as GameObject;

			// cameraのスクリプトを取得、設定
			ThirdPersonCamera2 cameraScript = camera.GetComponent<ThirdPersonCamera2> ();
			cameraScript.jumpPos = player.transform.FindChild ("JumpPos").transform;
			cameraScript.standardPos = player.transform.FindChild ("CamPos").transform;
			cameraScript.frontPos = player.transform.FindChild ("FrontPos").transform;


			//GameObject headCube =  PhotonNetwork.Instantiate ("HeadCube", spawnPosition, this.transform.rotation, 0) as GameObject;
			//headCube.transform.parent = player.transform;
			//headCube.GetComponent<ColorScript> ().player = player.transform;
			//headCube.transform.position = player.transform.position + new Vector3 (0, 1.55f + headCube.transform.localScale.x * 0.5f, 0);


			//Quaternion rotation = Quaternion.Euler (90,0,0);
			//GameObject Name = PhotonNetwork.Instantiate ("NAME", player.transform.position,rotation, 0) as GameObject;
			////////////Name.GetComponent<TextMesh> ().text = field.text;
			//Name.AddComponent<NameScript> ().SetTarget (player);

			//GameObject MyCamera = Instantiate (Resources.Load("MyCamera"), player.transform.position,player.transform.rotation) as GameObject;
			//MyCamera.GetComponent<CameraController> ().SetTarget (player);
			//GameObject.Find ("StartCanvas").SetActive (false);
			//MyCamera.transform.parent = player.transform;

			//Name.transform.parent = player.transform;
		}
	}



	void OnGUI() {
		// Photon接続状態
		if (SceneManager.GetActiveScene ().name == "Start") {
			GUILayout.Label ("対戦待ち" + PhotonNetwork.countOfPlayersOnMaster.ToString () + "人");
			GUILayout.Label ("対戦中" + PhotonNetwork.countOfRooms.ToString () + "人");
		}
	}


}