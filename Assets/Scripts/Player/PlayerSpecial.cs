using UnityEngine;
using System.Collections;

// ─────────────────────────────────────────────────────────────────────────────
//  PlayerSpecial.cs
//  Attach to the Player GameObject.
//  Triggers the correct special ability when L is pressed and SP is full.
// ─────────────────────────────────────────────────────────────────────────────

[RequireComponent(typeof(SPManager))]
[RequireComponent(typeof(CharacterSelector))]
public class PlayerSpecial : MonoBehaviour
{
    private CharacterData     _data;
    private SPManager         _sp;
    private CharacterSelector _selector;
    private WeaponPrimary     _primary;
    private PlayerMovement    _movement;
    private PlayerHealth      _health;

    private bool _specialActive;

    // Cached base values before special modifies them
    private float _baseMoveSpeed;

    void Awake()
    {
        _sp       = GetComponent<SPManager>();
        _selector = GetComponent<CharacterSelector>();
        _primary  = GetComponentInChildren<WeaponPrimary>();
        _movement = GetComponent<PlayerMovement>();
        _health   = GetComponent<PlayerHealth>();
    }

    void Start()
    {
        _data = _selector.data;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && _sp.IsFull && !_specialActive)
            ActivateSpecial();
    }

    void ActivateSpecial()
    {
        if (_data == null) return;
        _sp.ConsumeAll();

        switch (_data.specialType)
        {
            case SpecialType.WarCry:        StartCoroutine(WarCryRoutine());        break;
            case SpecialType.PanataShield:  StartCoroutine(PanataShieldRoutine());  break;
            case SpecialType.AncestralFury: StartCoroutine(AncestralFuryRoutine()); break;
            case SpecialType.Overcharge:    StartCoroutine(OverchargeRoutine());    break;
        }
    }

    // ── War Cry — Datu Sulo ──────────────────────────────────────────────────
    // WeaponBase.damage and fireRate are protected — WeaponPrimary can access
    // them internally. We trigger via a public method on WeaponPrimary.
    // Add public void ApplyDamageMultiplier(float m) to WeaponPrimary — ask first.
    IEnumerator WarCryRoutine()
    {
        _specialActive = true;
        Debug.Log("[WarCry] Active — hook WeaponPrimary.ApplyDamageMultiplier() here");
        // Placeholder until WeaponPrimary exposes a multiplier method
        yield return new WaitForSeconds(_data.specialDuration);
        Debug.Log("[WarCry] Ended");
        _specialActive = false;
    }

    // ── Panata Shield — Tala Lingayan ────────────────────────────────────────
    // Uses ForceInvincible() — the one line you add to PlayerHealth
    IEnumerator PanataShieldRoutine()
    {
        _specialActive = true;
        _health?.ForceInvincible(_data.specialDuration);
        Debug.Log("[PanataShield] Active");
        yield return new WaitForSeconds(_data.specialDuration);
        Debug.Log("[PanataShield] Ended");
        _specialActive = false;
    }

    // ── Ancestral Fury — Lakan Yano ──────────────────────────────────────────
    // moveSpeed is private in PlayerMovement — needs a public setter or we skip it.
    // Same as WarCry, damage boost needs WeaponPrimary method.
    // For now: logs a placeholder, speed boost is skipped until PM exposes setter.
    IEnumerator AncestralFuryRoutine()
    {
        _specialActive = true;
        Debug.Log("[AncestralFury] Active — hook moveSpeed setter and damage multiplier here");
        yield return new WaitForSeconds(_data.specialDuration);
        Debug.Log("[AncestralFury] Ended");
        _specialActive = false;
    }

    // ── Overcharge — Nara Alon ───────────────────────────────────────────────
    IEnumerator OverchargeRoutine()
    {
        _specialActive = true;

        if (_data.turretPrefab != null)
        {
            GameObject turret = Instantiate(_data.turretPrefab, transform.position, Quaternion.identity);
            Destroy(turret, _data.specialDuration);
            Debug.Log("[Overcharge] Turret deployed");
        }
        else
        {
            Debug.LogWarning("[Overcharge] No turret prefab assigned in CharacterData.");
        }

        yield return new WaitForSeconds(_data.specialDuration);
        _specialActive = false;
        Debug.Log("[Overcharge] Ended");
    }
}