using UnityEngine;

// ─────────────────────────────────────────────────────────────────────────────
//  SecondaryHealThrow.cs  —  Tala Lingayan
//  Throws a heal potion. The potion travels forward and, on collision with the
//  player (or on hitting the ground), heals 1 HP.
//
//  Potion prefab needs:
//    • Rigidbody2D (gravity scale ~0.8)
//    • HealPotionProjectile.cs (handles collision → heal player)
//    • Tag: "PlayerProjectile"
// ─────────────────────────────────────────────────────────────────────────────

public class SecondaryHealThrow : SecondaryBase
{
    private float _launchForce;

    public override void Init(CharacterData data)
    {
        base.Init(data);
        _launchForce = data.secondaryLaunchForce;
    }

    public override void TriggerSecondary()
    {
        if (_data.secondaryPrefab == null)
        {
            Debug.LogWarning("[SecondaryHealThrow] No potion prefab assigned in CharacterData.");
            return;
        }

        // Aim direction comes from AimController if available, otherwise use facing
        Vector2 aimDir = _aim != null
            ? _aim.GetAimDirection()
            : new Vector2(FacingSign(), 0f);

        Vector3 spawnPos = transform.position + new Vector3(FacingSign() * 0.4f, 0.2f, 0f);
        GameObject potion = Instantiate(_data.secondaryPrefab, spawnPos, Quaternion.identity);

        Rigidbody2D rb = potion.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = aimDir.normalized * _launchForce;
        }
    }
}