using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject soundManager = GameObject.Find("SoundManager");
                if (soundManager == null)
                {
                    soundManager = new GameObject("SoundManager");
                    soundManager.AddComponent<SoundManager>();

                    instance = soundManager.GetComponent<SoundManager>();
                    instance.Init();
                }
            }
            return instance;
        }
    }

    private AudioSource audioSourceBGM;
    private AudioSource audioSourceSE;

    private void Init()
    {
        GameObject audioSource = new GameObject("BGM");
        audioSourceBGM = audioSource.AddComponent<AudioSource>();
        audioSourceBGM.transform.parent = instance.transform;
        audioSource = new GameObject("SE");
        audioSourceSE = audioSource.AddComponent<AudioSource>();
        audioSourceSE.transform.parent = instance.transform;

        audioSourceBGM.playOnAwake = false;
        audioSourceBGM.loop = true;
        audioSourceSE.playOnAwake = false;
        audioSourceSE.loop = false;
    }

    public void Play(AudioClip clip, SoundType type)
    {
        if (type == SoundType.BGM)
        {
            audioSourceBGM.clip = clip;
            audioSourceBGM.Play();
        }
        else
        {
            audioSourceSE.clip = clip;
            audioSourceSE.Play();
        }
    }

    public void TurnOnBGM(bool value)
    {
        if (value)
        {
            audioSourceBGM.volume = 1;
        }
        else
        {
            audioSourceBGM.volume = 0;
        }
    }

    public void TurnOnSE(bool value)
    {
        if (value)
        {
            audioSourceSE.volume = 1;
        }
        else
        { 
            audioSourceSE.volume = 0;
        }
    }

    public void Stop(bool value)
    {
        if (value)
        {
            audioSourceBGM.Pause();
            audioSourceSE.Pause();
        }
        else
        {
            audioSourceBGM.UnPause();
            audioSourceSE.UnPause();
        }
    }
}

public enum SoundType
{
    BGM,
    SE
}
