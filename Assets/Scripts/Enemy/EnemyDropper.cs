using UnityEngine;

public class EnemyDropper : MonoBehaviour
{
    [System.Serializable]
    public class Drop
    {
        public GameObject prefab;       // the pickup prefab to spawn
        [Range(0f, 1f)]
        public float chance = 0.5f;     // 0 = never, 1 = always
    }

    [Header("Drops")]
    [SerializeField] private Drop[] possibleDrops;

    [Header("Drop Settings")]
    [SerializeField] private float dropUpwardForce = 3f;    // slight pop upward on spawn
    [SerializeField] private float dropSpreadForce = 1.5f;  // slight horizontal spread

    private EnemyBase enemyBase;

    private void Awake()
    {
        enemyBase = GetComponent<EnemyBase>();
    }

    private void OnEnable()
    {
        if (enemyBase != null)
            enemyBase.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        if (enemyBase != null)
            enemyBase.OnDeath -= HandleDeath;
    }

    // ─── Death Handler ────────────────────────────────────────────────────────

    private void HandleDeath()
    {
        foreach (Drop drop in possibleDrops)
        {
            if (drop.prefab == null) continue;

            // Roll the chance
            if (Random.value <= drop.chance)
                SpawnDrop(drop.prefab);
        }
    }

    // ─── Spawn ───────────────────────────────────────────────────────────────

    private void SpawnDrop(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity);

        // Give it a little pop so it doesn't just appear flat on the ground
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float randomX = Random.Range(-dropSpreadForce, dropSpreadForce);
            rb.AddForce(new Vector2(randomX, dropUpwardForce), ForceMode2D.Impulse);
        }
    }
}