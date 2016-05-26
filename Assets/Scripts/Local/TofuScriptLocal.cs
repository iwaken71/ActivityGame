using UnityEngine;
using System.Collections;

public class TofuScriptLocal : MonoBehaviour {

	public float myColorNumber;
	ScoreScriptLocal script;
	Rigidbody rb;
	//BoxCollider _collider;
	Vector3 okiba = new Vector3(0,-1000,0);
	// Use this for initialization
	void Start () {
		script = GameObject.Find ("ScoreManager").GetComponent<ScoreScriptLocal> ();
		myColorNumber = script.mycolor.r + script.mycolor.b * 10 + script.mycolor.g * 100;
		rb = GetComponent<Rigidbody> ();

	}

	// Update is called once per frame
	void Update () {


	}
	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "Stage") {
			col.gameObject.GetComponent<Renderer> ().material.color = this.GetComponent<Renderer> ().material.color;
			//Destroy (this.gameObject,0.05f);
			Destroy2();
		}
		if (col.gameObject.tag != "Tofu") {
			Invoke ("Destroy2",0.4f);
			//Destroy (this.gameObject, 0.4f);
		}
	}

	public Rigidbody GetRb(){
		return rb;
	}
	void Destroy2(){
		transform.position = okiba;
	}
}
