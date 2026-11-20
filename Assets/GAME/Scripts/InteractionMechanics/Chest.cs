using DG.Tweening;
using MelenitasDev.SoundsGood;
using UnityEngine;

public class Chest : APlayerInteractable
{
    [SerializeField] private APickable _itemPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite _openSprite;

    [SerializeField] private DialogueData _dialogueData;

    private bool _opened;

    public override string ActionText { get { return _opened ? "" : "Abrir"; } }

    public override void OnPlayerInteract()
    {
        if (_opened || PlayerController.Instance.GetFacingDirection() != Vector2.up) return;

        PlayerController.Instance.StopPlayer();
        PlayerController.Instance.gameObject.transform.DOMoveX(gameObject.transform.position.x, 0.1f);
        PlayerController.Instance.PlayAnimation("GetItem", () => {
            //new Sound(SFX.ChestOpen).SetOutput(Output.SFX).Play();
            APickable item = Instantiate(_itemPrefab, _spawnPoint.position, Quaternion.identity);
            item.GetComponent<Collider2D>().enabled = false;
            _dialogueData.lines[0].onNext += () => GiveItem(item);
            DialogueManager.Instance.StartDialogue(_dialogueData);
        }, false);

        _opened = true;
        if (_openSprite != null) _renderer.sprite = _openSprite;
    }

    private void GiveItem(APickable item)
    {
        item.OnPickup();
        Destroy(item.gameObject);
        PlayerController.Instance.SetFacingDirection(Vector2.down);
    }
}
