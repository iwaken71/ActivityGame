using UnityEngine;
using System.Collections;

public class PhotonCloudRPCTest : Photon.MonoBehaviour {
	private PhotonView m_photonView = null;
	public string messageText = "";


	// Use this for initialization
	void Start () {
		m_photonView = this.GetComponent<PhotonView>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnJoinedRoom() {
		m_photonView = this.GetComponent<PhotonView>();
	}
	/**
     * RPCで遠隔の関数を呼ぶ.
     */
	private void SendRPC(string str) {
		if (m_photonView != null) {
			m_photonView.RPC("SendMessageTest", PhotonTargets.All, str);
		}
	}

	void OnGUI() {
		if (GUILayout.Button("Quit")) { 
			Application.Quit();
			return;
		}

		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

		if (GUILayout.Button("RPC Test")) {
			// 送信するメッセージテキスト.
			string playerName = PhotonNetwork.playerName;
			string playerID   = (PhotonNetwork.player.ID).ToString();
			string str        = playerName + " / " + playerID;

			SendRPC(str);
			return;
		}
		if (GUILayout.Button("Clear Message")) {
			SendRPC("");
			return;
		}       
		GUILayout.Label("message : " + messageText);
	}
}
