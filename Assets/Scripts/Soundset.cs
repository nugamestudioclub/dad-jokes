using System;
using UnityEngine;

[Serializable]
public struct SoundEffect {
	[field: SerializeField]
	public string Name { get; private set; }

	[field: SerializeField]
	public AudioClip AudioClip { get; private set; }
}

[CreateAssetMenu(fileName = "Soundset", menuName = "ScriptableObjects/Soundset")]
public class Soundset : ScriptableObject {
	public SoundEffect BabyNoise1;
	public SoundEffect BabyNoise2;
	public SoundEffect BabyNoise3;
	public SoundEffect BabyNoise4;
	public SoundEffect BabyLaugh1;
	public SoundEffect BabyLaugh2;
}