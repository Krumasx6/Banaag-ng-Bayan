using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator anim;
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7.5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float doubleJumpForce = 12f;
    [SerializeField] private bool maximumJump = 2;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Crouch Colliders")]
    [SerializeField] private GameObject standingColliderObj;
    [SerializeField] private GameObject crouchColliderObj;

    [Header("Crouch")]
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 14f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.2f;

    // State
    private Vector2 input;
    private Vector2 lastMoveDirection = Vector2.right;
    private bool facingRight = true;
    private bool isGrounded;
    private bool hasDoubleJumped;
    private bool isCrouching;
    private bool isDashing;
    private float dashTimer;
    private float dashDirection;
    private bool dashBuffered;
    private float dashCooldownTimer;

    private float defaultGravityScale;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
        SetCrouchCollider(false);
    }

    private void Update()
    {
        CheckGround();
        ProcessInput();
        HandleCrouch();
        HandleJump();
        HandleDash();
        Animate();
        FlipCheck();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
            return;
        }

        float speed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;
        rb.linearVelocity = new Vector2(input.x * speed, rb.linearVelocity.y);
    }

    // ─── Input ───────────────────────────────────────────────────────────────

    private void ProcessInput()
    {
        if (isDashing) return;

        float moveX = 0f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX =  1f;

        input = new Vector2(moveX, 0f);
        if (moveX != 0f) lastMoveDirection = input;
    }

    // ─── Crouch (S) ──────────────────────────────────────────────────────────

    private void HandleCrouch()
    {
        // Only crouch when grounded — S in air is aim down, not crouch
        bool wantsCrouch = Input.GetKey(KeyCode.S) && isGrounded;

        if (wantsCrouch && !isCrouching)
        {
            isCrouching = true;
            SetCrouchCollider(true);
        }
        else if (!wantsCrouch && isCrouching)
        {
            isCrouching = false;
            SetCrouchCollider(false);
        }
    }

    private void SetCrouchCollider(bool crouching)
    {
        if (standingColliderObj != null) standingColliderObj.SetActive(!crouching);
        if (crouchColliderObj != null)   crouchColliderObj.SetActive(crouching);
    }

    // ─── Jump (K) — S never blocks jump ──────────────────────────────────────

    private void HandleJump()
    {
        if (!Input.GetKeyDown(KeyCode.K)) return;

        // Stand up first if crouching, then jump
        if (isCrouching)
        {
            isCrouching = false;
            SetCrouchCollider(false);
        }

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            hasDoubleJumped = false;
        }
        else if (!hasDoubleJumped)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
            hasDoubleJumped = true;
        }
    }

    // ─── Dash (I) — Cuphead style, air allowed, no cooldown ─────────────────

    private void HandleDash()
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        // Buffer the input — if pressed while dashing or on cooldown, store it
        if (Input.GetKeyDown(KeyCode.I))
            dashBuffered = true;

        // Clear buffer if key was released — no phantom dash
        if (Input.GetKeyUp(KeyCode.I))
            dashBuffered = false;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                rb.gravityScale = defaultGravityScale;
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

                // Fire buffered dash after cooldown clears
                if (dashBuffered && dashCooldownTimer <= 0f)
                {
                    dashBuffered = false;
                    StartDash();
                }
            }
            return;
        }

        if (dashBuffered && dashCooldownTimer <= 0f)
        {
            dashBuffered = false;
            StartDash();
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        dashDirection = facingRight ? 1f : -1f;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // kill fall momentum
    }

    // ─── Ground Check ────────────────────────────────────────────────────────

    private void CheckGround()
    {
        if (groundCheck == null) return;
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && !wasGrounded) hasDoubleJumped = false;
    }

    // ─── Flip ────────────────────────────────────────────────────────────────

    private void FlipCheck()
    {
        if (input.x > 0f && !facingRight) Flip();
        else if (input.x < 0f && facingRight) Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    // ─── Animation ───────────────────────────────────────────────────────────

    private void Animate()
    {
        if (anim == null) return;

        anim.SetFloat("MoveX", input.x);
        anim.SetFloat("MoveMagnitude", Mathf.Abs(input.x));
        anim.SetFloat("LastMoveX", lastMoveDirection.x);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("hasDoubleJumped", hasDoubleJumped);
        anim.SetBool("isCrouching", isCrouching);
        anim.SetBool("isDashing", isDashing);
        anim.SetFloat("VerticalVelocity", rb.linearVelocity.y);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    // ─── Public Accessors ────────────────────────────────────────────────────

    public bool IsGrounded() => isGrounded;
    public bool IsCrouching() => isCrouching;
    public bool IsDashing() => isDashing;
    public bool IsFacingRight() => facingRight;
}