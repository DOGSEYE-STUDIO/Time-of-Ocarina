using DG.Tweening;
using System;
using UnityEngine;

public class Door : APlayerInteractable
{
    private BoxCollider2D boxCollider;

    public bool isLocked;
    public bool isBossDoor;

    [SerializeField] private GameObject poof;
    [SerializeField] private Transform sprite;

    private float spriteStart;

    [SerializeField] private ASwitch targetSwitch;

    public event Action<bool> onDoorChange;

    public override string ActionText { get { return isLocked ? "Abrir" : ""; } }

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        if (!isLocked) targetSwitch.OnStateChanged += UpdateDoor;

        spriteStart = sprite.localPosition.y;
    }

    private void UpdateDoor(bool isOn)
    {
        boxCollider.enabled = !isOn;
        onDoorChange?.Invoke(isOn);

        if (isOn) sprite.DOLocalMoveY(spriteStart + 1, 0.3f);
        else sprite.DOLocalMoveY(spriteStart, 0.3f);
    }

    public override void OnPlayerInteract()
    {
        if (isLocked && PlayerInventory.Instance.HasKey())
        {
            Instantiate(poof, sprite.position, sprite.rotation);
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.1f);
            seq.AppendCallback(() =>
            {
                UpdateDoor(true);
                PlayerInventory.Instance.RemoveKey();
            });
            return;
        }

        if (isBossDoor && isLocked && PlayerInventory.Instance.HasBossKey)
        {
            UpdateDoor(true);
            return;
        }
    }
}
