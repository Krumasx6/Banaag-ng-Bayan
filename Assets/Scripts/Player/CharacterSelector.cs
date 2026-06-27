using UnityEngine;

// ─────────────────────────────────────────────────────────────────────────────
//  CharacterSelector.cs
//  Attach to the Player GameObject in every gameplay scene.
//  Reads the CharacterData chosen on the selection screen and applies stats.
// ─────────────────────────────────────────────────────────────────────────────

public class CharacterSelector : MonoBehaviour
{
    // ── Static bridge set by CharacterSelectUI before scene load ─────────────
    public static CharacterData SelectedData { get; set; }

    // ── Runtime reference ────────────────────────────────────────────────────
    public CharacterData data { get; private set; }

    private WeaponSecondary _secondary;

    void Awake()
    {
        data = SelectedData;

        if (data == null)
        {
            Debug.LogWarning("[CharacterSelector] No CharacterData selected — using defaults.");
            return;
        }

        _secondary = GetComponentInChildren<WeaponSecondary>();

        ApplyPrimaryWeapon();
        ApplySecondaryWeapon();
        ApplySPSettings();
        AttachSecondaryBehaviour();
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Primary weapon — WeaponBase fields are protected so we go through
    //  WeaponSecondary's existing SetWeapon() pattern. For WeaponPrimary we
    //  need its own setter — see note below.
    // ─────────────────────────────────────────────────────────────────────────

    void ApplyPrimaryWeapon()
    {
        WeaponPrimary primary = GetComponentInChildren<WeaponPrimary>();
        if (primary == null || data.primaryProjectilePrefab == null) return;

        // WeaponPrimary inherits SetWeapon from WeaponBase if you add it,
        // or call the existing pattern. For now we rely on the prefab
        // being pre-configured and only override via SetWeapon if available.
        // Add public void SetWeapon(...) to WeaponPrimary the same way
        // WeaponSecondary has it — ask before doing so.
        Debug.Log($"[CharacterSelector] Primary weapon config noted for {data.displayName}. " +
                  "Add SetWeapon() to WeaponPrimary to apply at runtime.");
    }

    void ApplySecondaryWeapon()
    {
        if (_secondary == null) return;
        if (data.secondaryPrefab == null) return;

        // SetWeapon() already exists on WeaponSecondary — safe to call
        _secondary.SetWeapon(
            data.secondaryPrefab,
            data.secondaryMaxUses,
            0.5f,           // fire rate for secondary; add to CharacterData if needed
            1               // damage handled per secondary script, not here
        );
    }

    void ApplySPSettings()
    {
        SPManager sp = GetComponent<SPManager>();
        if (sp == null) return;

        sp.maxSP            = data.maxSP;
        sp.spPerKill        = data.spPerKill;
        sp.spPerRescue      = data.spPerRescue;
        sp.spPerDamageTaken = data.spPerDamageTaken;
    }

    void AttachSecondaryBehaviour()
    {
        // Remove any leftover secondary behaviour from a previous run
        SecondaryBase existing = GetComponent<SecondaryBase>();
        if (existing != null) Destroy(existing);

        switch (data.secondaryType)
        {
            case SecondaryType.Grenade:
                gameObject.AddComponent<SecondaryGrenade>().Init(data);
                break;
            case SecondaryType.HealThrow:
                gameObject.AddComponent<SecondaryHealThrow>().Init(data);
                break;
            case SecondaryType.RageStrike:
                gameObject.AddComponent<SecondaryRageStrike>().Init(data);
                break;
            case SecondaryType.Drone:
                gameObject.AddComponent<SecondaryDrone>().Init(data);
                break;
        }
    }

    public CharacterType GetCharacterType() =>
        data != null ? data.characterType : CharacterType.DatuSulo;
}