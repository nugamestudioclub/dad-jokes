using Ink.Runtime;
using System.Linq;
using UnityEngine;

public class Scene : MonoBehaviour {
	[SerializeField]
	private TextAsset _inkText;

	[SerializeField]
	private DialogueView _dialogueView;

	private Story _story;

	void Awake() {
		_story = new Story(_inkText.text);
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
		for( int i = 0; i <= 9; ++i ) {
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

	private void OfferStoryOptions(PlayerInput input) {
		if( _dialogueView.Choices.Count != _story.currentChoices.Count && input.Interact ) {
			_dialogueView.Choices = _story.currentChoices.Select(x => x.text).ToList();
		}
	}
}