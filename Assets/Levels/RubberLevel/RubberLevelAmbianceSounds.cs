using System;
using UnityEngine;

public class RubberLevelAmbianceSounds : MonoBehaviour
{
    [Serializable]
    private class MadhouseSfx
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private MadhouseSfx[] sfxClips;

    [Range(0, 1f)][SerializeField] private float probabilityOfMadhouseSfx = 0.1f;
    [SerializeField] private float minHitSpeedForMadhouseSfx = 8f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var hitSpeed = collision.relativeVelocity.magnitude;
        if (UnityEngine.Random.value > probabilityOfMadhouseSfx || hitSpeed < minHitSpeedForMadhouseSfx)
            return;

        var sfx = sfxClips[UnityEngine.Random.Range(0, sfxClips.Length)];
        audioSource.PlayOneShot(sfx.clip, sfx.volume);
    }
}
