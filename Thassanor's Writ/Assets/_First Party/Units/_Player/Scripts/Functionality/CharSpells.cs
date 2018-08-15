﻿/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
   Author: 			Hayden Reeve
   File:			CharSpells.cs
   Version:			0.3.0
   Description: 	Controls all functions related to the Typing Elements within the game.
// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

using UnityEngine;
using UnityEngine.Networking;
using TMPro;

[RequireComponent(typeof(CharVisuals))]
public class CharSpells : NetworkBehaviour {

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		References
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	private Transform _trTypingComponent;	// This is the base root we'll be working with for the Input System.
	private TMP_InputField _inputField;     // This is the player's Input Field.
	private TMP_Text _backgroundText;       // Used to show what we think the player is trying to type, or show that another player is typing something.

	private CharVisuals _scVisual;			// The Visual Controller script for the character.

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		Variables
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	// The list of spells in our loadout. These should generally be assigned at runtime, however if they aren't, then the defaults are assigned in the inspector.
	[SerializeField] private Spell[] _spellLoadout;
	[SerializeField] private string[] _astSpellPhrases;

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		Initialisation
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	// Run before Start()
	private void Awake() {

		// Grab the script project root and it's associated references.
		_trTypingComponent = transform.Find("PlayerCanvas");
		_inputField = _trTypingComponent.GetComponentInChildren<TMP_InputField>();
		_scVisual = GetComponent<CharVisuals>();

	}

	// Run at the start of an object's lifetime.
	private void Start() {

		// Aquire Spell Settings from Phil's Implementation.
		int difficulty = 1;

		// Take out the corresponding "casting string" from each spell in the player's loadout and store it in something easier to manage later on.
		_astSpellPhrases = new string[_spellLoadout.Length];
		
		for (int it = 0; it < _spellLoadout.Length; it++)
			_astSpellPhrases[it] = _spellLoadout[it]._astSpellPhrase[difficulty];

		// Just as a failsafe, we want to make sure we're always starting with the input field disabled.
		TypeStatus(false, true);

	}

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		Class Calls
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	// Called when we're starting to type. Contains all activation code and executes any runtime requirements.
	public void TypeStatus(bool toggleOn, bool cancelCast = false) {

		ShowInputField(toggleOn);
		CallCameraZoom(toggleOn);
		CallAnimationCasting(toggleOn);

		if (!toggleOn)
			return;

		if (cancelCast)
			return;

		CastSpell();

	}

	/* ----------------------------------------------------------------------------- */
	// Spellcasting Calls

	// The Prediction Model that we're using to match the player's currently entered text to the closest matching spell in their loadout.
	public void PredictSpell() {

		if (!hasAuthority)
			return;

		// Identify which spell we're trying to cast by comparing our current string to the string for each equipped spell.

		// Display the closest matched string visually for the player to see.

	}

	// Choosing, and then casting the currently selected spell.
	private void CastSpell() {

		// Identify which spell we cast.


		// Determine and assign a value to how accurately we typed our spell.


		// Assign spell targets.


		// Call any functions related to the spell's specific function.


		// Finalise casting interface and move back to normal controls.
		TypeStatus(false, true);

	}

	// Here we evaluate the player's accuracy to what they intended to cast and modify casting time based on this.
	private float CastEvaluation(float fl) {

		return fl;

	}

	/* ----------------------------------------------------------------------------- */
	// Input Field Calls

	// We're using this to toggle the User Interface that allows the player to begin typing. Calling ShowInputField is also implicetly calling FocusInputField.
	public void ShowInputField(bool toggleOn = true) {

		// We're doing this when true to wipe any old text that may still be stored from the last spellcast.
		if (toggleOn)
			_inputField.text = "";

		if (!hasAuthority)
			return;

		_trTypingComponent.gameObject.SetActive(toggleOn);
		_inputField.interactable = toggleOn;

		FocusInputField(toggleOn);

	}

	// We're using the existing Events within the InputField object to force focus onto the object until it's not needed anymore.
	public void FocusInputField(bool toggleOn = true) {

		if (!hasAuthority)
			return;

		if (toggleOn) {
			_inputField.ActivateInputField();
			_inputField.MoveTextEnd(false);

		} else {
			_inputField.DeactivateInputField();

		}

	}

	/* ----------------------------------------------------------------------------- */
	// Visual Calls.

	private void CallCameraZoom(bool toggleOn) {

		if (!hasAuthority)
			return;

		_scVisual.CharacterZoom(toggleOn);

	}

	private void CallAnimationCasting(bool toggleOn) {

		_scVisual.AnimCasting(toggleOn);

	}

}