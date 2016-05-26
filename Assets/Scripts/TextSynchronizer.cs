using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Animationの設定を行うスクリプト
public class TextSynchronizer : Photon.MonoBehaviour {

	Text nameLabel;

	void Start(){
		nameLabel = transform.Find ("Canvas").Find ("Name").GetComponent<Text> ();
	}



	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (nameLabel == null) {
			nameLabel = transform.Find ("Canvas").Find ("Name").GetComponent<Text> ();
		}
		// データを送る 色を同期する
		if (stream.isWriting) {

			stream.SendNext(nameLabel.color.r);

			stream.SendNext(nameLabel.color.g);

			stream.SendNext(nameLabel.color.b);

			stream.SendNext(nameLabel.color.a);

			stream.SendNext (nameLabel.text);


		} else {

			float r = (float)stream.ReceiveNext();

			float g = (float)stream.ReceiveNext();

			float b = (float)stream.ReceiveNext();

			float a = (float)stream.ReceiveNext();

			string message = stream.ReceiveNext().ToString();

			nameLabel.color = new Color(r, g, b, a);
			nameLabel.text = message;

		}
	}
}