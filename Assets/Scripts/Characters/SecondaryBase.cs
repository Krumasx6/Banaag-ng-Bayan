using UnityEngine;

public abstract class SecondaryBase : MonoBehaviour
{
    protected CharacterData _data;
    protected AimController _aim;

    public virtual void Init(CharacterData data)
    {
        _data = data;
        _aim  = GetComponent<AimController>();
    }

    public abstract void TriggerSecondary();

    // Uses the existing public IsFacingRight() — no private field access
    protected int FacingSign()
    {
        PlayerMovement m = GetComponent<PlayerMovement>();
        return (m != null && m.IsFacingRight()) ? 1 : -1;
    }
}