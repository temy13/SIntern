﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PrincessScript : PlayerBaseScript {

	//public GameObject m_Block;
	public bool m_HasKey;
	public GameObject m_Key;

    private Animator m_Animator;

	// Use this for initialization
	new void Start () {
		base.Start ();
		m_HasKey = false;
		ChangeCharacter (true);

        m_Animator = transform.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	new void Update () {
		base.Update ();

        if (m_IsUsing)
            m_Animator.SetFloat("Horizontal", Mathf.Abs(Input.GetAxis("Horizontal")));
        else
            m_Animator.SetFloat("Horizontal", 0);
	}

	public override void Action(){
		if (IsGetKey ()) {
			return;
		}
		if (IsPushSwitch ()) {
			return;
		}
		
		if (!m_HasKey) {
			GenerateBlock ();
		} else {
			PutKey ();
		}
	}

	private bool IsGetKey(){
		GameObject Key = GameObject.FindGameObjectWithTag ("Key");
		if (!Key) {
			return false;
		}
		//GetKey
		if (this.IsTouch(Key.transform.position)) {
			Destroy (Key);
			m_HasKey = true;
            m_Animator.SetBool("HasKey", m_HasKey);
			return true;
		}
		return false;
	}

	private bool IsPushSwitch(){
		GameObject[] Switches = GameObject.FindGameObjectsWithTag ("Switch");
		if (Switches.Length <= 0) {
			return false;
		}
		foreach (GameObject o in Switches) {
			if (this.IsTouch(o.transform.position)) {
				o.SendMessage ("Push");
				return true;
			}
		}
		return false;
	}

	private bool IsTouch(Vector3 targetPos){
		return Mathf.Abs (targetPos.y - transform.position.y) < 0.5 && Mathf.Abs (targetPos.x - transform.position.x) < 0.5;
//		if(Mathf.Abs(targetPos.y - transform.position.y) > 0.5){
//			return false;
//		}
//		float distance = this.transform.position.x - targetPos.x;
//		if (this.transform.localScale.x > 0) {
//			return -1.5 < distance && distance < 0;
//		} else {
//			return 0 < distance && distance < 1.5;
//		}
	}

	private void PutKey(){
		Vector3 pos = GameManager.I.GetGeneratePos( this.transform.position);
		var go = Instantiate(m_Key, new Vector3(pos.x, pos.y, 0), Quaternion.identity) as GameObject;
        GameManager.I.AddGameObject(go, GameManager.Type.Dynamic);
		m_HasKey = false;
        m_Animator.SetBool("HasKey", m_HasKey);
	}

	private void GenerateBlock(){
		
        if(GameManager.I.ActionHime(this.transform))
        {
            m_Animator.SetTrigger("CreateBlock");
        }
	}

	void OnTriggerEnter(Collider other){
		//base.OnTriggerEnter(other);
		if(other.transform.tag == "Door"){
			if (this.m_HasKey) {
				other.transform.gameObject.SendMessage ("Goal");
			}
		}
        else if(other.transform.tag == "FinalDoor")
        {
            if (this.m_HasKey)
                GameManager.I.GotoEnding();
        }
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		base.OnControllerColliderHit (hit);
		switch (hit.transform.tag) {
		case "Enemy":
			Dead ();
			break;
		default:
			break;
		}
	}
	new public void Reset(){
		base.Reset ();
		m_HasKey = false;
		ChangeCharacter (true);
        m_Animator.SetBool("HasKey", m_HasKey);
	}

}
