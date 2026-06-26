using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHP = 3;
    [SerializeField] private int currentHP;

    [Header("Invincibility Frames")]
    [SerializeField] private float iFrameDuration = 1.5f;
    private float iFrameTimer;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;
    private float knockbackTimer;

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay = 2f;

    // Components
    private Rigidbody2D rb;
    private PlayerMovement movement;
    private Animator anim;

    // State
    private bool isDead;
    private bool isKnockedBack;

    // Events — HUDManager listens to these
    public System.Action<int, int> OnHealthChanged;  // currentHP, maxHP
    public System.Action OnDeath;
    public System.Action OnRespawn;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        currentHP = maxHP;
    }

    private void Start()
    {
        // Fire after all OnEnable subscriptions are done
        OnHealthChanged?.Invoke(currentHP, maxHP);
    }

    private void Update()
    {
        if (iFrameTimer > 0f)   iFrameTimer   -= Time.deltaTime;
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f) isKnockedBack = false;
        }
    }

    // ─── IDamageable ─────────────────────────────────────────────────────────

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        if (iFrameTimer > 0f) return;

        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        Debug.Log($"[PlayerHealth] TakeDamage — HP now {currentHP}, OnHealthChanged listeners: {OnHealthChanged?.GetInvocationList().Length}");

        OnHealthChanged?.Invoke(currentHP, maxHP);

        if (currentHP <= 0)
        {
            Die();
            return;
        }

        // Start iFrames and knockback
        iFrameTimer = iFrameDuration;
        ApplyKnockback();

        if (anim != null) anim.SetTrigger("hurt");
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        OnHealthChanged?.Invoke(currentHP, maxHP);
    }

    // ─── Knockback ───────────────────────────────────────────────────────────

    private void ApplyKnockback()
    {
        if (rb == null) return;

        isKnockedBack = true;
        knockbackTimer = knockbackDuration;

        // Push away from damage source — opposite of facing direction
        float dir = movement != null && movement.IsFacingRight() ? -1f : 1f;
        rb.linearVelocity = new Vector2(dir * knockbackForce, knockbackForce * 0.5f);
    }

    // ─── Death / Respawn ─────────────────────────────────────────────────────

    private void Die()
    {
        isDead = true;
        OnDeath?.Invoke();

        if (anim != null) anim.SetTrigger("die");

        // Hide after a short delay so death animation plays first
        Invoke(nameof(HidePlayer), 0.5f);
        Invoke(nameof(Respawn), respawnDelay);
    }

    private void HidePlayer()
    {
        gameObject.SetActive(false);
    }

    private void Respawn()
    {
        // Reposition before showing
        if (respawnPoint != null)
            transform.position = respawnPoint.position;

        gameObject.SetActive(true);

        isDead = false;
        currentHP = maxHP;
        iFrameTimer = iFrameDuration; // brief iFrames on respawn so you don't immediately get hit
        isKnockedBack = false;

        OnHealthChanged?.Invoke(currentHP, maxHP);
        OnRespawn?.Invoke();

        if (anim != null) anim.SetTrigger("respawn");
    }

    // ─── Public Accessors ────────────────────────────────────────────────────

    public int GetCurrentHP() => currentHP;
    public int GetMaxHP() => maxHP;
    public bool IsDead() => isDead;
    public bool IsInvincible() => iFrameTimer > 0f;
    public bool IsKnockedBack() => isKnockedBack;

    // Called by CheckpointManager when a new checkpoint is reached
    public void SetRespawnPoint(Transform point) => respawnPoint = point;
}