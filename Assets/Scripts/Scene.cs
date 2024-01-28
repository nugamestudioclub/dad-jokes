using Ink;
using Ink.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour {
	[SerializeField]
	private TextAsset _inkText;

	[SerializeField]
	private DialogueView _dialogueView;

	[SerializeField]
	private List<string> _items = new();

	[SerializeField]
	private AudioSource _sfxAudioSource;

	[SerializeField]
	private Soundset _soundset;

	private readonly Dictionary<string, AudioClip> _sounds = new();

	private Story _story;

	private readonly List<InteractableObject> _interactableObjects = new();

	private SceneMode _mode;
	public SceneMode Mode {
		get => _mode;
		private set {
			_dialogueView.Hidden = value != SceneMode.Dialogue;
			_mode = value;
		}
	}

	void Awake() {
		_story = new Story(_inkText.text);
		_story.onError += (errorMessage, errorType) => {
			if( errorType == ErrorType.Warning )
				Debug.LogWarning(errorMessage);
			else
				Debug.LogError(errorMessage);
		};
		InitializeSounds();
		_story.BindExternalFunction("playSfx", (string name) => { PlaySfx(name); });
	}

	void Start() {
		foreach( var obj in GameObject.FindGameObjectsWithTag("canInteract")
			.Concat(GameObject.FindGameObjectsWithTag("canPickUp")) ) {
			_interactableObjects.Add(obj.GetComponent<InteractableObject>());
		}
		Mode = SceneMode.Dialogue;
		ContinueStory();
	}

	void Update() {
		var input = ReadInput();
		if( _dialogueView.IsSpeaking ) {
			if( _story.canContinue && input.Interact ) {
				SkipStory();
			}
		}
		else {
			if( _story.canContinue ) {
				ContinueStory(input);
			}
			else if( _dialogueView.IsSpeaking ) {
				return;
			}
			else if( _story.currentChoices.Count > 0 ) {
				OfferStoryOptions(input);
			}
			else if( input.Interact ) {
				TransitionManager.ToCredits();
			}
		}
	}

	private void InitializeSounds() {
		_sounds[_soundset.BabyNoise1.Name] = _soundset.BabyNoise1.AudioClip;
		_sounds[_soundset.BabyNoise2.Name] = _soundset.BabyNoise2.AudioClip;
		_sounds[_soundset.BabyNoise3.Name] = _soundset.BabyNoise3.AudioClip;
		_sounds[_soundset.BabyNoise4.Name] = _soundset.BabyNoise4.AudioClip;
		_sounds[_soundset.BabyLaugh1.Name] = _soundset.BabyLaugh1.AudioClip;
		_sounds[_soundset.BabyLaugh2.Name] = _soundset.BabyLaugh2.AudioClip;
	}

	private PlayerInput ReadInput() {
		return new PlayerInput {
			Interact = ReadInteract(),
			Selection = ReadSelection()
		};
	}

	private static bool ReadInteract() {
		return Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space);
	}

	private int ReadSelection() {
		//Debug.Log($"num objects {_interactableObjects.Count}");
		if( Mode == SceneMode.Dialogue ) {
			return -1;
		}
		for( int i = 0; i < _interactableObjects.Count; ++i ) {
			var interactable = _interactableObjects[i];
			if( interactable.HasInteraction ) {
				interactable.HasInteraction = false;
				interactable.CanInteract = false;
				Debug.Log($"current object id {interactable.Id}");
				return interactable.Id;
			}
		}
		return -1;
		/*
		for( int i = 1; i <= 9; ++i ) {
			if( Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i) ) {
				return i;
			}
		}
		return -1;
		*/
	}

	private void ContinueStory(PlayerInput? input = null) {
		if( input != null && !input.Value.Interact )
			return;
		string dialogue = _story.Continue();
		StartCoroutine(_dialogueView.Speak(dialogue));
	}

	private void SkipStory() {
		ContinueStory();
	}

	private int FindChoiceIndex(int itemIndex) {
		string item = _items[itemIndex];
		var choices = _story.currentChoices.Select(x => x.text).ToList();
		return choices.IndexOf(item);
	}

	private void OfferStoryOptions(PlayerInput input) {
		if( _dialogueView.Choices.Count != _story.currentChoices.Count && input.Interact ) {
			Mode = SceneMode.Default;
			_dialogueView.Choices = _story.currentChoices.Select(x => x.text).ToList();
		}
		else if( input.Selection >= 0 ) {
			int choiceIndex = FindChoiceIndex(input.Selection);
			_story.ChooseChoiceIndex(choiceIndex);
			string dialogue = _story.Continue();
			Mode = SceneMode.Dialogue;
			StartCoroutine(_dialogueView.Speak(dialogue));
		}
	}

	private void PlaySfx(string name) {
		Debug.Log($"{nameof(PlaySfx)}(name: {name}");
		_sfxAudioSource.PlayOneShot(_sounds[name]);
	}
}