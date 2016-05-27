using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : Photon.MonoBehaviour {


	public AudioClip[] audioclips;
	public float MaxTime = 60;


	float timer = 0;
	private static GameManager instance = null;
	bool isGame = false;
	ScoreScript scoreScript;
	State state = State.Ready;
	private PhotonView m_photonView = null;
	AudioSource audioSource;
	float countDownTime = 3;
	string resultMessage = "";
	Color winColor = Color.blue;
	Animator canvasAnim;
	int result_num = 0;
	List<ScorePoint> pointList = new List<ScorePoint>(); 
	int playernum = 0;
	int readynum;


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
			playernum = PhotonNetwork.playerList.Length;
			break;
		case State.PreGame:
			countDownTime -= Time.deltaTime;
			if (countDownTime < 0) {
				state = State.Game;
				audioSource.clip = audioclips [1];
				audioSource.Play ();
				timer = MaxTime;
				if (PhotonNetwork.player.name == "") {
					PhotonNetwork.player.name = "player" + PhotonNetwork.player.ID;
				}
				result_num = 0;
				pointList.Clear ();
			}
			break;
		case State.Game:
			timer -= Time.deltaTime;
			Debug.Log (PhotonNetwork.countOfPlayers);
			if (timer < 0) {
				state = State.Result;
				Result ();
				playernum = PhotonNetwork.playerList.Length;

				//Invoke ("Calculation",2.0f);
			}
			//PhotonNetwork.player.SetScore (scoreScript.myScore);
			break;
		case State.Result:
			if (pointList.Count == playernum) {
				Calculation ();
				pointList.Clear ();
				result_num = 0;
			}
			break;
		case State.Pose:
			break;
		default:
			break;
		}
	}

	void Result(){
		//PhotonNetwork.player.SetScore (scoreScript.myScore);
		//ScorePoint myPoint = new ScorePoint (PhotonNetwork.player.ID,PhotonNetwork.playerName,scoreScript.myScore);
		//pointList.Add (myPoint);
		//result_num++;
		m_photonView.RPC ("SendPoint",PhotonTargets.All,PhotonNetwork.player.ID,PhotonNetwork.playerName,scoreScript.myScore);
	}
	void Calculation(){
		string max_Player = "--";
		int max_Score = -1;
		int max_id = -1;

		var newList = pointList
			.OrderByDescending (point => point.score)
			.ToList();
		string mes = "";
		int num = 1;//順位
		foreach (ScorePoint score in newList){
			mes += "No." + num.ToString () + ": " + score.GetName () + " Score: " + score.GetScore ().ToString()+"\n";
			num++;
		}
		resultMessage = mes;
		/*
		foreach (ScorePoint score in pointList) {
			if (score.GetScore () > max_Score) {
				max_Score = score.GetScore ();
				max_Player = score.GetName();
				max_id = score.GetID ();
			}
		}

		if (PhotonNetwork.player.ID == max_id) {
			resultMessage = "1位: " + max_Player + " Score:" + max_Score.ToString ()+"\n"+"おめでとう！";
		} else {
			resultMessage = "1位:" + max_Player + " Score:" + max_Score.ToString ()+"\n"+"残念...";
		}
		*/

		
	}
	/*
	void Calculation(){
		//PhotonPlayer max_player = PhotonNetwork.player;
		int max_player_ID = -1;
		int max_Score = -1;
		string max_player = "---";
		foreach (PhotonPlayer player0 in PhotonNetwork.playerList) {
			if (max_Score < player0.GetScore ()) {
				max_Score = player0.GetScore ();
				max_player_ID = player0.ID;
				max_player = player0.name;
			}
		}
		if (PhotonNetwork.player.ID == max_player_ID) {
			resultMessage = "1位: " + max_player + " Score:" + max_Score.ToString ()+"\n"+"おめでとう！";
		} else {
			resultMessage = "1位 Player名:" + max_player + " Score:" + max_Score.ToString ()+"\n"+"残念...";
		}
		Debug.Log ("cal");
	}*/

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

	[PunRPC]
	public void ClearStage(){
		foreach (GameObject cube in GameObject.FindGameObjectsWithTag ("Stage")) {
			cube.GetComponent<Renderer> ().material.color = Color.white;
		}
	}
	[PunRPC]
	void SendPoint(int id,string name,int score){
		ScorePoint point1 = new ScorePoint (id,name,score);
		pointList.Add (point1);
		result_num++;
	}

	public void GameStartButton(){
		foreach (GameObject cube in GameObject.FindGameObjectsWithTag ("Stage")) {
			cube.GetComponent<Renderer> ().material.color = Color.white;
		}
		m_photonView.RPC ("ClearStage",PhotonTargets.All);
	}

	public float GetTimer(){
		return timer;

	}


}
