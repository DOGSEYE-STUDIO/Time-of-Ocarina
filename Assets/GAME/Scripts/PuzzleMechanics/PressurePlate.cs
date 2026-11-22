using MelenitasDev.SoundsGood;
using UnityEngine;

public class PressurePlate : ASwitch
{
    private void OnTriggerStay2D(Collider2D other)
    {
        if (IsOn) return;

        PushableObject pushable = other.gameObject.GetComponent<PushableObject>();

        if (other.gameObject.CompareTag("Player") || pushable != null)
        {
            IsOn = true;
            UpdateState();
            UpdateSprite();
            new Sound(SFX.PlacaPisar).SetOutput(Output.SFX).Play();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PushableObject pushable = other.gameObject.GetComponent<PushableObject>();

        if (other.gameObject.CompareTag("Player") || pushable != null)
        {
            if (IsOn) new Sound(SFX.PlacaSoltar).SetOutput(Output.SFX).Play();
            IsOn = false;
            UpdateState();
            UpdateSprite();
        }
    }
}
