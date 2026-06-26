using UnityEngine;

public class WeaponPrimary : WeaponBase
{
    protected override void Awake()
    {
        base.Awake();
        infiniteAmmo = true;
    }
}