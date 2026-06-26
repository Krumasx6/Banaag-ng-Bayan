using UnityEngine;

public class EnemyRanged : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float preferredRange = 5f;   // ideal shooting distance
    [SerializeField] private float minRange = 2.5f;       // backs away if player gets closer than this

    [Header("Ranged Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 2f;

    [Header("Player Detection")]
    [SerializeField] private LayerMask playerLayer;

    // State
    private Transform player;
    private float fireCooldown;

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

        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer < minRange)
            BackAway();             // too close — back away
        else if (distToPlayer > preferredRange)
            ChasePlayer();          // too far — close the gap
        else
            StandAndShoot();        // sweet spot — stop and fire

        FlipToPlayer();
        Animate(distToPlayer);
    }

    // ─── Chase ───────────────────────────────────────────────────────────────

    private void ChasePlayer()
    {
        float dir = player.position.x > transform.position.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
    }

    // ─── Back Away ───────────────────────────────────────────────────────────

    private void BackAway()
    {
        float dir = player.position.x > transform.position.x ? -1f : 1f;
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
    }

    // ─── Stand and Shoot ─────────────────────────────────────────────────────

    private void StandAndShoot()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (fireCooldown > 0f) return;
        if (projectilePrefab == null) return;

        fireCooldown = fireRate;
        if (anim != null) anim.SetTrigger("attack");
        Shoot();
    }

    private void Shoot()
    {
        Transform point = firePoint != null ? firePoint : transform;
        Vector2 dir = (player.position - point.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, point.position, Quaternion.identity);
        EnemyProjectile p = proj.GetComponent<EnemyProjectile>();
        if (p != null) p.Init(dir);
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
        anim.SetBool("InRange", distToPlayer <= preferredRange && distToPlayer >= minRange);
    }

    // ─── Override Hit ────────────────────────────────────────────────────────

    protected override void OnHit()
    {
        base.OnHit();
    }

    // ─── Gizmos ──────────────────────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, preferredRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minRange);
    }
}