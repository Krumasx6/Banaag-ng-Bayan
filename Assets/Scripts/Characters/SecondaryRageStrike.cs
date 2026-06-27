using UnityEngine;
using System.Collections;

// ─────────────────────────────────────────────────────────────────────────────
//  SecondaryRageStrike.cs  —  Lakan Yano
//  Melee lunge: player surges forward, dealing boosted damage to any enemy hit
//  within the lunge hitbox. Uses a temporary OverlapBox during the lunge frame.
// ─────────────────────────────────────────────────────────────────────────────

public class SecondaryRageStrike : SecondaryBase
{
    private float _damage;
    private float _lungeForce;
    private bool  _isLunging;

    // Hitbox dimensions (tune in Inspector or CharacterData if needed)
    private readonly Vector2 _hitboxSize  = new Vector2(1.4f, 1.0f);
    private readonly float   _hitboxOffsetX = 0.9f;

    public override void Init(CharacterData data)
    {
        base.Init(data);
        _damage     = data.rageStrikeDamage;
        _lungeForce = data.rageStrikeLungeForce;
    }

    public override void TriggerSecondary()
    {
        if (_isLunging) return;
        StartCoroutine(LungeRoutine());
    }

    private IEnumerator LungeRoutine()
    {
        _isLunging = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Zero out vertical momentum so the lunge is horizontal
            rb.linearVelocity = new Vector2(FacingSign() * _lungeForce, rb.linearVelocity.y);
        }

        // Wait one physics frame, then check for hits
        yield return new WaitForFixedUpdate();

        Vector2 hitboxCenter = (Vector2)transform.position + new Vector2(FacingSign() * _hitboxOffsetX, 0f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxCenter, _hitboxSize, 0f);

        foreach (Collider2D col in hits)
        {
            if (col.gameObject == gameObject) continue;
            IDamageable target = col.GetComponent<IDamageable>();
            target?.TakeDamage((int)_damage);
        }

        // Brief cooldown before allowing another lunge
        yield return new WaitForSeconds(0.4f);
        _isLunging = false;
    }

    // Draw hitbox in Scene view for easy tuning
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.4f);
        Vector2 center = (Vector2)transform.position + new Vector2(FacingSign() * _hitboxOffsetX, 0f);
        Gizmos.DrawCube(center, _hitboxSize);
    }
}