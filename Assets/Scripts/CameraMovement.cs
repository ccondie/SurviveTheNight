﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	private GameObject player;
	private Vector3 offset;
	bool shakyCam;
	private Vector3 shakyOffset;
	bool shakyHold;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		offset = transform.position;
	}

	// Update is called once per frame
	void LateUpdate () {
		if (shakyHold) {
			float x = UnityEngine.Random.value * 2.0f - 1.0f;
			float y = UnityEngine.Random.value * 2.0f - 1.0f;
			x *= mag;
			y *= mag;
			shakyOffset = new Vector3(x, y, 0);
			shakyOffset = ((Vector3) orig) + shakyOffset;
			transform.position = shakyOffset;
		} else if (shakyCam) {
			//float x = Random.Range(-1f, 1f);
			//float y = Random.Range (-1f, 1f);
			//Vector3 shakyOffset = new Vector3 (x, y, 0);
			transform.position = shakyOffset;
		} else {
			transform.position = player.transform.position + offset;
		}
	}
	float mag;
	Vector3? orig = null;
	public void BeginShake(float magnitude) {
		if (orig == null) orig = transform.position;
		mag = magnitude;
		shakyHold = true;
	}

	public void EndShake() {
		orig = null;
		shakyHold = false;
	}

	public void Shake(float duration, float magnitude) {
		StartCoroutine(setShake(duration, magnitude));
	}

	//float duration = 1f;
	//float magnitude = 1f;

	public IEnumerator setShake(float duration, float magnitude) {
		float elapsed = 0.0f;
		Vector3 originalPos = transform.position;
		while (elapsed < duration) {
			elapsed += Time.deltaTime;
			float percentComplete  = elapsed/duration;
			float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);
			float x = UnityEngine.Random.value * 2.0f - 1.0f;
			float y = UnityEngine.Random.value * 2.0f - 1.0f;
			x *= magnitude* damper;
			y *= magnitude* damper;
			shakyOffset = new Vector3(x, y, 0);
			shakyOffset = originalPos + shakyOffset;
			transform.position = shakyOffset;
			shakyCam = true;
			yield return null;	
		}
		shakyCam = false;
		transform.position = originalPos;
	}

}
