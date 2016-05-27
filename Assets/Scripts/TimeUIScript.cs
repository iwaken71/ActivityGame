using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class TimeUIScript : Photon.MonoBehaviour {

	public Text timeLabel;
	public Text numberLabel;
	public Text countDownText;
	public GameObject resultPanel;
	public Text resultLabel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timerView ();
		numberView ();
		countDownView ();
		resultView ();



	}

	void timerView(){
		if (GameManager.GetInstance ().GetState () == 2) {
			timeLabel.text =  GameManager.GetInstance ().GetTimer().ToString ("f1");
		} else {
			timeLabel.text = "";
		}
	}
	void numberView(){
		numberLabel.text= "現在"+PhotonNetwork.playerList.Length + "人";
	}
	void countDownView(){
		
		if (GameManager.GetInstance ().GetState () == 1) {
			countDownText.text = ((int)(GameManager.GetInstance ().GetCountDownTimer () + 1)).ToString (); 
			float timer1 = GameManager.GetInstance ().GetCountDownTimer ();
			int timer2 = (int)GameManager.GetInstance ().GetCountDownTimer ();
			countDownText.fontSize = (int)(144 + (250 - 144) * (1 - (timer1 - timer2)));
		} else {
			countDownText.text = "";
		}
	}
	void resultView(){
		if (GameManager.GetInstance ().GetState () == 3) {
			resultPanel.SetActive (true);
			resultLabel.text = GameManager.GetInstance ().GetRusultMessage ();
		} else {
			resultPanel.SetActive (false);
			resultLabel.text = "";
		}
	}
}
