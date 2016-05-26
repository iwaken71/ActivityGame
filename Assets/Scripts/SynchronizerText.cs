using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SynchronizerText : Photon.MonoBehaviour {

	NameManager manager;
	string Sender = "";
	ScoreScript script;
	string messageName = "";
	Text label;
	float timer = 0;
	bool push = false;

	// Use this for initialization
	void Start () {
		manager = GameObject.Find ("NameManager").GetComponent<NameManager>();
		script = GameObject.Find ("ScoreManager").GetComponent<ScoreScript>();
		label = GetComponent<Text> ();
		Sender = manager.GetName ();

	}

	
	// Update is called once per frame
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			if (push) {
				stream.SendNext (Sender);

				stream.SendNext (script.mycolor.r);

				stream.SendNext (script.mycolor.g);

				stream.SendNext (script.mycolor.b);

				stream.SendNext (script.mycolor.a);
				push = false;
				Debug.Log (Sender);
			}

		} else {
			string str = (string)stream.ReceiveNext ();
			if (str != "") {
				messageName = str;
				timer = 3;
			}

			float r = (float)stream.ReceiveNext();

			float g = (float)stream.ReceiveNext();

			float b = (float)stream.ReceiveNext();

			float a = (float)stream.ReceiveNext();
			if (str != "") {
				GetComponent<Text>().color = new Vector4(r, g, b, a);
			}


		}
	
	}

	void Update(){
		timer -= Time.deltaTime;
		if(messageName != ""){
			label.text = messageName + "says NICE!";
			if (timer < 0) {
				messageName = "";
			}
		}else{
			label.text = "";
		}
		if (Input.GetKeyDown (KeyCode.N)) {
			PushNice ();
		}
	}

	public void PushNice(){
		push = true;
		Sender = "";
		Sender = manager.name;
	}
}
