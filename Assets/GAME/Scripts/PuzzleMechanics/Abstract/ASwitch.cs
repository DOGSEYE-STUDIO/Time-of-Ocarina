using System;
using UnityEngine;

public abstract class ASwitch : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;

    public bool IsOn;

    public event Action<bool> OnStateChanged;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    public void Toggle()
    {
        IsOn = !IsOn;
        UpdateSprite();
        OnStateChanged?.Invoke(IsOn);
    }

    private void UpdateSprite()
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.sprite = IsOn ? onSprite : offSprite;
    }
}