using UnityEngine;

public class HealthAnimatorUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Animator healthAnimator;

    private static readonly int HealthParam = Animator.StringToHash("Health");

    private void Awake()
    {
        if (playerHealth == null)
            playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (healthAnimator == null)
            healthAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (playerHealth == null) { enabled = false; return; }
        playerHealth.OnHealthChanged += HandleHealthChanged;
        SetHealthAnimation(playerHealth.GetCurrentHP());
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int currentHP, int maxHP)
    {
        SetHealthAnimation(currentHP);
    }

    private void SetHealthAnimation(int currentHP)
    {
        if (healthAnimator == null) return;
        int clampedHP = Mathf.Clamp(currentHP, 0, 3);
        healthAnimator.SetInteger(HealthParam, clampedHP);
    }
}