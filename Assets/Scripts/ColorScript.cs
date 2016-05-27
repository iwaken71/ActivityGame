using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ColorScript : Photon.MonoBehaviour {

	Renderer _renderer;
	GameObject tofu;
	ScoreScript script;
	float timer = 0;
	float interval = 1.5f;
	int rensya = 1;
	float term = 0.05f;
	public Transform player;
	AudioSource audiosource;
	public AudioClip[] clips;
	int max = 20;
	GameObject[] tofus;//豆腐を予め作っておく
	TofuScript[] tofuScript;
	bool[] inGround;
	Vector3 okiba = new Vector3(0,-1000,0);
	int index = 0;
	public float chargetime = 0;
	GameObject ChargeObj;
	Image ChargeImage;



	// Use this for initialization
	void Awake(){
		if (photonView.isMine) {
			
			_renderer = this.GetComponent<Renderer> ();
			tofu = Resources.Load ("Tofu") as GameObject;
			_renderer.material.color = new Color (Random.value, Random.value, Random.value, 1.0f);



			audiosource = GetComponent<AudioSource> ();
			tofus = new GameObject[max];
			tofuScript = new TofuScript[max];
			inGround = new bool[max];
		}
	}
	void Start () {
		if (photonView.isMine) {
			script = GameObject.Find ("ScoreManager").GetComponent<ScoreScript> ();
			script.mycolor = _renderer.material.color;
			script.isColor = true;
			player = transform.parent;

			for (int i = 0; i < max; i++) {
				tofus [i] = PhotonNetwork.Instantiate ("Tofu", okiba, Quaternion.identity, 0) as GameObject;
				tofus [i].GetComponent<Renderer> ().material.color = script.mycolor;
				inGround [i] = false;
				tofuScript [i] = tofus [i].GetComponent<TofuScript> ();
			}
			//ChargeObj = GameObject.FindGameObjectWithTag ("Charge");
		}

	}
	
	// Update is called once per frame
	void Update () {
		
		if (photonView.isMine) {
			if (ChargeObj == null) {
				ChargeObj = GameObject.FindGameObjectWithTag ("Charge");
				ChargeImage = ChargeObj.GetComponent<Image> ();
				ChargeImage.color = script.mycolor;
			} else {
				
				ChargeImage.fillAmount = chargetime / 1.5f;

			}
			transform.position = player.position + Vector3.up * (1.65f + transform.localScale.y * 0.5f);
			if (timer > 0) {
				timer -= Time.deltaTime;
			}

			if (Input.GetKeyUp (KeyCode.Space)/*Input.GetMouseButtonUp(0)||*/&& term >= 0 && timer <= 0) {
				Shot ();
				term -= Time.deltaTime;
				chargetime = 0;
				if (term < 0) {
					timer = interval;
					term = 0.2f;
				}
			}
			if (Input.GetKey (KeyCode.Space)){
				if (chargetime <= 3) {
					chargetime += Time.deltaTime;
				}
			} else {
				chargetime = 0;
			}
		
		
			this.transform.localScale = Vector3.one * 0.1f + Vector3.one * script.myScore * 0.025f;
			//Level ();
		}
	}

		
	

	public void Shot(){
		int random = Random.Range (0,2);
		audiosource.clip = clips[random]; //声
		audiosource.Play ();
		Charge_Rensya ();
		for (int i = 0; i < rensya; i++) {
			if (tofus [index].transform.position.y < -500) {
				if (rensya == 3) {
					Vector3 pos = new Vector3 (transform.position.x, 1.65f, transform.position.z);
					tofus [index].transform.position = pos;
					transform.localRotation = Quaternion.Euler (0, -30 + i * 30, 0);
					Vector3 angle = transform.forward * 20; //+ new Vector3(0,-45 + 45*i,0);
					tofuScript [index].GetRb ().velocity = angle;
					index++;
					if (index >= max) {
						index = 0;
					}
				} else {
					Vector3 pos = new Vector3 (transform.position.x, 1.65f, transform.position.z);
					tofus [index].transform.position = pos;
					tofuScript [index].GetRb ().velocity = transform.forward * (10 * i + 20);
					index++;
					if (index >= max) {
						index = 0;
					}
				}
			}
			//GameObject obj = PhotonNetwork.Instantiate ("Tofu",pos + transform.forward.normalized, transform.rotation, 0) as GameObject;
			//obj.GetComponent<Renderer> ().material.color = script.mycolor;
			//obj.GetComponent<Rigidbody> ().velocity = transform.forward * 10;
		}
		transform.localRotation = Quaternion.identity;
	}

	void Charge_Rensya(){
		if (chargetime > 2.0f/2) {
			rensya = 6;
			interval = 1.0f;
		} else if(chargetime > 1.0f/2){
			rensya = 3;
			interval = 0.8f;
		}else {
			rensya = 1;
			interval = 0.2f;
		}

	}
	void Level(){
		int score = script.myScore;
		if (score < 5) {
		} else if (score < 10) {
			//rensya = 0.3f;
		} else if (score < 15) {
			//rensya = 2;
			interval = 1.2f;
		} else if (score < 20) {
			interval = 2;
		} else if (score < 25) {
			//rensya = 0.5f;
		} else if (score < 30) {
			interval = 1.0f;
		} else if (score < 35) {
			interval = 0.8f;
		} else if (score < 40) {
			//tydrensya = 0.4f;
		} else if (score < 45) {
			interval = 0.5f;
		} else if (score < 50) {
			//interval = 0.25f;
		} else if (score < 55) {
			//interval = 0.1f;
		} else if (score < 60) {
			//interval = 0.01f;
		} else if (score < 65) {
		} else {
		}
	}
}
