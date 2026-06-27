using UnityEngine;
using UnityEngine.Events;

// ─────────────────────────────────────────────────────────────────────────────
//  SPManager.cs
//  Attach alongside PlayerHealth / PlayerCombat on the Player GameObject.
//  Tracks the SP bar.  Other systems (enemies, rescue triggers) call the
//  public Grant*() methods.  HUDManager should subscribe to OnSPChanged.
//
//  How to wire up SP gain:
//    EnemyBase.OnDeath event  →  player SPManager.GrantKill()
//    RescueTrigger            →  player SPManager.GrantRescue()
//    PlayerHealth.OnDamaged   →  player SPManager.GrantDamageTaken()
// ─────────────────────────────────────────────────────────────────────────────

public class SPManager : MonoBehaviour
{
    // ── Config (overwritten by CharacterSelector) ─────────────────────────────
    [HideInInspector] public float maxSP            = 100f;
    [HideInInspector] public float spPerKill        = 20f;
    [HideInInspector] public float spPerRescue      = 30f;
    [HideInInspector] public float spPerDamageTaken = 10f;

    // ── Runtime ───────────────────────────────────────────────────────────────
    public float CurrentSP  { get; private set; }
    public bool  IsFull     => CurrentSP >= maxSP;

    // ── Events ────────────────────────────────────────────────────────────────
    // HUDManager (or any listener) subscribes:
    //   spManager.OnSPChanged.AddListener((cur, max) => UpdateBar(cur, max));
    public UnityEvent<float, float> OnSPChanged = new UnityEvent<float, float>();
    public UnityEvent               OnSPFull    = new UnityEvent();

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        CurrentSP = 0f;
        NotifyListeners();
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Public Grant methods — called by external systems
    // ─────────────────────────────────────────────────────────────────────────

    public void GrantKill()        => AddSP(spPerKill);
    public void GrantRescue()      => AddSP(spPerRescue);
    public void GrantDamageTaken() => AddSP(spPerDamageTaken);

    // ─────────────────────────────────────────────────────────────────────────
    //  Called by PlayerSpecial after consuming the bar
    // ─────────────────────────────────────────────────────────────────────────
    public void ConsumeAll()
    {
        CurrentSP = 0f;
        NotifyListeners();
    }

    // ─────────────────────────────────────────────────────────────────────────
    private void AddSP(float amount)
    {
        bool wasNotFull = !IsFull;
        CurrentSP = Mathf.Min(CurrentSP + amount, maxSP);
        NotifyListeners();
        if (wasNotFull && IsFull) OnSPFull.Invoke();
    }

    private void NotifyListeners() => OnSPChanged.Invoke(CurrentSP, maxSP);
}