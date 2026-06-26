using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int healAmount = 1;
    [SerializeField] private GameObject pickupEffectPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null) return;

        // Don't pick up if already full health
        if (playerHealth.GetCurrentHP() >= playerHealth.GetMaxHP()) return;

        playerHealth.Heal(healAmount);
        SpawnEffect();
        Destroy(gameObject);
    }

    private void SpawnEffect()
    {
        if (pickupEffectPrefab == null) return;
        Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
    }
}