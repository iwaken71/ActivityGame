using UnityEngine;
using System.Collections;


//Animationの設定を行うスクリプト
public class ColorSynchronizer : Photon.MonoBehaviour {



	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		// データを送る 色を同期する
		if (stream.isWriting) {

			stream.SendNext(GetComponent<Renderer>().material.color.r);

			stream.SendNext(GetComponent<Renderer>().material.color.g);

			stream.SendNext(GetComponent<Renderer>().material.color.b);

			stream.SendNext(GetComponent<Renderer>().material.color.a);

	
		} else {

			float r = (float)stream.ReceiveNext();

			float g = (float)stream.ReceiveNext();

			float b = (float)stream.ReceiveNext();

			float a = (float)stream.ReceiveNext();

			GetComponent<Renderer>().material.color = new Vector4(r, g, b, a);

		}
	}
}