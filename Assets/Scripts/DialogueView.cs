using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TMPro;
using UnityEngine;

public class DialogueView : MonoBehaviour {
	[SerializeField]
	private float _wait_s = 0.025f;

	[SerializeField]
	private TMP_Text _txtDialogue;

	private Coroutine _current;

	private string _dialogue = "";

	private readonly StringBuilder _stringBuilder = new StringBuilder();

	private IList<string> _choices = new List<string>();

	public IList<string> Choices {
		get => new ReadOnlyCollection<string>(_choices);
		set {
			_choices = new List<string>(value);
			_txtDialogue.text = EnumerateChoices(_choices);
		}
	}

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

	private string EnumerateChoices(IList<string> choices) {
		_stringBuilder.Length = 0;
		for( int i = 0; i < choices.Count; ++i ) {
			_stringBuilder.Append(i + 1).Append(". ").Append(choices[i]).AppendLine();
		}
		return _stringBuilder.ToString();
	}

	private IEnumerator Show(string dialogue, int start) {
		for( int i = start + 1; i <= dialogue.Length; ++i ) {
			yield return new WaitForSeconds(_wait_s);
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