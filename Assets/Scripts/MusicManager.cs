using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource parkourMusic; // Audio Source for parkour music
    public AudioSource bossMusic;    // Audio Source for boss music
    public AudioSource winMusic;
    public AudioSource lossMusic;
    public AudioSource EnemyAttack;
    public AudioClip enemyAttackSound;
    public float fadeDuration = 2f;  // Duration of the fade effect

    void Start()
    {
        // Start with parkour music playing
        bossMusic.Stop();
        winMusic.Stop();
        lossMusic.Stop();
        parkourMusic.Play();
        parkourMusic.loop = true; // Set loop to true if you want continuous music
    }

    // Call this function to switch to boss music
    public void SwitchToBossMusic()
    {
        StartCoroutine(FadeOutAndIn(parkourMusic, bossMusic, fadeDuration)); // Fade out parkour music and fade in boss music
    }

    private IEnumerator FadeOutAndIn(AudioSource currentMusic, AudioSource newMusic, float fadeDuration)
    {
        // Fade out the current music
        float startVolume = currentMusic.volume;

        while (currentMusic.volume > 0)
        {
            currentMusic.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        currentMusic.Stop(); // Stop the current music once it's fully faded out
        currentMusic.volume = startVolume; // Reset volume for future use

        // Fade in the new music
        newMusic.Play();
        newMusic.volume = 0;

        while (newMusic.volume < 1)
        {
            newMusic.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }
    }

    public void SwitchToWinMusic()
    {
        StartCoroutine(FadeOutAndIn(bossMusic, winMusic, 2));
    }

    public void SwitchToLossMusic()
    {
        StartCoroutine(FadeOutAndIn(bossMusic, lossMusic, 2));
    }
    public void PlayScream()
    {
        // Play the footstep sound
        EnemyAttack.PlayOneShot(enemyAttackSound);
    }
}
