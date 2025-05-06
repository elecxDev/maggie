using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;  // Player's maximum health
    private int currentHealth;

    public GameManager gameManager;  // Reference to GameManager for GameOver

    public GameObject healthBarUI; // Health bar UI panel
    public Image healthBarFill; // UI Image for health bar display
    private CanvasGroup healthBarCanvasGroup; // CanvasGroup for fade animations

    public float fadeDuration = 1f; // Time to fade in/out the health bar
    public float healthBarFadeOutDuration = 2f; // Fade out duration after full health

    private bool isDead = false; // Flag to check if the enemy is already dead

    public float healRate = 3f; // Healing rate per quarter of a second (3% max health)
    private float healTimer = 0f; // Timer to track time since last damage
    private Coroutine healingCoroutine; // Healing coroutine reference

    void Start()
    {
        currentHealth = maxHealth;
        healthBarCanvasGroup = healthBarUI.GetComponent<CanvasGroup>(); // Get CanvasGroup
        healthBarCanvasGroup.alpha = 0f; // Initially hidden
        healthBarUI.SetActive(true); // Ensure UI is enabled
        StartCoroutine(FadeInHealthBar()); // Fade-in effect at start
    }

    void Update()
    {
        if (currentHealth < maxHealth * 0.75f) // Healing logic only if under 75% health
        {
            // Check if we are allowed to heal (7 seconds without damage)
            if (healTimer >= 7f)
            {
                StartHealing();
            }
            else
            {
                healTimer += Time.deltaTime; // Increment timer if we're not healing
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return; // Prevents extra processing if already dead

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        UpdateHealthBar();

        // Reset the healing timer after taking damage
        healTimer = 0f;

        // Check if player died
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        isDead = true;

        // Trigger the death animation
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("Dead");

        // Destroy the game object after animation finishes
        Destroy(gameObject, 6f);

        // Start fading out the health bar
        StartCoroutine(FadeOutHealthBar());

        // Switch to loss music
        FindObjectOfType<MusicManager>().SwitchToLossMusic();

        // Wait for a few seconds before transitioning to the game over scene
        StartCoroutine(WaitAndSwitchScene());
    }

    private IEnumerator WaitAndSwitchScene()
    {
        // Wait for 4 seconds before switching to the next scene
        yield return new WaitForSeconds(5f);

        // Load the scene with index 1 (the Game Over scene)
        SceneManager.LoadScene(0);
    }

    private void StartHealing()
    {
        // Heal up to 75% of max health
        float targetHealth = maxHealth * 0.75f;

        // Start gradual healing towards 75% health
        StartCoroutine(HealGradually(targetHealth));
    }

    private IEnumerator HealGradually(float targetHealth)
    {
        while (currentHealth < targetHealth)
        {
            // Heal by 3% of max health each 0.25 seconds
            currentHealth = Mathf.Clamp(currentHealth + (int)(maxHealth * 0.03f), 0, (int)targetHealth);
            UpdateHealthBar();
            yield return new WaitForSeconds(1f); // Wait for quarter of a second
        }
    }

    // UI Fade Animations
    private IEnumerator FadeInHealthBar()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            healthBarCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        healthBarCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutHealthBar()
    {
        float elapsedTime = 0f;
        while (elapsedTime < healthBarFadeOutDuration)
        {
            healthBarCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / healthBarFadeOutDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        healthBarCanvasGroup.alpha = 0f;
    }
}
