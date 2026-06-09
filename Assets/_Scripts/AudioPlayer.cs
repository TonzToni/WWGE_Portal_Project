using UnityEngine;
using UnityEngine.Audio;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource audioObject;
    public AudioMixer audioMixer;


    private AudioSource audioSource;

    // creating an instance of audio player
    public static AudioPlayer instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void MasterVolume(float level)
    {
        audioMixer.SetFloat("MasterVolume", level);
    }

    public void PlaySound(AudioClip clip, Transform transform, float volume = 1f)
    {
        // spawns audio source into world
        audioSource = Instantiate(audioObject, transform.localPosition, Quaternion.identity);
        //AudioSource audioSource = audioSourceObj.GetComponent<AudioSource>();

        // asigns values to audio source
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();

        // destroys audio source after clip has been played
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }

    public void DestroyAudioSource(AudioClip clip)
    {
        // ignores function is no source is present
        if (audioSource == null) return;

        // destroys audio source if audio clips match, this avoids destroying audio sources that are playing unrelated sounds
        if (audioSource.clip == clip)
            Destroy(audioSource);
    }
}