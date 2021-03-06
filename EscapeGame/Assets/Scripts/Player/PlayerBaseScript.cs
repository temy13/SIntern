﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBaseScript : MonoBehaviour {

	//public Vector2 m_SPEED = new Vector2(0.05f, 0.05f);
	private CharacterController m_Controller;
	public GameManagerScript m_GameManagerScript;
	private Vector3	m_Move = Vector3.zero;
	private float m_Speed = 1.25f;
	private const float	GRAVITY = 19.8f;			// 重力
	protected bool m_IsUsing = true;
	private bool m_IsAlive = true;
	private Vector3 m_firstPosition;

	private GameObject[] Lifts;

	// Use this for initialization
	public void Start () {
		m_Controller = GetComponent<CharacterController>();
		m_GameManagerScript = GameObject.Find ("GameManager").GetComponent<GameManagerScript> ();
		m_firstPosition = this.transform.position;
		Lifts = GameObject.FindGameObjectsWithTag ("Lift");

	}
	
	// Update is called once per frame
	public void Update () {
		this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, 0.1f);
		Move ();
		if (this.m_IsUsing && Input.GetButtonDown ("Action")) {
			Action ();
		}
		if (Input.GetButtonDown ("Change")) {
			ChangeCharacter (!m_IsUsing);
		}
		if (Lifts.Length > 0) {
			foreach (var o in Lifts) {
				if (o.GetComponent<LiftScript> ().IsLiftDown ()) {
					IsPressedCheck (o.transform);
				}
			}
		}

		if(this.transform.position.y < -10.0f){
			Dead ();
		}
	}

	void Move(){
		float x = 0.0f;
		if (this.m_IsUsing) {
			x = Input.GetAxis ("Horizontal") * m_Speed;
		}
		Vector3 scale = this.transform.localScale;
		if (x < 0) {
			this.transform.localScale = new Vector3(-1*Mathf.Abs(scale.x), scale.y, 1); //左向く
		} else if(x > 0) {
			this.transform.localScale = new Vector3(1*Mathf.Abs(scale.x), scale.y, 1); //右向く
		}
		m_Move = new Vector3(x, m_Move.y , 0.0f);		// 左右上下のキー入力を取得し、移動量に代入.
		m_Move.y -=  GRAVITY * Time.deltaTime;	// 重力を代入.
		m_Controller.Move(m_Move * Time.deltaTime);	// キャラ移動.

        if (m_Controller.isGrounded)
            m_Move.y = 0f;
	}

	public void ChangeCharacter(bool IsUsing){
		this.m_IsUsing = IsUsing;

        if(IsUsing)
        {
            transform.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            transform.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
        }
	}

	public virtual void Action(){
		;
	}


	public void Dead(){
		Debug.Log ("Dead");
		PauseCanvasScript.I.GameOver ();
		this.m_IsAlive = false;
		Destroy (this.transform.gameObject);
	}

	public void Reset(){
		this.transform.position = m_firstPosition;
		this.m_IsAlive = true;
	}


	List<string> m_ClimbTags = new List<string>(){"Block","FixedBlock","Lift"};

	public void OnControllerColliderHit(ControllerColliderHit hit) {

		string tag = hit.transform.tag;

		//
		float r = 0f;
		float maxY = 0;
		float minY = 0;
		if (this.transform.gameObject.name == "Princess") {
			r = 0.75f;
			//offsetY = 0.3f;
			//1.05 ~ 0.94 NG:1.17
			minY = 0.9f;
			maxY = 1.1f;
		}else if(this.transform.gameObject.name == "Robot"){
			r = 1.5f;
			//offsetY = 1.4f;
			//offsetY = 1.82f;
			//1.47 - 1.77 NG:1.77
			maxY = 1.50f;
			minY = 1.45f;
		}
			
		if (m_ClimbTags.Contains (tag)) {
			Collider[] colliders = Physics.OverlapSphere (this.transform.position, r);
			bool isDownThrough = false;
			foreach (Collider col in colliders) {
				if (m_ClimbTags.Contains (col.transform.tag)) {
					Vector3 pos = transform.position;
					float xLimit = 1.3f;
					if (col.transform.tag == "Lift") {
						xLimit += (col.transform.localScale.x - 1.0f) * 0.4f;
					}
					float diff = col.transform.position.y - pos.y;
					if (minY < diff && diff < maxY && 
						Mathf.Abs(col.transform.position.x - pos.x) < xLimit) {
						if (0 < transform.localScale.x && pos.x < col.transform.position.x ||
						   0 > transform.localScale.x && pos.x > col.transform.position.x) {
							isDownThrough = true;
						}
					}
				}
			}
			if (!isDownThrough) {
				if (m_Controller.stepOffset == 0) {
					this.m_Controller.stepOffset = 0.8f;
					this.m_Controller.slopeLimit = 90f;
				}
			} else {
				if (m_Controller.stepOffset == 0.8f) {
					this.m_Controller.stepOffset = 0.0f;
					this.m_Controller.slopeLimit = 0;
				}
			}

		}
	}

	void IsPressedCheck(Transform tr){
		
		if(Mathf.Abs(tr.position.x - this.transform.position.x) < tr.localScale.x / 2.0f
			&& tr.position.y > this.transform.position.y){
			float height = this.transform.gameObject.name == "Princess" ? 1.0f : 1.47f;
			if (tr.position.y - this.transform.position.y < height) {
				Dead ();
			}
		}
	}
		
}
