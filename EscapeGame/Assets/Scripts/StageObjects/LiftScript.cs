﻿using UnityEngine;
using System.Collections;

public class LiftScript : MonoBehaviour {

	private Vector3 m_firstPosition;

	private bool m_IsCollision;
	private bool m_IsMoving;

	// Use this for initialization
	void Start () {
		this.m_firstPosition = this.transform.position;
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Move(bool isDown, int distance){
		m_IsCollision = false;
		if (this.m_IsMoving) {
			return;
		}
		StartCoroutine (MoveCoroutine (distance, isDown));
	}

	// コルーチン  
	private IEnumerator MoveCoroutine(float distance, bool isDown) {
		m_IsMoving = true;
		float limit = isDown ? m_firstPosition.y - distance : m_firstPosition.y;
		float diff = isDown ? -0.1f : 0.1f;
		for (int i = 0; i < distance*10; i++) {
			if (m_IsCollision) {
				break;
			}
			this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y + diff, 0);
			yield return new WaitForSeconds (0.1f);  
			if (isDown && this.transform.position.y <= limit) {
				break;
			} else if (!isDown && this.transform.position.y >= limit) {
				break;
			}
		}
		m_IsMoving = false;
		yield return null;
	} 

	void OnTriggerEnter(Collider other){
		Debug.Log (other.transform.tag);
		m_IsCollision = true;
	}

	public void Reset(){
		this.transform.position = this.m_firstPosition;
	}
}
