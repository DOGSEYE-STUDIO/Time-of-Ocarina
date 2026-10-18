using UnityEngine;

public abstract class SpecialItem : MonoBehaviour
{
    public abstract void Use(Vector2 facingDirection);

    public virtual void SetButtonHeld(bool held) { }
}
