using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAudio : NetworkBehaviour
{
    AudioSource shootingAudioSource;
    AudioSource xpGainAudioSource;
    AudioSource levelUpAudioSource;
    [SerializeField] AudioClip gunshotSound;
    [SerializeField] AudioClip xpGainSound;
    [SerializeField] AudioClip levelUpSound;

    PlayerLeveling leveling;

    void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        shootingAudioSource = audioSources[0];
        xpGainAudioSource = audioSources[1];
        levelUpAudioSource = audioSources[2];

        leveling = transform.GetComponent<PlayerLeveling>();
        UpdateAudio();
    }

    public void UpdateAudio()
    {
        if (leveling != null)
        {
            if (leveling.Level < 20)
            {
                levelUpAudioSource.pitch = (float)(1f - (0.03 * (leveling.Level - 1)));
            }
            else
            {
                levelUpAudioSource.pitch = 0.4f;
            }
        }
        
    }

    public void PlayGunshotSound()
    {
        if (gunshotSound != null) 
        {
            shootingAudioSource.PlayOneShot(gunshotSound);
        }
    }

    public void PlayXpGainSound()
    {
        if (xpGainSound != null)
        {
            xpGainAudioSource.PlayOneShot(xpGainSound);
        }
    }

    public void PlayLevelUpSound()
    {
        if (levelUpSound)
        {
            levelUpAudioSource.PlayOneShot(levelUpSound);
        }
    }


}
