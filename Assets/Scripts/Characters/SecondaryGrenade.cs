using UnityEngine;

// ─────────────────────────────────────────────────────────────────────────────
//  SecondaryGrenade.cs  —  Datu Sulo
//  Throws a physics-based arc grenade toward the aim direction.
//  The grenade prefab needs:
//    • Rigidbody2D (gravity scale ~1)
//    • A GrenadeProjectile.cs component (handles detonation/AOE damage)
//    • Tag: "PlayerProjectile"
// ─────────────────────────────────────────────────────────────────────────────

public class SecondaryGrenade : SecondaryBase
{
    [Header("Runtime — set via Init()")]
    [SerializeField] private float _launchForce;

    public override void Init(CharacterData data)
    {
        base.Init(data);
        _launchForce = data.secondaryLaunchForce;
    }

    public override void TriggerSecondary()
    {
        if (_data.secondaryPrefab == null)
        {
            Debug.LogWarning("[SecondaryGrenade] No grenade prefab assigned in CharacterData.");
            return;
        }

        // Spawn at player position with a small forward offset
        Vector3 spawnPos = transform.position + new Vector3(FacingSign() * 0.5f, 0.3f, 0f);
        GameObject grenade = Instantiate(_data.secondaryPrefab, spawnPos, Quaternion.identity);

        // Launch in an arc: horizontal = facing direction, vertical = upward bias
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 launchDir = new Vector2(FacingSign(), 0.6f).normalized;
            rb.linearVelocity = launchDir * _launchForce;
        }
    }
}