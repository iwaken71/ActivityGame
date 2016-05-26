using UnityEngine;
using System.Collections;

public class AIScript : MonoBehaviour {

	Vector3 move;
	float timer = 0;

	// Use this for initialization
	void Start () {
		move = Vector3.zero;
	
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		transform.position += move * 1 * Time.deltaTime;

		if (timer > 3) {
			Action ();
			timer = 0;
		}
	
	}

	void Action(){
		int n = Random.Range (0, 10);
		switch (n) {
		case 1:
			move = new Vector3 (0, 0, 1);
			break;
		case 2:
			move = new Vector3 (0, 0, -1);
			break;
		case 3:
			move = new Vector3 (1, 0, 0);
			break;
		case 4:
			move = new Vector3 (-1, 0, 0);
			break;
		case 5:
			move = new Vector3 (1, 0, 1);
			break;
		case 6:
			move = new Vector3 (-1, 0, 1);
			break;
		case 7:
			move = new Vector3 (1, 0, -1);
			break;
		case 8:
			move = new Vector3 (-1, 0, -1);
			break;
		default:
			break;
			
			
		}
	}
}
