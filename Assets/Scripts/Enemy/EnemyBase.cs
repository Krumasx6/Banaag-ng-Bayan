using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHP = 3;
    protected int currentHP;

    [Header("Death")]
    [SerializeField] private float deathDelay = 0.5f;
    [SerializeField] private GameObject deathEffectPrefab;

    // Components
    protected Rigidbody2D rb;
    protected Animator anim;
    protected SpriteRenderer sr;

    // State
    protected bool isDead;

    // Events
    public System.Action OnDeath;

    protected virtual void Awake()
    {
        rb  = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr   = GetComponent<SpriteRenderer>();

        currentHP = maxHP;
    }

    // ─── IDamageable ─────────────────────────────────────────────────────────

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        OnHit();

        if (currentHP <= 0)
            Die();
    }

    // ─── Overrideable Hooks ───────────────────────────────────────────────────

    // Called every hit — override in subclass for flash, sound, etc.
    protected virtual void OnHit()
    {
        if (anim != null) anim.SetTrigger("hurt");
    }

    // Called when HP hits 0
    protected virtual void Die()
    {
        isDead = true;
        OnDeath?.Invoke();

        if (anim != null) anim.SetTrigger("die");
        if (rb != null)   rb.linearVelocity = Vector2.zero;

        // Disable collider so bullets pass through dead enemy
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player?.GetComponent<SPManager>()?.GrantKill();

        Invoke(nameof(DestroyEnemy), deathDelay);
    }

    private void DestroyEnemy()
    {
        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }


    public bool IsDead() => isDead;
}