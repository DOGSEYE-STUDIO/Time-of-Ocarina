using System;
using DG.Tweening;
using MelenitasDev.SoundsGood;
using UnityEngine;

public abstract class ASwitch : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;

    public bool IsOn;
    public bool CanToggle { get; protected set; } = true;

    public event Action<bool> OnStateChanged;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    public void Toggle()
    {
        if (!CanToggle)
            return;

        CanToggle = false;

        IsOn = !IsOn;
        UpdateSprite();
        UpdateState();
    }

    public void Sacudir()
    {
        new Sound(SFX.Interruptor).SetOutput(Output.SFX).Play();
        transform.DOKill(true);
        transform.DOPunchScale(new Vector3(0.2f, -0.2f, 0f), 0.2f, 6, 0.5f);
        Temblor.Camara(transform.position, 0.03f);
    }

    protected void FinishToggle()
    {
        CanToggle = true;
    }

    protected void UpdateSprite()
    {
        if (spriteRenderer == null)
        {
            FinishToggle();
            return;
        }

        spriteRenderer.sprite = IsOn ? onSprite : offSprite;

        FinishToggle();
    }

    protected void UpdateState()
    {
        OnStateChanged?.Invoke(IsOn);
    }
}