using UnityEngine;

public class WeaponSecondary : WeaponBase
{
    [Header("Secondary Settings")]
    [SerializeField] private bool destroyOnEmpty = true;

    protected override void Awake()
    {
        base.Awake();
        infiniteAmmo = false;
    }

    protected override void Fire(Vector2 direction)
    {
        base.Fire(direction);

        if (destroyOnEmpty && !HasAmmo())
            ClearWeapon();
    }

    public void SetWeapon(GameObject prefab, int ammo, float rate, int dmg)
    {
        projectilePrefab = prefab;
        maxAmmo          = ammo;
        currentAmmo      = ammo;
        fireRate         = rate;
        damage           = dmg;

        OnAmmoChanged?.Invoke(ammo, ammo);
    }

    public void ClearWeapon()
    {
        Unequip();
        OnAmmoChanged?.Invoke(0, 0);
    }
}