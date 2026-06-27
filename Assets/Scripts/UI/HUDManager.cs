using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Health Sprites")]
    [SerializeField] private Image hpImage;
    [SerializeField] private Sprite[] hpSprites;  // 0=3HP, 1=2HP, 2=1HP, 3=0HP

    [Header("Special Points Sprites")]
    [SerializeField] private Image spImage;
    [SerializeField] private Sprite[] spSprites;  // 0=0SP, 1=1SP, 2=2SP, 3=3SP (or however many levels)

    [Header("Player Reference")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerSpecial playerSpecial; // hook up later when built

    private void Start()
    {
        if (playerHealth == null)
        {
            Debug.LogWarning("HUDManager: No PlayerHealth assigned.");
            return;
        }

        playerHealth.OnHealthChanged += UpdateHP;
        playerHealth.OnDeath         += OnPlayerDeath;
        playerHealth.OnRespawn       -= OnPlayerRespawn;

        // Initialize
        UpdateHP(playerHealth.GetCurrentHP(), playerHealth.GetMaxHP());
        
        SPManager sp = playerHealth.GetComponent<SPManager>();

        if (sp != null)
        {
            sp.OnSPChanged.AddListener((cur, max) => UpdateSPFromFloat(cur, max));
            sp.OnSPFull.AddListener(FlashSPBarFull);
        }
    }

    private void OnDestroy()
    {
        if (playerHealth == null) return;
        playerHealth.OnHealthChanged -= UpdateHP;
        playerHealth.OnDeath         -= OnPlayerDeath;
        playerHealth.OnRespawn       += OnPlayerRespawn;
    }

    // ─── HP Sprite Swap ──────────────────────────────────────────────────────

    private void UpdateHP(int current, int max)
    {
        if (hpImage == null || hpSprites == null || hpSprites.Length == 0) return;

        // Index = max - current (3HP=index0, 2HP=index1, 1HP=index2, 0HP=index3)
        int index = Mathf.Clamp(max - current, 0, hpSprites.Length - 1);
        hpImage.sprite = hpSprites[index];
    }

    // ─── SP Sprite Swap (called by PlayerSpecial later) ──────────────────────

    public void UpdateSP(int current, int max)
    {
        if (spImage == null || spSprites == null || spSprites.Length == 0) return;

        int index = Mathf.Clamp(current, 0, spSprites.Length - 1);
        spImage.sprite = spSprites[index];
    }

    // ─── Death / Respawn ─────────────────────────────────────────────────────

    private void OnPlayerDeath()
    {
        Debug.Log("Player died.");
    }

    private void OnPlayerRespawn()
    {
        Debug.Log("Player respawned.");
    }

    private void UpdateSPFromFloat(float cur, float max)
    {
        // Convert float SP to a sprite index matching your spSprites array
        int index = Mathf.FloorToInt((cur / max) * (spSprites.Length - 1));
        index = Mathf.Clamp(index, 0, spSprites.Length - 1);
        if (spImage != null && spSprites != null && spSprites.Length > 0)
            spImage.sprite = spSprites[index];
    }

    private void FlashSPBarFull()
    {
        // Optional — add a visual flash here when SP is ready
        Debug.Log("SP Full — ready to use special!");
    }
}