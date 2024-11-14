using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BaseFeature : MonoBehaviour
{
    public bool SFXAudioSourceCreated { get; set; }
    [field: SerializeField]
    public AudioClip AudioClipForOnStarted { get; set; }

    [field: SerializeField]
    public AudioClip AudioClipForOnStopped { get; set; }
    private AudioSource audioSource;
    [SerializeField]
    public FeatureUsage featureUsage = FeatureUsage.Once;
    protected virtual void Awake()
    {
        CreateSFXAudioSource();
    }
    private void CreateSFXAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (gameObject.GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        SFXAudioSourceCreated = true;
    }
    protected void PlayOnStarted()
    {
        if (SFXAudioSourceCreated && AudioClipForOnStarted != null)
        {
            audioSource.clip = AudioClipForOnStarted;
            audioSource.Play();
        }
    }
    protected void PlayOnEnded()
    {
        if (SFXAudioSourceCreated && AudioClipForOnStopped != null)
        {
            audioSource.clip = AudioClipForOnStopped;
            audioSource.Play();
        }
    }
    protected void PlayAudioClip(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
