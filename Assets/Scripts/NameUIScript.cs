using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NameUIScript : Photon.MonoBehaviour {

	Text nameLabel;
	ScoreScript scoreScript;

	// Use this for initialization
	void Start () {
		if (photonView.isMine) {
			nameLabel = transform.Find ("Canvas").Find ("Name").GetComponent<Text> ();
			if (PhotonNetwork.playerName == "") {
				PhotonNetwork.playerName = "player"+PhotonNetwork.player.ID;
			}
			nameLabel.text = PhotonNetwork.playerName;
			scoreScript = GameObject.Find ("ScoreManager").GetComponent<ScoreScript> ();
			nameLabel.color = scoreScript.mycolor;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {
			nameLabel.color = scoreScript.mycolor;
			//nameLabel.text = PhotonNetwork.player.name;
		}
	
	}
}
