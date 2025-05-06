using UnityEngine;
using UnityEngine.UI;

public class PlayerSprint : MonoBehaviour
{
    private PlayerMotor playerMotor;
    public Image staminaBarFill; // Reference to the Image component of the stamina bar fill
    public float maxStamina = 5f;
    private float currentStamina;
    public float staminaDrainRate = 1f;
    public float staminaRegenRate = 0.5f;
    private bool isExhausted = false;
    public PlayerMotor playermotor;

    void Start()
    {
        playerMotor = GetComponent<PlayerMotor>();
        currentStamina = maxStamina;
        staminaBarFill.fillAmount = currentStamina / maxStamina; // Initialize the stamina bar fill
    }

    void Update()
    {
        // Check if the player is sprinting
        bool isSprinting = playerMotor.IsSprinting();

        // Handle stamina depletion or regeneration based on whether the player is sprinting
        if (isSprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isExhausted = true;
                playerMotor.Sprint(); // Stop sprinting when exhausted
            }
        }
        else if (!isSprinting)
        {
            // Only regenerate stamina when not sprinting and not exhausted
            currentStamina += staminaRegenRate * Time.deltaTime;

            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
            }
        }

        // Update the stamina bar fill amount
        staminaBarFill.fillAmount = currentStamina / maxStamina;
    }
}