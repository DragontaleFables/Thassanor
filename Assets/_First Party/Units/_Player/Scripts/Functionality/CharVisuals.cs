﻿/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
   Author: 			Hayden Reeve
   File:			CharVisuals.cs
   Version:			0.5.1
   Description: 	Called by player scripts that need to execute visual functions. Should not directly recieve player input.
// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CharVisuals : MonoBehaviour {

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		References
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	// Hierarchy Components
	private Animator _an;
	private SpriteRenderer _sr;
	private CameraPlayer _cm;

	[Space] [Header("Customisation")]

	[SerializeField] private NecromancerStyle _necromancerStyle;

	public NecromancerStyle _NecromancerStyle {
		set {
			_necromancerStyle = value;
			LoadStyle();
		}
	}

	[Space] [Header("Post Processing")]

	private PostProcessVolume _ppDeath;
	private PostProcessVolume _ppSpellcasting;

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		Variables
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	[Space] [Header("Camera")]

	private bool _characterInFocus;

	[Space]
	[SerializeField] private Vector3 _cameraOffset = new Vector3(0, 0, 0);
	[SerializeField] private Vector3 _cameraPanning = new Vector3(0, 0, 0);

	[Space]
	private List<Transform> _everyCameraFocus;

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		Initialisation
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	private void Reset() => GetReferences();
	private void Awake() => GetReferences();

	// Component Grab.
	private void GetReferences() {

		_an = GetComponentInChildren<Animator>();
		_sr = GetComponentInChildren<SpriteRenderer>();

		_cm = Camera.main.GetComponent<CameraPlayer>();

		_ppDeath = _cm.transform.Find("DeathEffects").GetComponent<PostProcessVolume>();;
		_ppSpellcasting = _cm.transform.Find("SpellcastEffects").GetComponent<PostProcessVolume>();

	}

	// Called before Update().
	private void Start() => LoadStyle();

	private void LoadStyle() => _an.runtimeAnimatorController = _necromancerStyle._animatorController;


	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		Class Calls
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	/* ----------------------------------------------------------------------------- */
	// Animation Controllers

	public void AnimMovement(float[] movementValues = null) {
	
		// If no value is passed to the function we should substitute {0,0} instead.
		movementValues = movementValues ?? new float[2];
		
		// First we need to determine whether we're running or not and change our animator as needed.
		_an.SetBool("isRunning", (movementValues[0] != 0 || movementValues[1] != 0) );

		// Here we only want to change our facing direction if we've got implicit movement direction (0<) plugged in.
		if (movementValues[1] > 0)
			_sr.flipX = false;

		else if (movementValues[1] < 0) 
			_sr.flipX = true;

	}

	public void AnimCasting(bool isSpelling = true) => _an.SetBool("isSpelling", isSpelling);

	public void Death() => _an.SetBool("isDead", true);

	/* ----------------------------------------------------------------------------- */
	// Camera Calls

	// Some easy fucntions that allow us to manipulate the camera without resetting anything crucial.
	public void ResetCamera() => _cm._PrimaryFocus = transform;
	public void ResetFocals() => _everyCameraFocus = new List<Transform>() { transform };
	private void LoadFocals() => _cm._everyFocus = _everyCameraFocus;

	// Controls the magnification effect when our character needs to be brought into focus.
	public void CharacterZoom(bool isClose) {

		_characterInFocus = isClose;

		if (isClose)
			ResetCamera();

		else
			LoadFocals();

		_cm._additionalOffset = isClose ? _cameraOffset : new Vector3 (0, 0, 0);
		_cm._focalOffset = isClose ? _cameraPanning : new Vector3 (0, 0, 0);

	}

	// We should add items we want to include in our focus here.
	public void AddCameraFocus(Transform focus) {

		_everyCameraFocus.Add(focus);

		if (!_characterInFocus)
			LoadFocals();

	}

	// This is kind of costly instead of just resetting our focus, but otherwise it iterates through our foci and removes when it finds a match.
	public void RemoveCameraFocus(Transform focus) {

		foreach (Transform cameraFocus in _everyCameraFocus) {

			if (cameraFocus == focus) {

				_everyCameraFocus.Remove(cameraFocus);
				return;

			}
		}

	}

	/* ----------------------------------------------------------------------------- */
	// Post Processing Calls

	public void PostProcessDeath(float health) => _ppDeath.weight = Dragontale.MathFable.Remap(health, 0, 100, 1, 0);

	public void PostProcessSpelling(bool isCasting) => _ppSpellcasting.weight = isCasting ? 1f : 0f;

}