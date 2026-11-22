using DG.Tweening;
using MelenitasDev.SoundsGood;
using UnityEngine;

public class Marimba : MonoBehaviour
{
    private enum Note { A, B, C, D, E, F, G }

    [SerializeField] private Transform sprite;
    [SerializeField] private bool isVertical;
    private float originalY;

    [SerializeField] private Note note;

    private Sound marimbaSound;

    private void Start()
    {
        originalY = sprite.position.y;

        switch (note)
        {
            case Note.A:
                marimbaSound = new(SFX.MarimbaA);
                break;
            case Note.B:
                marimbaSound = new(SFX.MarimbaB);
                break;
            case Note.C:
                marimbaSound = new(SFX.MarimbaC);
                break;
            case Note.D:
                marimbaSound = new(SFX.MarimbaD);
                break;
            case Note.E:
                marimbaSound = new(SFX.MarimbaE);
                break;
            case Note.F:
                marimbaSound = new(SFX.MarimbaF);
                break;
            case Note.G:
                marimbaSound = new(SFX.MarimbaG);
                break;
        }

        marimbaSound.SetSpatialSound(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!isVertical) sprite.DOMoveY(originalY - 1f/16f, 0);

        marimbaSound.Play();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!isVertical) sprite.DOMoveY(originalY, 0);
    }
}
