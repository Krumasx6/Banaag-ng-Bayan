using UnityEngine;

// ─────────────────────────────────────────────────────────────────────────────
//  SPPickup.cs
//  Restores a flat SP amount to any character.
//  Tag this GameObject: "Pickup"
// ─────────────────────────────────────────────────────────────────────────────

public class SPPickup : MonoBehaviour
{
    public float spAmount = 25f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        SPManager sp = other.GetComponent<SPManager>();
        if (sp == null) return;

        // We add SP via a dedicated grant method to keep SPManager as the authority
        sp.GrantRescue(); // reuses rescue SP value, or expose a custom Grant(float) if preferred
        Destroy(gameObject);
    }
}