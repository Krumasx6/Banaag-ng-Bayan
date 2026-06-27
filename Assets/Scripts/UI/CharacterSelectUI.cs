using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// ─────────────────────────────────────────────────────────────────────────────
//  CharacterSelectUI.cs
//  Attach to a CharacterSelectManager GameObject in the Character Selection scene.
//
//  Scene Setup:
//    Canvas
//    └── CharacterSelectManager  [this script]
//        ├── CharacterSlot_0  (Button with CharacterSlotUI child components)
//        ├── CharacterSlot_1
//        ├── CharacterSlot_2
//        ├── CharacterSlot_3
//        ├── ConfirmButton    (Button)
//        ├── DetailName       (TMP_Text)
//        ├── DetailRole       (TMP_Text)
//        ├── DetailLore       (TMP_Text)
//        ├── DetailPlaystyle  (TMP_Text)
//        ├── Ability_J        (TMP_Text)
//        ├── Ability_U        (TMP_Text)
//        └── Ability_L        (TMP_Text)
// ─────────────────────────────────────────────────────────────────────────────

public class CharacterSelectUI : MonoBehaviour
{
    [Header("Character Data Assets (drag from Project)")]
    public CharacterData[] characters;        // assign 4 CharacterData SOs in order

    [Header("Card Buttons (one per character, in order)")]
    public Button[] cardButtons;

    [Header("Card Visual Highlights")]
    public Image[] cardHighlights;            // Image components to tint on selection
    public Color   selectedColor   = new Color(0.79f, 0.66f, 0.30f, 0.6f);
    public Color   unselectedColor = new Color(1f, 1f, 1f, 0.08f);

    [Header("Portrait Images")]
    public Image[] portraits;                 // Assign Image per card

    [Header("Detail Panel")]
    public TMP_Text detailName;
    public TMP_Text detailRole;
    public TMP_Text detailLore;
    public TMP_Text detailPlaystyle;
    public TMP_Text abilityPrimary;
    public TMP_Text abilitySecondary;
    public TMP_Text abilitySpecial;

    [Header("Confirm Button")]
    public Button   confirmButton;
    public TMP_Text confirmLabel;

    [Header("Scene to Load on Confirm")]
    public string gameplaySceneName = "Stage01";

    // ── Runtime ───────────────────────────────────────────────────────────────
    private int _selected = -1;

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        // Wire up card buttons
        for (int i = 0; i < cardButtons.Length; i++)
        {
            int idx = i; // capture for lambda
            cardButtons[i].onClick.AddListener(() => OnCardSelected(idx));
        }

        confirmButton.onClick.AddListener(OnConfirm);
        confirmButton.interactable = false;

        // Default to first character pre-highlighted
        OnCardSelected(0);
    }

    // ─────────────────────────────────────────────────────────────────────────
    void Update()
    {
        // Keyboard navigation
        if (Input.GetKeyDown(KeyCode.RightArrow)) Cycle(1);
        if (Input.GetKeyDown(KeyCode.LeftArrow))  Cycle(-1);
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (confirmButton.interactable) OnConfirm();
        }
    }

    void Cycle(int dir)
    {
        int next = (_selected + dir + characters.Length) % characters.Length;
        OnCardSelected(next);
    }

    // ─────────────────────────────────────────────────────────────────────────
    void OnCardSelected(int index)
    {
        if (index < 0 || index >= characters.Length) return;
        _selected = index;

        // Update card highlights
        for (int i = 0; i < cardHighlights.Length; i++)
        {
            if (cardHighlights[i] != null)
                cardHighlights[i].color = i == index ? selectedColor : unselectedColor;
        }

        // Update portraits
        CharacterData data = characters[index];
        for (int i = 0; i < portraits.Length; i++)
        {
            if (portraits[i] != null && i < characters.Length)
                portraits[i].sprite = characters[i].portrait;
        }

        // Populate detail panel
        if (detailName)       detailName.text    = data.displayName;
        if (detailLore)       detailLore.text    = data.lore;

        // These detail strings can also be stored in CharacterData as extra fields
        // For now we derive short descriptions from SecondaryType / SpecialType
        if (detailRole)       detailRole.text    = GetRoleString(data);
        if (detailPlaystyle)  detailPlaystyle.text = GetPlaystyleString(data);

        if (abilityPrimary)   abilityPrimary.text   = $"[J] Primary — auto fire, {data.primaryFireRate:0.##}s rate";
        if (abilitySecondary) abilitySecondary.text = $"[U] {data.secondaryType} — max {data.secondaryMaxUses} uses";
        if (abilitySpecial)   abilitySpecial.text   = $"[L] {data.specialType} — {data.specialDuration}s duration";

        // Confirm button
        if (confirmButton) confirmButton.interactable = true;
        if (confirmLabel)  confirmLabel.text = $"Summon {data.displayName}";
    }

    // ─────────────────────────────────────────────────────────────────────────
    void OnConfirm()
    {
        if (_selected < 0 || _selected >= characters.Length) return;

        // Store selection on the static bridge
        CharacterSelector.SelectedData = characters[_selected];

        SceneManager.LoadScene(gameplaySceneName);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Helper strings — replace with authored strings in CharacterData if preferred
    // ─────────────────────────────────────────────────────────────────────────

    string GetRoleString(CharacterData d)
    {
        return d.characterType switch
        {
            CharacterType.DatuSulo    => "Warrior · Balanced Offense",
            CharacterType.TalaLingayan => "Healer · Defensive Support",
            CharacterType.LakanYano   => "Berserker · Melee Brawler",
            CharacterType.NaraAlon    => "Engineer · Ranged Control",
            _ => ""
        };
    }

    string GetPlaystyleString(CharacterData d)
    {
        return d.characterType switch
        {
            CharacterType.DatuSulo    => "Aggressive mid-range fighter. High damage upside, straightforward mechanics.",
            CharacterType.TalaLingayan => "Survivalist. Best staying power in the roster — low damage ceiling, near-unkillable.",
            CharacterType.LakanYano   => "Extreme risk/reward. Highest melee damage, low range. Built for the dangerous lane.",
            CharacterType.NaraAlon    => "Passive damage stacker. Drones apply chip while you position. Methodical pressure.",
            _ => ""
        };
    }
}