using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int attackDamage = 10;
    private PlayerHealth playerHealth;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    public void AttackPlayer()
    {
        if (playerHealth != null)
        {
            Debug.Log("Enemy attacks! Dealing " + attackDamage + " damage.");
            playerHealth.TakeDamage(attackDamage);
        }
        else
        {
            Debug.LogError("PlayerHealth script not found on Player!");
        }
    }
}
