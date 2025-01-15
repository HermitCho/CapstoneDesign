using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{

    [SerializeField] private AudioClip[] soundClips;

    public void GetSound(AudioSource currentAudio, int index, float volume = 1f, float? pitch = null, float? speed = null)
    {
        if (index < 0 || index >= soundClips.Length)
        {
            Debug.LogWarning($"SoundManager: Invalid index {index} for sound clips array.");
            return;
        }

        if (pitch.HasValue)
        {
            currentAudio.pitch = Mathf.Clamp(pitch.Value, 0.1f, 3.0f); // Limit pitch to a reasonable range
        }
        else
        {
            currentAudio.pitch = 1.0f; // Default pitch
        }

        currentAudio.volume = Mathf.Clamp01(volume);
        currentAudio.clip = soundClips[index];

        if (speed.HasValue && currentAudio.clip != null)
        {
            currentAudio.pitch *= Mathf.Clamp(speed.Value, 0.1f, 3.0f);
        }

        currentAudio.Play();

    }
}

