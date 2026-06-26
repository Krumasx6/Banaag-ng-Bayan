using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Info")]
    [SerializeField] private string weaponName = "Weapon";
    [SerializeField] private Sprite weaponIcon;         // shown in HUD weapon slot

    [Header("Firing")]
    [SerializeField] protected float fireRate = 0.2f;
    [SerializeField] protected int damage = 1;

    [Header("Ammo")]
    [SerializeField] protected int maxAmmo = 30;
    [SerializeField] protected int currentAmmo;
    [SerializeField] protected bool infiniteAmmo = false;

    [Header("Projectile")]
    [SerializeField] protected GameObject projectilePrefab;
    // State
    protected float fireCooldown;
    protected bool isEquipped;

    // Events — HUDManager listens to these
    public System.Action<int, int> OnAmmoChanged;   // currentAmmo, maxAmmo

    protected virtual void Awake()
    {
        currentAmmo = maxAmmo;
    }

    protected virtual void Update()
    {
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;
    }

    // ─── Fire ────────────────────────────────────────────────────────────────

    // Called by PlayerCombat — returns true if shot fired
    public bool TryFire(Vector2 direction)
    {
        if (!isEquipped) return false;
        if (fireCooldown > 0f) return false;
        if (!HasAmmo()) return false;

        Fire(direction);
        ConsumeAmmo();
        fireCooldown = fireRate;
        return true;
    }

    protected virtual void Fire(Vector2 direction)
    {
        if (projectilePrefab == null) return;
        Vector3 spawnPos = _firePoint != null ? _firePoint.position : transform.position;

        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null) p.Init(direction);
    }

    // ─── Ammo ────────────────────────────────────────────────────────────────

    private void ConsumeAmmo()
    {
        if (infiniteAmmo) return;
        currentAmmo = Mathf.Max(0, currentAmmo - 1);
        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
    }

    public bool HasAmmo() => infiniteAmmo || currentAmmo > 0;

    // ─── Equip / Unequip ─────────────────────────────────────────────────────

    public virtual void Equip()
    {
        isEquipped = true;
        gameObject.SetActive(true);
        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
    }

    public virtual void Unequip()
    {
        isEquipped = false;
        gameObject.SetActive(false);
    }

    // ─── Public Accessors ────────────────────────────────────────────────────

    private Transform _firePoint;
    public void SetFirePoint(Transform point) => _firePoint = point;
    public string GetWeaponName() => weaponName;
    public Sprite GetWeaponIcon() => weaponIcon;
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
    public bool IsEquipped() => isEquipped;
}