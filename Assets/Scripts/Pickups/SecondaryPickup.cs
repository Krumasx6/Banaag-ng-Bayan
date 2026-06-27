using UnityEngine;

// ─────────────────────────────────────────────────────────────────────────────
//  SecondaryPickup.cs
//  Tag this GameObject: "Pickup"
//  Set targetCharacter in the Inspector to match which character gets this drop.
// ─────────────────────────────────────────────────────────────────────────────

public class SecondaryPickup : MonoBehaviour
{
    [Header("Which character can pick this up")]
    public CharacterType targetCharacter;

    [Header("How many uses to restore")]
    public int usesGranted = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        CharacterSelector selector = other.GetComponent<CharacterSelector>();
        if (selector == null) return;
        if (selector.GetCharacterType() != targetCharacter) return;

        WeaponSecondary secondary = other.GetComponentInChildren<WeaponSecondary>();
        if (secondary == null) return;

        // AddAmmo() is inherited from WeaponBase — already public
        secondary.AddAmmo(usesGranted);
        Destroy(gameObject);
    }
}