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

	[SerializeField]
	private GameObject _parent;

	private Coroutine _current;

	private string _dialogue = "";

	private readonly StringBuilder _stringBuilder = new();

	private IList<string> _choices = new List<string>();

	public bool Hidden {
		get => _parent.activeSelf;
		set {
			_parent.SetActive(!value);
		}
	}

	public IList<string> Choices {
		get => new ReadOnlyCollection<string>(_choices);
		set {
			_choices = new List<string>(value);
			_txtDialogue.text = ""; // EnumerateChoices(_choices);
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

	private IEnumerator Show(int start) {
		for( int i = start + 1; i <= _dialogue.Length; ++i ) {
			yield return new WaitForSeconds(_wait_s);
			_stringBuilder.Length = 0;
			_stringBuilder
				.Append(_dialogue[..i])
				.Append("<color=#00000000>")
				.Append(_dialogue[i..]);
			_txtDialogue.text = _stringBuilder.ToString();
		}
	}

	private Coroutine Speak(string dialogue, bool resume) {
		if( _current != null )
			StopCoroutine(_current);
		int start = 0;
		if( dialogue.EndsWith('\n') )
			dialogue = dialogue[..^1];
		if( resume ) {
			start = _dialogue.Length + 1;
			_stringBuilder.Length = 0;
			_stringBuilder
				.Append(_dialogue).AppendLine()
				.Append(dialogue);
			_dialogue = _stringBuilder.ToString();
		}
		else {
			_dialogue = dialogue;
		}
		return _current = StartCoroutine(Show(start));
	}
}