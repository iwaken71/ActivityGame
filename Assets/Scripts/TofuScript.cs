using UnityEngine;
using System.Collections;

public class TofuScript : Photon.MonoBehaviour {

	public float myColorNumber;
	ScoreScript script;
	Rigidbody rb;
	//BoxCollider _collider;
	Vector3 okiba = new Vector3(0,-1000,0);
	// Use this for initialization
	void Start () {
		if (photonView.isMine) {
			script = GameObject.Find ("ScoreManager").GetComponent<ScoreScript> ();
			myColorNumber = script.mycolor.r + script.mycolor.b * 10 + script.mycolor.g * 100;
			rb = GetComponent<Rigidbody> ();
			//_collider = GetComponent<BoxCollider> ();
			//rb.useGravity = true;
			//_collider.enabled = true;
		}
	}
	
	// Update is called once per frame
	void Update () {

	
	}
	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "Stage") {
			// カウントダウン中はしない
			if (GameManager.GetInstance ().GetState () == 2) {
				col.gameObject.GetComponent<Renderer> ().material.color = this.GetComponent<Renderer> ().material.color;
			}
			//Destroy (this.gameObject,0.05f);
			Destroy2();
		}

		if (col.gameObject.tag != "Tofu") {
			//Invoke ("Destroy2",2.0f);
			//Destroy (this.gameObject, 0.4f);
		}
	}

	public Rigidbody GetRb(){
		return rb;
	}
	void Destroy2(){
		
		//GetComponent<Rigidbody> ().useGravity = false;
		//_collider.enabled = false;
		transform.position = okiba;
		//GetComponent<Rigidbody> ().velocity = Vector3.zero;
	}
}
