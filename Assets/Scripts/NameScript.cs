using UnityEngine;
using System.Collections;

public class NameScript : MonoBehaviour {

	Transform target;

	// Use this for initialization
	void Start () {
		
	
	}
	
	// Update is called once per frame
	void Update () {
		if (target) {
			this.transform.position = target.position;
		}
	}

	public void SetTarget(GameObject obj){
		target = obj.transform;
	}
}
