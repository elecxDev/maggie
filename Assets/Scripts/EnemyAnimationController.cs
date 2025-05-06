using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float speed = rb.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        // Example: Check if enemy is in the air
        bool isJumping = !Physics.Raycast(transform.position, Vector3.down, 1.1f);
        animator.SetBool("IsJumping", isJumping);
    }
}