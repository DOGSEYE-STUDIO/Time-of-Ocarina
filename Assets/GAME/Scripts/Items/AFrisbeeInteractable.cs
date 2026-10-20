using UnityEngine;

public abstract class AFrisbeeInteractable : MonoBehaviour
{
    public bool ReturnOnHit = true;

    public abstract void OnFrisbeeHit();
}
