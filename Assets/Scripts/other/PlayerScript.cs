using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	Rigidbody rb;
	public float speed = 10.0f;
	float forward,right;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	
	}
	
	// Update is called once per frame
	void Update () {
		forward = Input.GetAxis ("Vertical");
		right = Input.GetAxis ("Horizontal");
		transform.Translate (Vector3.forward * forward*speed*Time.deltaTime);
		transform.Rotate (new Vector3(0,right,0));
		//transform.position += move * speed * Time.deltaTime;


	
	}
	void FixedUpdate(){
		//rb.AddForce(move* speed);
		if (Input.GetKeyDown(KeyCode.Space)) {
			Ray ray = new Ray (transform.position, Vector3.down);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 0.51f)) {
				rb.velocity = new Vector3 (rb.velocity.x, 20, rb.velocity.z);
			}
		}

	}
}



	   