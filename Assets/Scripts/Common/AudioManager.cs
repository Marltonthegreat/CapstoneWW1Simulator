using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioSource sfxAudioSource;

    const string MASTER_VOLUME = "Master";
    const string SFX_VOLUME = "SFX";
    const string MUSIC_VOLUME = "Music";

    public float masterVolume
    {
        get
        {
            audioMixer.GetFloat(MASTER_VOLUME, out float dB);
            return DBToLinear(dB);
        }
        set
        {
            float dB = LinearToDB(value);
            audioMixer.SetFloat(MASTER_VOLUME, dB);
            PlayerPrefs.SetFloat(MASTER_VOLUME, value);
        }
    }

    public float sfxVolume
    {
        get
        {
            audioMixer.GetFloat(SFX_VOLUME, out float dB);
            return DBToLinear(dB);
        }
        set
        {
            float dB = LinearToDB(value);
            audioMixer.SetFloat(SFX_VOLUME, dB);
            PlayerPrefs.SetFloat(SFX_VOLUME, value);
        }
    }

    public float musicVolume
    {
        get
        {
            audioMixer.GetFloat(MUSIC_VOLUME, out float dB);
            return DBToLinear(dB);
        }
        set
        {
            float dB = LinearToDB(value);
            audioMixer.SetFloat(MUSIC_VOLUME, dB);
            PlayerPrefs.SetFloat(MUSIC_VOLUME, value);
        }
    }

    void Start()
    {
        masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME, .5f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME, .5f);
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME, .5f);
    }

    public void PlaySFX(AudioClip clip)
	{
        sfxAudioSource.PlayOneShot(clip);
	}
    
    public void PlayAmbientSFX(AudioClip clip)
	{
        sfxAudioSource.PlayOneShot(clip, 0.125f);
	}
    
    public void PlayMusic(AudioClip clip, bool loop = false)
	{
        musicAudioSource.clip = clip;
        musicAudioSource.loop = loop;
        musicAudioSource.Play();
	}

    public static float LinearToDB(float linear)
	{
        return (linear != 0) ? 20.0f * Mathf.Log10(linear) : -144.0f;
    }

    public static float DBToLinear(float dB)
	{
        return Mathf.Pow(10.0f, dB / 20.0f);
    }
}

