using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    public Transform player;  // Reference to the player
    private NavMeshAgent agent;
    private Animator anim;  // Reference to the animator

    public float chaseRange = 10f; // Distance at which the enemy starts chasing
    public float attackRange = 1.5f; // Distance before attacking the player

    private bool isChasing = false;
    private bool isAttacking = false; // To prevent attack animation from triggering continuously

    private Rigidbody rb;

    public GameObject winScreen;  // Reference to the win screen UI (you can link this via the Inspector)
    private EnemyAttack enemyAttack;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();  // Get the NavMeshAgent component
        anim = GetComponent<Animator>();  // Get the Animator component
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component (for velocity)
        enemyAttack = GetComponent<EnemyAttack>();  // Get EnemyAttack component
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);  // Calculate the distance to the player
        float speed = agent.velocity.magnitude;  // Get the current speed based on the agent's movement

        // Update the animator with the speed (use walking animation for both walking and running)
        anim.SetFloat("Speed", speed);

        // Handle Chase and Movement
        if (distanceToPlayer <= chaseRange)
        {
            if (!isChasing)
            {
                isChasing = true;
            }
            agent.SetDestination(player.position);  // Continuously set the destination to the player's position
        }
        else
        {
            if (isChasing)
            {
                isChasing = false;
                agent.ResetPath();  // Stop the agent's movement if out of range
                anim.SetFloat("Speed", 0);  // Stop the walking animation when not chasing
            }
        }

        // Handle Jumping animation based on velocity
        if (IsJumping())
        {
            anim.SetBool("IsJumping", true);
        }
        else
        {
            anim.SetBool("IsJumping", false);
        }

        // Trigger the attack animation if within attack range, and if not already attacking
        if (distanceToPlayer <= attackRange && !isAttacking)
        {
            StartCoroutine(AttackPlayer());  // Use coroutine to prevent continuous triggering
        }
    }

    // Attack Player with a cooldown to avoid multiple triggers while animation is playing
    private IEnumerator AttackPlayer()
    {
        isAttacking = true;  // Set flag to true to prevent attack trigger during animation

        anim.SetTrigger("Attack");  // Trigger the attack animation

        FindObjectOfType<MusicManager>().PlayScream();

        // Wait for the duration of the attack animation (adjust this to your animation's length)
        yield return new WaitForSeconds(0.5f);  // Adjust this value based on your attack animation duration
        if (enemyAttack != null)
        {
            enemyAttack.AttackPlayer();  // Call the function to deal damage
        }

        yield return new WaitForSeconds(1.5f); // Allow time for attack to finish

        isAttacking = false;  // Reset flag to allow future attacks

        // After the attack animation, return to movement (walking/idle)
        if (agent.velocity.magnitude > 0.1f)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);  // Resume walking animation if moving
        }
        else
        {
            anim.SetFloat("Speed", 0);  // Idle if no movement
        }
    }

    bool IsJumping()
    {
        // Check if the enemy's Rigidbody velocity on the Y-axis is non-zero (meaning it's in the air)
        return Mathf.Abs(rb.velocity.y) > 0.1f;  // Adjust the threshold if needed
    }

    public void Die()
    {
        anim.SetTrigger("Die");  // Trigger the die animation

        // Stop the enemy's movement
        agent.isStopped = true;

        // Optionally, disable the collider to prevent further interaction with the player
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Display the win screen
        if (winScreen != null)
        {
            winScreen.SetActive(true);
        }

        // Start the coroutine to wait and set the position
        StartCoroutine(SetPositionAndDisappear());

        // Destroy the enemy after 10 seconds (this will also remove the GameObject from the scene)
        Destroy(gameObject, 10f);
    }

    private IEnumerator SetPositionAndDisappear()
    {
        // Wait for 0.5 seconds after death animation starts
        yield return new WaitForSeconds(0.5f);

        // Set the enemy's transform Y position to 7.05
        Vector3 newPosition = transform.position;
        newPosition.y = 7.05f;
        transform.position = newPosition;
    }
}
