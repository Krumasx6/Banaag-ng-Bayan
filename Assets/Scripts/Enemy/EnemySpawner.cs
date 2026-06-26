using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;   // drag enemy prefabs here
    [SerializeField] private Transform[] spawnPoints;     // off-screen spawn positions
    [SerializeField] private int enemyCount = 3;          // how many to spawn per trigger

    [Header("Trigger Settings")]
    [SerializeField] private LayerMask playerLayer;

    // State
    private bool hasSpawned;

    // ─── Trigger ─────────────────────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasSpawned) return;
        if (((1 << other.gameObject.layer) & playerLayer) == 0) return;

        hasSpawned = true;
        SpawnEnemies();
    }

    // ─── Spawn ───────────────────────────────────────────────────────────────

    private void SpawnEnemies()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        for (int i = 0; i < enemyCount; i++)
        {
            // Pick a random prefab and spawn point
            GameObject prefab     = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Transform  spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            if (prefab == null || spawnPoint == null) continue;

            Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        }
    }

    // ─── Gizmos ──────────────────────────────────────────────────────────────

    private void OnDrawGizmos()
    {
        // Draw trigger zone in blue
        Gizmos.color = hasSpawned ? Color.gray : Color.cyan;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            Gizmos.DrawWireCube(transform.position, col.bounds.size);

        // Draw spawn points in red
        if (spawnPoints == null) return;
        foreach (Transform sp in spawnPoints)
        {
            if (sp == null) continue;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(sp.position, 0.3f);
            Gizmos.DrawLine(transform.position, sp.position);
        }
    }
}