using UnityEngine;
using System.Collections.Generic;

public class AmbientSoundPlayer : MonoBehaviour
{
    public List<AudioClip> ambientSounds = new List<AudioClip>();

    public float minInterval = 5f;
    public float maxInterval = 15f;

    public float volume = 0.5f;
    public float spatialBlend = 0f;

    private AudioSource audioSource;
    private float timer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = spatialBlend;

        SetNextTimer();
    }

    void Update()
    {
        if (ambientSounds.Count == 0) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            PlayRandomSound();
            SetNextTimer();
        }
    }

    private void PlayRandomSound()
    {
        int randomIndex = Random.Range(0, ambientSounds.Count);
        AudioClip clipToPlay = ambientSounds[randomIndex];

        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay, volume);
        }
    }

    private void SetNextTimer()
    {
        timer = Random.Range(minInterval, maxInterval);
    }
}
