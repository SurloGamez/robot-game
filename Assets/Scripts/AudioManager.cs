using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(string name)
    {
        Sound toPlay = Array.Find(sounds, sound => sound.name == name);

        GameObject soundObj = new GameObject();
        soundObj.transform.parent = gameObject.transform;
        AudioSource audsr = soundObj.AddComponent<AudioSource>();
        audsr.clip = toPlay.clip;
        audsr.volume = toPlay.volume;
        audsr.pitch = toPlay.pitch;

        Destroy(soundObj, audsr.clip.length + 0.1f);
    }

}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public float volume, pitch;


}


