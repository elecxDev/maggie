using System.Collections;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioClip footstepSound;  // The footstep sound clip
    public float stepInterval = 0.5f; // Interval between footstep sounds in seconds
    private AudioSource audioSource; // The AudioSource component that will play footsteps
    private float timeSinceLastStep = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
    }

    void Update()
    {
        // Check if the player is moving (this assumes you're controlling movement with the "W" key)
        if (IsPlayerMoving())
        {
            timeSinceLastStep += Time.deltaTime;

            // Play a footstep sound every "stepInterval" seconds
            if (timeSinceLastStep >= stepInterval)
            {
                PlayFootstep();
                timeSinceLastStep = 0f;
            }
        }
    }

    private bool IsPlayerMoving()
    {
        // You can use your character controller's velocity or movement inputs to check for movement
        // For example, if you use the Input system, check if the player is pressing any movement keys
        return Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;
    }

    private void PlayFootstep()
    {
        // Play the footstep sound
        audioSource.PlayOneShot(footstepSound);
    }
}
