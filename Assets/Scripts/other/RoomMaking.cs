using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoomMaking : Photon.MonoBehaviour {

	public InputField field;
	bool inRoom = false;

	void Start () {
		//	魔法の呪文
		PhotonNetwork.ConnectUsingSettings ("0.1");
		if (!GameObject.Find ("StartCanvas").GetComponent<Canvas>().enabled) {
			GameStart ();
		}
	}

	// Update is called once per frame
	void Update () {

	}

	//  ランダムでルームを選び入る
	void OnJoinedLobby(){
		
		PhotonNetwork.JoinRandomRoom();
	}

	//  JoinRandomRoom()が失敗した(false)時に呼ばれる
	void OnPhotonRandomJoinFailed(){
		//  部屋に入れなかったので自分で作る
		PhotonNetwork.CreateRoom (null);
	}

	//  ルームに入れた時に呼ばれる（自分の作ったルームでも）
	void OnJoinedRoom(){
		inRoom = true;
	}

	public void GameStart(){
		if (inRoom) {
			GameObject.Find ("StartCanvas").GetComponent<Canvas> ().enabled = false;
			GameObject player = PhotonNetwork.Instantiate ("Cube", this.transform.position, this.transform.rotation, 0) as GameObject;
			player.GetComponent<PlayerScript> ().enabled = true;
			Quaternion rotation = Quaternion.Euler (90,0,0);
			GameObject Name = PhotonNetwork.Instantiate ("NAME", player.transform.position,rotation, 0) as GameObject;
			Name.GetComponent<TextMesh> ().text = field.text;
			Name.AddComponent<NameScript> ().SetTarget (player);

			GameObject MyCamera = Instantiate (Resources.Load("MyCamera"), player.transform.position,player.transform.rotation) as GameObject;
			MyCamera.GetComponent<CameraController> ().SetTarget (player);
			GameObject.Find ("StartCanvas").SetActive (false);
			//MyCamera.transform.parent = player.transform;

			//Name.transform.parent = player.transform;

		}

	}

}