﻿/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
   Author: 			Hayden Reeve
   File:			CharVisuals.cs
   Version:			0.1.0
   Description: 	Called by player scripts that need to execute visual functions. Should not directly recieve player input.
// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharVisuals : MonoBehaviour {

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		References
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	Animator _animator;
	SpriteRenderer _spriteRenderer;

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		Variables
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	// Foobar

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		Initialisation
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	private void Awake() {

		_animator = GetComponentInChildren<Animator>();
		_spriteRenderer = GetComponentInChildren<SpriteRenderer>();

	}

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		Class Calls
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	public void AnimMovement(float[] aflMovement) {
	
		// First we need to determine whether we're running or not.
		_animator.SetBool("isRunning", (aflMovement[0] != 0 || aflMovement[1] != 0) );

		// Then we need to determine whether we've changed directions or not.
		if (aflMovement[1] > 0) {
			_spriteRenderer.flipX = true;
		} else if (aflMovement[1] < 0) {
			_spriteRenderer.flipX = false;
		}

	}

	// Call Animation Class. We are running.
	public void AnimMovement(bool isMoving) {
		_animator.SetBool("isRunning", isMoving);
	}

	// Call ANimation Class, we are running, and have changed direction.
	public void AnimMovement(bool isMoving, float flMovementDir) {
		
		_animator.SetBool("isRunning", isMoving);

		if (flMovementDir > 0) {
			_spriteRenderer.flipX = true;
		} else if (flMovementDir < 0) {
			_spriteRenderer.flipX = false;
		}
		
	}

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		Class Runtime
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	// // Update is called once per frame
	// private void Update () {
		
	// }

}