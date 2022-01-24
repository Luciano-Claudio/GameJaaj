using UnityEngine;

public class AudioM : MonoBehaviour
{
    private AudioSource sounds;
    public AudioClip BtnClip;
    public static AudioM inst = null;

    void Awake()
    {
        if (inst == null)
            inst = this;
        else if (inst != this)
            Destroy(gameObject);
        sounds = gameObject.GetComponent<AudioSource>();
    }

    public void PlayAudio(AudioClip clipAudio)
    {
        sounds.clip = clipAudio;
        sounds.Play();
    }

    public void BtnSound()
    {
        sounds.clip = BtnClip;
        sounds.Play();
    }
}
