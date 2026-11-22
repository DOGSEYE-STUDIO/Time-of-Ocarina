using DG.Tweening;
using MelenitasDev.SoundsGood;
using System;
using UnityEngine;

public class Door : APlayerInteractable
{
    private BoxCollider2D boxCollider;

    public bool isLocked;
    public bool isBossDoor;

    [SerializeField] private GameObject poof;
    [SerializeField] private Transform sprite;
    [SerializeField] private GameObject _destello;

    private float spriteStart;

    [SerializeField] private ASwitch targetSwitch;
    [SerializeField] private bool invertir;

    public event Action<bool> onDoorChange;

    public override string ActionText { get { return isLocked ? "Abrir" : ""; } }

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        spriteStart = sprite.localPosition.y;

        if (!isLocked)
        {
            targetSwitch.OnStateChanged += UpdateDoor;
            UpdateDoor(targetSwitch.IsOn);
        }
    }

    private void UpdateDoor(bool isOn)
    {
        if (invertir) isOn = !isOn;

        boxCollider.enabled = !isOn;
        onDoorChange?.Invoke(isOn);

        if (isOn && Time.timeSinceLevelLoad > 0.5f) new Sound(SFX.PuertaAbrir).SetOutput(Output.SFX).Play();

        if (isOn) sprite.DOLocalMoveY(spriteStart + 1, 0.3f);
        else sprite.DOLocalMoveY(spriteStart, 0.3f);
    }

    public override void OnPlayerInteract()
    {
        if (isLocked && !isBossDoor && PlayerInventory.Instance.HasKey())
        {
            new Sound(SFX.Cerradura).SetOutput(Output.SFX).Play();
            Instantiate(poof, sprite.position, sprite.rotation);
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.1f);
            seq.AppendCallback(() =>
            {
                Destellar();
                UpdateDoor(true);
                PlayerInventory.Instance.RemoveKey();
            });
            return;
        }

        if (isBossDoor && isLocked && PlayerInventory.Instance.HasBossKey)
        {
            new Sound(SFX.Cerradura).SetOutput(Output.SFX).Play();
            Destellar();
            UpdateDoor(true);
            if (VictoryMenu.Instance != null) DOVirtual.DelayedCall(1f, VictoryMenu.Instance.Show).SetLink(gameObject);
            return;
        }
    }

    private void Destellar()
    {
        new Sound(SFX.Destello).SetOutput(Output.SFX).Play();
        if (_destello != null) Instantiate(_destello, sprite.position, Quaternion.identity);
    }
}
