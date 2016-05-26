using UnityEngine;
using System.Collections;

public class RPCTest : Photon.MonoBehaviour {

	[PunRPC]
	void SendMessageTest(string message) {
		// 取得したメッセージを格納.
		GameObject photonG = GameObject.Find("/RPCTest");
		if (photonG == null) return;
		PhotonCloudRPCTest script = photonG.GetComponent<PhotonCloudRPCTest>();
		if (script == null) return;
		script.messageText = message;
	}

	[PunRPC]
	void GameStartMessage() {
		// 取得したメッセージを格納.
		GameObject photonG = this.gameObject;
		if (photonG == null) return;
		GameManager script = photonG.GetComponent<GameManager>();
		if (script == null) return;
		script.GameStart ();

	}
	[PunRPC]
	void SendScore() {
		// 取得したメッセージを格納.
		GameObject photonG = this.gameObject;
		if (photonG == null) return;
		GameManager script = photonG.GetComponent<GameManager>();
		if (script == null) return;
		script.GameStart ();

	}

	[PunRPC]
	void SendColor(Color color) {
		// 取得したメッセージを格納.
		GameObject photonG = this.gameObject;
		if (photonG == null) return;
		GameManager script = photonG.GetComponent<GameManager>();
		if (script == null) return;

	}


}
