using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;

public class DialogueView : MonoBehaviour {
	private const float WAIT_S = 0.025f;

	[SerializeField]
	private TMP_Text _txtDialogue;

	private Coroutine _current;

	private string _dialogue = "";

	public bool IsSpeaking { get; private set; }

	public IEnumerator Speak(string dialogue) {
		IsSpeaking = true;
		yield return Speak(dialogue, false);
		IsSpeaking = false;
	}

	public IEnumerator SpeakMore(string dialogue) {
		IsSpeaking = true;
		yield return Speak(dialogue, true);
		IsSpeaking = false;
	}

	public void Offer(IEnumerable<string> choices) {

	}

	private IEnumerator Show(string dialogue, int start) {
		for( int i = start + 1; i <= dialogue.Length; ++i ) {
			yield return new WaitForSeconds(WAIT_S);
			_txtDialogue.text = dialogue[..i];
		}
	}

	private Coroutine Speak(string dialogue, bool resume) {
		if( _current != null )
			StopCoroutine(_current);
		int start = 0;
		if( resume ) {
			start = _dialogue.Length + 1;
			dialogue = _dialogue + "\n" + dialogue;
		}
		_dialogue = dialogue;
		return _current = StartCoroutine(Show(dialogue, start));
	}
}