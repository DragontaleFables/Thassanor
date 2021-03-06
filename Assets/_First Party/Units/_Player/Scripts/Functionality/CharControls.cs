﻿/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
   Author: 			Hayden Reeve
   File:			CharControls.cs
   Version:			0.5.1
   Description: 	Recieves movement input and controls the player's position.
// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

using UnityEngine;

[RequireComponent(typeof(CharVisuals))]
public class CharControls : MonoBehaviour {

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		References
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	private Rigidbody _rb;
	private Transform _rallyPoint;
	private CharVisuals _charVisuals;

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		Variables
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	// Movement Variables
	[SerializeField] private float _flMovementSpeed;
	private Vector3 _v3Trajectory = new Vector3 (0f,0f,0f);

	[SerializeField] private Vector3 _v3RallyDistance = new Vector3(0f, 0f, 0f);

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		Initialisation
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	public void Awake() {

		// References
		_rb = GetComponent<Rigidbody>();
		_charVisuals = GetComponent<CharVisuals>();
		_rallyPoint = transform.Find("RallyPoint");

	}

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		Class Calls
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */
	
	public void TrajectoryChange(float[] flDirection = null) {

		// Any null answers should be considered "Halt" commands
		if (flDirection == null)
			_v3Trajectory = new Vector3(0f, _v3Trajectory.y, 0f);

		else {

			// Otherwise, consider the input as normal and add the values to each direction as needed.
			_v3Trajectory = new Vector3(flDirection[1] * _flMovementSpeed, _v3Trajectory.y, flDirection[0] * _flMovementSpeed);

			// And update the RallyPoint to reside on the opposite facing side of the player. Minions need to know their place!
			if (_v3Trajectory != Vector3.zero) {

				// Place the initial position.
				_rallyPoint.position = transform.position - _v3Trajectory.normalized;
				_rallyPoint.LookAt(2 * _rallyPoint.position - transform.position);

				// Add our adjustments.
				_rallyPoint.position = _rallyPoint.TransformPoint(_v3RallyDistance);

			}

		}
			

		// Then, since we have a new command, we send that variable to the Animator to adjust the visuals.
		_charVisuals.AnimMovement(flDirection);

	}

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		Class Runtime
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	// Apply new Vector3 to velocity.
	private void FixedUpdate() => _rb.velocity += _v3Trajectory.normalized; 

}