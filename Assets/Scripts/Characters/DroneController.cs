using UnityEngine;
using System.Collections;

// ─────────────────────────────────────────────────────────────────────────────
//  DroneController.cs
//  Attach to the Drone prefab.
//  Drone bullet damage is set in the bullet prefab Inspector — not via code.
// ─────────────────────────────────────────────────────────────────────────────

public class DroneController : MonoBehaviour
{
    [Header("Orbit")]
    public float orbitRadius = 1.5f;
    public float orbitSpeed  = 90f;

    [Header("Combat")]
    public GameObject droneBulletPrefab;
    public float      fireRate       = 0.4f;
    public float      detectionRange = 8f;
    public float      bulletSpeed    = 12f;
    // damage is set on the bullet prefab's Projectile component in the Inspector

    private Transform _owner;
    private float     _angle;
    private float     _fireTimer;

    public void Init(Transform owner, float lifetime)
    {
        _owner = owner;
        StartCoroutine(LifetimeCountdown(lifetime));
    }

    void Update()
    {
        if (_owner == null) { Destroy(gameObject); return; }
        Orbit();

        _fireTimer += Time.deltaTime;
        if (_fireTimer >= 1f / fireRate)
        {
            _fireTimer = 0f;
            TryFire();
        }
    }

    void Orbit()
    {
        _angle += orbitSpeed * Time.deltaTime;
        float rad = _angle * Mathf.Deg2Rad;
        transform.position = (Vector2)_owner.position
            + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;
    }

    void TryFire()
    {
        GameObject target = FindNearestEnemy();
        if (target == null || droneBulletPrefab == null) return;

        Vector2 dir = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
        GameObject bullet = Instantiate(droneBulletPrefab, transform.position, Quaternion.identity);
        bullet.tag = "PlayerProjectile";

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = dir * bulletSpeed;

        // Damage value comes from the prefab's Projectile component — set it there
        Projectile p = bullet.GetComponent<Projectile>();
        if (p != null) p.Init(dir);
    }

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearest = null;
        float closest = detectionRange;

        foreach (GameObject e in enemies)
        {
            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist < closest) { closest = dist; nearest = e; }
        }
        return nearest;
    }

    IEnumerator LifetimeCountdown(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}