﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class NameManager : MonoBehaviour {

	//string name = "guest";

	// Use this for initialization
	void Start () {
		if (SceneManager.GetActiveScene().name == "Start") {
			DontDestroyOnLoad (this.gameObject);
		}
	}

	// Update is called once per frame
	void Update () {

	}
	public void EnterGame(){
		if (SceneManager.GetActiveScene().name == "Start") {


			GameObject field = GameObject.FindGameObjectWithTag ("InputField");
			if (field.GetComponent<InputField> ().text != "")
				PhotonNetwork.playerName = field.GetComponent<InputField> ().text;
			else
				PhotonNetwork.playerName = "player"+PhotonNetwork.player.ID;

			// 入る
			PhotonNetwork.JoinRandomRoom ();


			SceneManager.LoadScene ("Unitychan");
			//Application.LoadLevel ("Unitychan");



		}
	}
	public string GetName(){
		return name;
	}
}
