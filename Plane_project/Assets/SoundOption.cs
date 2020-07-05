using UnityEngine;

public class SoundOption : MonoBehaviour
{
	public enum SoundType
	{
		SoundEffect,
		BackgroundMusic
	};

    public string path;
	public SoundType type;
	public AudioSource mainAudioItem;

	public float volRatio;
	public bool slowMo = false;
}