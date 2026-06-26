using UnityEngine;

public class HealthTester : MonoBehaviour
{
    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            playerHealth.TakeDamage(1);

        if (Input.GetKeyDown(KeyCode.G))
            playerHealth.Heal(1);
    }
}