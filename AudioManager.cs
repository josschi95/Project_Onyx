using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource sourceTheme;
    [SerializeField] private AudioSource sourceFX;
    public Theme[] themes;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of AudioManager found");
            Destroy(this);
            return;
        }
        else instance = this;

        /*
        foreach (Theme s in themes)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        */
    }

    public void Play(string name)
    {
        foreach (Theme t in themes)
        {
            if (t.name == name)
            {
                sourceFX.clip = t.clip;
                sourceFX.Play();
                return;
            }
        }
        //Theme s = Array.Find(themes, sound => sound.name == name);
        //if (s != null) s.source.Play();
        //else Debug.Log(name + "Not Found");
    }
}
