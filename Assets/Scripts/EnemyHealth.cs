using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public float detectionRadius = 20f;
    public GameObject healthBarUI; // Reference to the health bar UI
    public Image healthBarFill; // The Image component that will show the health bar
    private bool playerInRange = false;
    private CanvasGroup healthBarCanvasGroup; // CanvasGroup to control transparency

    public float fadeDuration = 1f; // Duration of the fade-in effect
    public float damageDuration = 1f; // Duration to smoothly deplete health bar
    public float healthBarFadeOutDuration = 1f; // Duration of the fade-out effect

    private float damageCooldown = 0.5f; // Cooldown time between hits (0.5 seconds)
    private float lastDamageTime = 0f; // Time of last damage taken

    private bool isDead = false; // Flag to check if the enemy is already dead

    private void Start()
    {
        currentHealth = maxHealth;
        healthBarCanvasGroup = healthBarUI.GetComponent<CanvasGroup>(); // Get the CanvasGroup component
        healthBarCanvasGroup.alpha = 0f; // Set initial transparency to 0 (hidden)
        healthBarUI.SetActive(false); // Hide the health bar initially
    }

    private void Update()
    {
        // Check if the player is within detection radius
        float distanceToPlayer = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);

        if (distanceToPlayer <= detectionRadius && !playerInRange)
        {
            playerInRange = true;
            healthBarUI.SetActive(true); // Show health bar when the player is close
            StartCoroutine(FadeInHealthBar()); // Start the fade-in effect
        }
        else if (distanceToPlayer > detectionRadius && playerInRange)
        {
            playerInRange = false;
            healthBarUI.SetActive(false); // Hide health bar if the player moves out of range
        }

        // Check if the enemy is dead and ensure Die() is only called once
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    // Coroutine to fade in the health bar
    private IEnumerator FadeInHealthBar()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            healthBarCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration); // Gradually change alpha
            elapsedTime += Time.deltaTime; // Increase time
            yield return null; // Wait for the next frame
        }
        FindObjectOfType<MusicManager>().SwitchToBossMusic();

        healthBarCanvasGroup.alpha = 1f; // Ensure the final alpha is fully visible
    }

    // This function will be called when the box collides with the enemy
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Box")) // Check if the object colliding is the box
        {
            // Apply damage with each collision
            TakeDamage(maxHealth / 15f); // Each hit deals 1/15th of the max health
            Debug.Log("Box hit! Current Health: " + currentHealth); // Debug log to track damage
        }
    }

    // Handle damage
    private void TakeDamage(float damage)
    {
        // Trigger the health bar deplete effect
        Debug.Log("Taking damage: " + damage); // Debug log for damage
        currentHealth -= damage;

        if (currentHealth < 0) currentHealth = 0; // Ensure health doesn't go below 0

        StartCoroutine(DepleteHealthBar(damage)); // Call the coroutine to update the health bar
    }

    // Coroutine to deplete the health bar gradually
    private IEnumerator DepleteHealthBar(float damage)
    {
        float startHealth = currentHealth;
        float targetFillAmount = currentHealth / maxHealth;
        float startFillAmount = healthBarFill.fillAmount;

        // Gradually update the health bar fill amount
        float elapsedTime = 0f;
        while (elapsedTime < damageDuration)
        {
            healthBarFill.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / damageDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        healthBarFill.fillAmount = targetFillAmount; // Ensure the final fill is correct
    }

    // Handle enemy death
    private void Die()
    {
        isDead = true; // Set the flag to true to prevent multiple calls to Die()

        // Play death animation
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("Dead");

        // Disable enemy movement and AI (Optional)
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<EnemyAI>().enabled = false;

        FindObjectOfType<MusicManager>().SwitchToWinMusic();

        // Start health bar fade-out and destroy enemy after animation
        StartCoroutine(FadeOutHealthBar());
        Destroy(gameObject, 6f); // Wait for the animation and health bar fade-out before destroying
        StartCoroutine(WaitAndSwitchScene());
    }

    // Coroutine to fade out the health bar
    private IEnumerator FadeOutHealthBar()
    {
        float elapsedTime = 0f;

        while (elapsedTime < healthBarFadeOutDuration)
        {
            healthBarCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / healthBarFadeOutDuration); // Fade out
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        healthBarCanvasGroup.alpha = 0f; // Ensure the health bar is fully hidden
    }
    private IEnumerator WaitAndSwitchScene()
    {
        // Wait for 4 seconds before switching to the next scene
        yield return new WaitForSeconds(1f);

        // Load the scene with index 1 (the Game Over scene)
        SceneManager.LoadScene(2);
    }
}
