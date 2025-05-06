using System.Collections;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private bool crouching = false;
    private bool sprinting = false;
    private Animator anim;

    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 3f;
    public float gravity = -9.8f;
    public float jumpHeight = 1.5f;

    private float currentSpeed;
    private Vector3 previousPosition;

    private float s = 0f;  // Movement speed tracker

    private bool isIdle = false;
    private bool isWalking = false;
    private bool isRunning = false;
    private bool isJumping = false;
    private bool isCrouched = false;

    private float idleCheckTimer = 0f;
    private float idleThreshold = 0.2f;
    private bool wasMoving = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        currentSpeed = walkSpeed;
        previousPosition = transform.position;
    }

    void Update()
    {
        float movementDistance = Vector3.Distance(previousPosition, transform.position);
        s = movementDistance / Time.deltaTime;

        isGrounded = controller.isGrounded;

        if (s > 0.000001f)
        {
            idleCheckTimer = 0f;
            wasMoving = true;
        }
        else if (wasMoving)
        {
            idleCheckTimer += Time.deltaTime;
            if (idleCheckTimer >= idleThreshold)
            {
                wasMoving = false;
            }
        }

        if (!wasMoving)
        {
            isIdle = true;
            isRunning = false;
            isWalking = false;
        }
        else if ((currentSpeed == walkSpeed || currentSpeed == crouchSpeed))
        {
            isIdle = false;
            isRunning = false;
            isWalking = true;
        }
        else if (currentSpeed == sprintSpeed && movementDistance > 0.15f)
        {
            isIdle = false;
            isRunning = true;
            isWalking = false;
        }

        isCrouched = crouching;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
            isJumping = false;
        }

        anim.SetBool("IsIdle", isIdle);
        anim.SetBool("IsWalking", isWalking);
        anim.SetBool("IsRunning", isRunning);
        anim.SetBool("IsCrouched", isCrouched);
        anim.SetBool("IsJumping", isJumping);

        previousPosition = transform.position;
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = transform.TransformDirection(new Vector3(input.x, 0, input.y));
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (crouching)
        {
            Uncrouch();
        }

        if (isGrounded)
        {
            isJumping = true;
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            StartCoroutine(ResetJumpAfterLanding());
        }
    }

    private IEnumerator ResetJumpAfterLanding()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => controller.isGrounded);
        isJumping = false;
    }

    public void Crouch()
    {
        if (sprinting) Sprint();

        if (!crouching)
        {
            crouching = true;
            controller.height = 5f;
            controller.radius = 2.5f;
            controller.center = new Vector3(0, 2.75f, 0.25f);
            currentSpeed = crouchSpeed;
        }
        else
        {
            Uncrouch();
        }

        isCrouched = crouching;
        anim.SetBool("IsCrouching", crouching);
    }

    private void Uncrouch()
    {
        crouching = false;
        controller.height = 6.31f;
        controller.radius = 1.44f;
        controller.center = new Vector3(0.04f, 3.1f, 0.16f);
        currentSpeed = walkSpeed;
        anim.SetBool("IsCrouching", false);
    }

    public void Sprint()
    {
        if (crouching) Uncrouch();

        sprinting = !sprinting;
        currentSpeed = sprinting ? sprintSpeed : walkSpeed;
        anim.SetBool("IsSprinting", sprinting);
    }

    public bool IsSprinting()
    {
        return sprinting;
    }
}
