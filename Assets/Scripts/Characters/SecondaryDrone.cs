using UnityEngine;

// ─────────────────────────────────────────────────────────────────────────────
//  SecondaryDrone.cs  —  Nara Alon
//  Spawns an auto-targeting drone that floats near the player and shoots the
//  nearest enemy.
//
//  Drone prefab needs:
//    • DroneController.cs component (orbits player, fires at enemies)
//    • Its own lifetime is controlled here via _droneDuration
// ─────────────────────────────────────────────────────────────────────────────

public class SecondaryDrone : SecondaryBase
{
    private float     _droneDuration;
    private GameObject _activeDrone;

    public override void Init(CharacterData data)
    {
        base.Init(data);
        _droneDuration = data.droneDuration;
    }

    public override void TriggerSecondary()
    {
        if (_data.secondaryPrefab == null)
        {
            Debug.LogWarning("[SecondaryDrone] No drone prefab assigned in CharacterData.");
            return;
        }

        // Despawn existing drone if still active
        if (_activeDrone != null) Destroy(_activeDrone);

        _activeDrone = Instantiate(
            _data.secondaryPrefab,
            transform.position + new Vector3(0f, 1.2f, 0f),
            Quaternion.identity
        );

        // Pass owner transform so the drone can orbit the player
        DroneController dc = _activeDrone.GetComponent<DroneController>();
        if (dc != null) dc.Init(transform, _droneDuration);

        // Auto-destroy after duration in case DroneController doesn't handle it
        Destroy(_activeDrone, _droneDuration);
    }

    private void OnDestroy()
    {
        if (_activeDrone != null) Destroy(_activeDrone);
    }
}