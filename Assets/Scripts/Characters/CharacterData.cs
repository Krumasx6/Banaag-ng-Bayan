using UnityEngine;

// ─────────────────────────────────────────────────────────────────────────────
//  CharacterData.cs
//  ScriptableObject — one asset per playable character.
//  Create via: Right-click > Create > BanaagNgBayan > CharacterData
// ─────────────────────────────────────────────────────────────────────────────

public enum CharacterType  { DatuSulo, TalaLingayan, LakanYano, NaraAlon }
public enum SecondaryType  { Grenade, HealThrow, RageStrike, Drone }
public enum SpecialType    { WarCry, PanataShield, AncestralFury, Overcharge }

[CreateAssetMenu(menuName = "BanaagNgBayan/CharacterData", fileName = "NewCharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Identity")]
    public CharacterType characterType;
    public string        displayName;
    [TextArea(2, 4)]
    public string        lore;               // short flavour text for selection screen
    public Sprite        portrait;           // full-body or bust art
    public Sprite        icon;               // small HUD icon

    [Header("Base Stats")]
    public float moveSpeed  = 5f;
    public int   maxHP      = 3;
    public float dashCooldown = 0.2f;       // seconds; override per character if needed

    [Header("Primary Weapon")]
    public GameObject primaryProjectilePrefab;
    public float      primaryFireRate   = 0.15f;   // seconds between shots
    public float      primaryDamage     = 1f;

    [Header("Secondary Weapon")]
    public SecondaryType secondaryType;
    public int           secondaryMaxUses  = 3;
    public Sprite        secondaryIcon;            // shown in WeaponSlotUI

    [Header("Secondary Prefabs / Settings")]
    // Assign whichever fields apply to this character; leave others null/zero.
    public GameObject secondaryPrefab;            // grenade, heal-potion, drone prefabs
    public float      secondaryLaunchForce = 10f; // Grenade / HealThrow arc force
    public float      rageStrikeDamage     = 3f;  // LakanYano melee lunge damage
    public float      rageStrikeLungeForce = 12f;
    public float      droneDuration        = 8f;  // NaraAlon drone lifetime

    [Header("Special Ability")]
    public SpecialType specialType;
    public float       specialDuration      = 5f;
    public float       specialDamageMultiplier = 1f;   // WarCry = 1.5, AncestralFury = 2.5
    public float       specialFireRateMultiplier = 1f; // WarCry only
    public float       specialSpeedMultiplier = 1f;    // AncestralFury only
    public GameObject  turretPrefab;                   // NaraAlon Overcharge auto-turret

    [Header("SP Bar")]
    public float maxSP           = 100f;
    public float spPerKill        = 20f;
    public float spPerRescue      = 30f;
    public float spPerDamageTaken = 10f;
}