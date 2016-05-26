using UnityEngine;
using System.Collections;

public class GameManager : Photon.MonoBehaviour {

	private static GameManager instance = null;
	public float timer = 60;
	bool isGame = false;
	ScoreScript scoreScript;
	State state = State.Ready;
	private PhotonView m_photonView = null;
	AudioSource audioSource;
	float countDownTime = 3;
	string resultMessage = "";
	Color winColor = Color.blue;
	public AudioClip[] audioclips;
	Animator canvasAnim;

	enum State{
		Ready=0,PreGame=1,Game=2,Result=3,Pose=4
	};

	void Awake(){
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (this.gameObject);
		} else {
			Destroy (this.gameObject);
		}

	}

	void OnJoinedRoom() {
		m_photonView = this.GetComponent<PhotonView>();
		canvasAnim.SetTrigger ("Loading");
	}

	// Use this for initialization
	void Start () {
		scoreScript = GameObject.FindGameObjectWithTag ("ScoreManager").GetComponent<ScoreScript>();
		State state = State.Ready;
		m_photonView = this.GetComponent<PhotonView>();
		audioSource = GetComponent<AudioSource> ();
		canvasAnim = GameObject.Find ("Canvas").GetComponent<Animator>();

	}
	
	// Update is called once per frame
	void Update () {
		canvasAnim.SetInteger ("state",(int)state);
		/*
		foreach (PhotonPlayer player in PhotonNetwork.playerList) {
			Debug.Log (player.name);
		}*/
		switch (state) {
		case State.Ready:
			if (!audioSource.isPlaying && PhotonNetwork.connected) {
				audioSource.clip = audioclips [0];
				audioSource.Play ();
			}
			break;
		case State.PreGame:
			countDownTime -= Time.deltaTime;
			if (countDownTime < 0) {
				state = State.Game;
				audioSource.clip = audioclips [1];
				audioSource.Play ();
				timer = 60;
				if (PhotonNetwork.player.name == "") {
					PhotonNetwork.player.name = "player" + PhotonNetwork.player.ID;
				}
			}
			break;
		case State.Game:
			timer -= Time.deltaTime;
			if (timer < 0) {
				state = State.Result;
				Result ();
				Calculation ();
			}
			//PhotonNetwork.player.SetScore (scoreScript.myScore);
			break;
		case State.Result:
			break;
		case State.Pose:
			break;
		default:
			break;
		}
	}

	public void Result(){
		PhotonNetwork.player.SetScore (scoreScript.myScore);
	}
	void Calculation(){
		PhotonPlayer max_player = PhotonNetwork.player;
		int max_player_ID = -1;
		int max_Score = -1;
		foreach (PhotonPlayer player in PhotonNetwork.playerList) {
			if (max_Score < player.GetScore ()) {
				max_Score = player.GetScore ();
				max_player_ID = player.ID;
				max_player = player;
			}
		}
		if (PhotonNetwork.player.ID == max_player_ID) {
			resultMessage = "1位 Player名:" + max_player.name + " Score:" + max_Score.ToString ()+"\n"+"おめでとう！";
		} else {
			resultMessage = "1位 Player名:" + max_player.name + " Score:" + max_Score.ToString ()+"\n"+"残念...";
		}
		Debug.Log ("cal");
	}

	public static GameManager GetInstance(){
		return instance;
	}

	public void GameStart(){
		Debug.Log (state);
		switch (state) {
		case State.Ready:
			state = State.PreGame;
			countDownTime = 2.99f;
			audioSource.Stop ();
			break;
		case State.PreGame:
			break;
		case State.Game:
			break;
		case State.Result:
			state = State.PreGame;
			countDownTime = 2.99f;
			audioSource.Stop ();
			break;
		case State.Pose:
			break;
		default:
			break;
		}

	}

	public void SendRPCStart() {
		if (m_photonView != null) {
			m_photonView.RPC("GameStartMessage", PhotonTargets.All);
		}
	}
	public void SendRPCColoer(){
		if (m_photonView != null) {
			m_photonView.RPC("SendColor", PhotonTargets.All,scoreScript.mycolor);
		}
	}

	public int GetState(){
		return (int)state;
	}

	public float GetCountDownTimer(){
		return countDownTime;
	}
	public string GetRusultMessage(){
		return resultMessage;
	}
}
