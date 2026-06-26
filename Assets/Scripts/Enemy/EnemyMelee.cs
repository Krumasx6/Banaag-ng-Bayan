using UnityEngine;

public class EnemyMelee : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float backoffDistance = 1.5f;  // backs away after attacking

    [Header("Melee Attack")]
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private Transform attackPoint;

    [Header("Player Detection")]
    [SerializeField] private LayerMask playerLayer;

    // State
    private Transform player;
    private float attackCooldownTimer;
    private bool isBackingOff;
    private float backoffTimer;
    [SerializeField] private float backoffDuration = 0.5f;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (isDead) return;
        if (player == null) return;

        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (isBackingOff)
        {
            BackOff();
            backoffTimer -= Time.deltaTime;
            if (backoffTimer <= 0f) isBackingOff = false;
        }
        else if (distToPlayer <= attackRange)
            Attack();
        else
            ChasePlayer();

        FlipToPlayer();
        Animate(distToPlayer);
    }

    // ─── Chase ───────────────────────────────────────────────────────────────

    private void ChasePlayer()
    {
        float dir = player.position.x > transform.position.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
    }

    // ─── Back Off ────────────────────────────────────────────────────────────

    private void BackOff()
    {
        // Move away from player
        float dir = player.position.x > transform.position.x ? -1f : 1f;
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
    }

    // ─── Attack ──────────────────────────────────────────────────────────────

    private void Attack()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (attackCooldownTimer > 0f) return;

        attackCooldownTimer = attackCooldown;

        if (anim != null) anim.SetTrigger("attack");

        // Hit player
        Transform point = attackPoint != null ? attackPoint : transform;
        Collider2D hit = Physics2D.OverlapCircle(point.position, attackRange, playerLayer);
        if (hit != null)
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            damageable?.TakeDamage(attackDamage);
        }

        // Back off after attacking
        isBackingOff = true;
        backoffTimer = backoffDuration;
    }

    // ─── Flip ────────────────────────────────────────────────────────────────

    private void FlipToPlayer()
    {
        if (sr == null || player == null) return;
        sr.flipX = player.position.x < transform.position.x;
    }

    // ─── Animation ───────────────────────────────────────────────────────────

    private void Animate(float distToPlayer)
    {
        if (anim == null) return;
        anim.SetFloat("MoveSpeed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool("InRange", distToPlayer <= attackRange);
        anim.SetBool("IsBackingOff", isBackingOff);
    }

    // ─── Contact Damage ──────────────────────────────────────────────────────

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        IDamageable damageable = other.gameObject.GetComponentInParent<IDamageable>();
        damageable?.TakeDamage(attackDamage);
    }

    // ─── Override Hit ────────────────────────────────────────────────────────

    protected override void OnHit()
    {
        base.OnHit();
    }

    // ─── Gizmos ──────────────────────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        Transform point = attackPoint != null ? attackPoint : transform;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(point.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, backoffDistance);
    }
}