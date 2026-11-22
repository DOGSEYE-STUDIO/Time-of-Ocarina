using MelenitasDev.SoundsGood;
using UnityEngine;

public class RoomClose : MonoBehaviour
{
    [SerializeField] ASwitch targetSwitch;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        targetSwitch.Toggle();
        new Sound(SFX.Portazo).SetOutput(Output.SFX).Play();
        Temblor.Camara(transform.position, 0.2f);

        GetComponent<Collider2D>().enabled = false;
    }

    public void Reabrir()
    {
        Collider2D trigger = GetComponent<Collider2D>();
        if (trigger.enabled || targetSwitch.IsOn) return;

        targetSwitch.Toggle();
        trigger.enabled = true;
    }
}
