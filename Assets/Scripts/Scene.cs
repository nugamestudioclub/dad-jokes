﻿using Ink;
using Ink.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Scene : MonoBehaviour {
	[SerializeField]
	private TextAsset _inkText;

	[SerializeField]
	private DialogueView _dialogueView;

	[SerializeField]
	private List<string> _items = new();

	private Story _story;

	void Awake() {
		_story = new Story(_inkText.text);
		_story.onError += (errorMessage, errorType) => {
			if( errorType == ErrorType.Warning )
				Debug.LogWarning(errorMessage);
			else
				Debug.LogError(errorMessage);
		};
	}

	void Update() {
		var input = ReadInput();
		if( _story.canContinue ) {
			ContinueStory(input);
		}
		else if( !_dialogueView.IsSpeaking ) {
			OfferStoryOptions(input);
		}
	}

	private static PlayerInput ReadInput() {
		return new PlayerInput {
			Interact = ReadInteract(),
			Selection = ReadSelection()
		};
	}

	private static bool ReadInteract() {
		return Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space);
	}

	private static int ReadSelection() {
		for( int i = 1; i <= 9; ++i ) {
			if( Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i) ) {
				return i;
			}
		}
		return -1;
	}

	private void ContinueStory(PlayerInput input) {
		if( input.Interact && !_dialogueView.IsSpeaking ) {
			string dialogue = _story.Continue();
			StartCoroutine(_dialogueView.Speak(dialogue));
		}
	}

	private int FindChoiceIndex(int itemIndex) {
		string item = _items[itemIndex];
		var choices = _story.currentChoices.Select(x => x.text).ToList();
		return choices.IndexOf(item);
	}

	private void OfferStoryOptions(PlayerInput input) {
		if( _dialogueView.Choices.Count != _story.currentChoices.Count && input.Interact ) {
			_dialogueView.Choices = _story.currentChoices.Select(x => x.text).ToList();
		}
		else if( input.Selection > 0 ) {
			int choiceIndex = FindChoiceIndex(input.Selection - 1);
			_story.ChooseChoiceIndex(choiceIndex);
			string dialogue = _story.Continue();
			StartCoroutine(_dialogueView.Speak(dialogue));
		}
	}
}