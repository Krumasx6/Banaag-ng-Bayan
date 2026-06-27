using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSlotUI : MonoBehaviour
{
    public enum SlotType { Primary, Secondary }

    [Header("Slot Type")]
    [SerializeField] private SlotType slotType = SlotType.Primary;

    [Header("UI References")]
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TMP_Text activeLabel;
    [SerializeField] private TMP_Text ammoText;

    [Header("Player References")]
    [SerializeField] private WeaponPrimary weaponPrimary;
    [SerializeField] private WeaponSecondary weaponSecondary;

    private void Start()
    {
        if (slotType == SlotType.Primary)
        {
            if (weaponPrimary == null) return;
            weaponPrimary.OnAmmoChanged += UpdateAmmo;
            // Primary always visible — show immediately
            ShowPrimary();
        }
        else
        {
            if (weaponSecondary == null) return;
            weaponSecondary.OnAmmoChanged += UpdateAmmo;
            // Secondary hidden until pickup
            SetEmpty();
        }
    }

    private void OnDestroy()
    {
        if (weaponPrimary != null)   weaponPrimary.OnAmmoChanged   -= UpdateAmmo;
        if (weaponSecondary != null) weaponSecondary.OnAmmoChanged -= UpdateAmmo;
    }

    // ─── Update ──────────────────────────────────────────────────────────────

    private void UpdateAmmo(int current, int max)
    {
        if (slotType == SlotType.Secondary && current <= 0 && max <= 0)
        {
            SetEmpty();
            return;
        }

        // Show slot
        if (weaponIcon != null)
        {
            weaponIcon.gameObject.SetActive(true);
            weaponIcon.sprite = slotType == SlotType.Primary
                ? weaponPrimary?.GetWeaponIcon()
                : weaponSecondary?.GetWeaponIcon();
        }

        if (activeLabel != null)
            activeLabel.gameObject.SetActive(true);

        if (ammoText != null)
        {
            // Primary = infinite so just show ∞
            ammoText.text = slotType == SlotType.Primary
                ? "∞"
                : $"{current} / {max}";
        }
    }

    // ─── Primary always shown on Start ───────────────────────────────────────

    private void ShowPrimary()
    {
        if (weaponIcon != null)
        {
            weaponIcon.gameObject.SetActive(true);
            weaponIcon.sprite = weaponPrimary?.GetWeaponIcon();
        }

        if (activeLabel != null) activeLabel.gameObject.SetActive(true);
        if (ammoText != null)    ammoText.text = "∞";
    }

    // ─── Secondary hidden when empty ─────────────────────────────────────────

    private void SetEmpty()
    {
        if (weaponIcon != null)  weaponIcon.gameObject.SetActive(false);
        if (activeLabel != null) activeLabel.gameObject.SetActive(false);
        if (ammoText != null)    ammoText.text = "";
    }
}