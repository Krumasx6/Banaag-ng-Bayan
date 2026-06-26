using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 3f;

    [Header("Layers")]
    [SerializeField] private LayerMask destroyOnLayers;  // set to Ground, Wall etc.

    [Header("Effects")]
    [SerializeField] private GameObject hitEffectPrefab;

    private Vector2 direction;
    private bool initialized;

    private void Start()
    {
        Destroy(gameObject, lifetime);
        if (initialized) AlignToDirection();
    }

    public void Init(Vector2 dir)
    {
        direction = dir.normalized;
        initialized = true;
        AlignToDirection();
    }

    private void Update()
    {
        if (!initialized) return;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    // ─── Collision ───────────────────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore self tag
        if (other.CompareTag("EnemyProjectile")) return;

        // Ignore player projectiles
        if (other.CompareTag("PlayerProjectile")) return;

        // Hit the player — damage and destroy
        if (other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            damageable?.TakeDamage(damage);
            SpawnHitEffect();
            Destroy(gameObject);
            return;
        }

        // Hit walls/ground — destroy
        bool shouldDestroy = ((destroyOnLayers.value & (1 << other.gameObject.layer)) != 0);
        if (shouldDestroy)
        {
            SpawnHitEffect();
            Destroy(gameObject);
            return;
        }

        // Hit anything else with IDamageable (future: destructibles etc.)
        IDamageable other_damageable = other.GetComponent<IDamageable>();
        if (other_damageable != null)
        {
            SpawnHitEffect();
            Destroy(gameObject);
        }
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private void AlignToDirection()
    {
        if (direction == Vector2.zero) return;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void SpawnHitEffect()
    {
        if (hitEffectPrefab == null) return;
        Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
    }
}