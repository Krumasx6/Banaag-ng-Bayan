using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Data")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int ammo = 30;
    [SerializeField] private float fireRate = 0.3f;
    [SerializeField] private int damage = 1;

    [Header("Weapon Info")]
    [SerializeField] private string weaponName = "Weapon";
    [SerializeField] private Sprite weaponIcon;

    [Header("Effects")]
    [SerializeField] private GameObject pickupEffectPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerCombat combat = other.GetComponentInParent<PlayerCombat>();
        if (combat == null) return;

        combat.EquipSecondary(projectilePrefab, ammo, fireRate, damage);

        SpawnEffect();
        Destroy(gameObject);
    }

    private void SpawnEffect()
    {
        if (pickupEffectPrefab == null) return;
        Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
    }
}