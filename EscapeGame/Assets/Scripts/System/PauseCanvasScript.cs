﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseCanvasScript : SingletonMonoBehaviour<PauseCanvasScript> {

	[SerializeField]
	private GameObject ResetUI;
	[SerializeField]
	private GameObject GameOverUI;

	void Awake()
	{
		if (this != I)
		{
			Destroy(this.gameObject);
			return;
		}
		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start () {
		if (ResetUI == null) {
			ResetUI = GameObject.Find ("Reset");
		}
		if (GameOverUI == null) {
			GameOverUI = GameObject.Find ("GameOver");
		}


		ResetUI.transform.FindChild("ResetButton").GetComponent<Button>().onClick.AddListener (Reset);
		ResetUI.transform.FindChild("CancelButton").GetComponent<Button>().onClick.AddListener (PauseCancel);
		GameOverUI.transform.FindChild("RetryButton").GetComponent<Button>().onClick.AddListener (Reset);

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Return) )
        {
			ResetPause ();
        }
	}

	void ResetPause(){
		Time.timeScale = 0;
		ResetUI.transform.FindChild ("ResetButton").GetComponent<Button> ().Select ();
		ResetUI.SetActive (true);
	}

	public void Reset(){
		PauseCancel ();
		GameManager.I.CallReset();
	}

	public void GameOver(){
		Time.timeScale = 0;
		GameOverUI.transform.FindChild ("RetryButton").GetComponent<Button> ().Select ();
		GameOverUI.SetActive (true);
	}

	void PauseCancel(){
		Time.timeScale = 1;
		ResetUI.SetActive (false);
		GameOverUI.SetActive (false);
	}
}
