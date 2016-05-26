using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	GameObject player;
	float offset,distance;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (player) {
			transform.position = player.transform.position + new Vector3 (0, 3, -5);
			transform.LookAt (player.transform);
		}
	}

	public void SetTarget(GameObject obj){
		player = obj;
	}
}
