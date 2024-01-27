using Ink.Runtime;
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

	private bool _done;

	void Update() {
		if( _story.canContinue && !_dialogueView.IsSpeaking ) { 
			Debug.Log("continue story");
			string dialogue = _story.Continue();
			StartCoroutine(_dialogueView.Speak(dialogue));
		}
		if( !_story.canContinue ) {
			for( int i = 0; i < _story.currentChoices.Count; ++i ) {
				var choice = _story.currentChoices[i];

			}
		}
	}
}