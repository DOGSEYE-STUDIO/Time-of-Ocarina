using DG.Tweening;
using MelenitasDev.SoundsGood;
using UnityEngine;

public class Chest : APlayerInteractable
{
    [SerializeField] private APickable _itemPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite _openSprite;
    [SerializeField] private GameObject _destello;
    [SerializeField] private GameObject _brillo;

    [SerializeField] private DialogueData _dialogueData;

    private bool _opened;

    public override string ActionText { get { return _opened ? "" : "Abrir"; } }

    public override void OnPlayerInteract()
    {
        if (_opened || PlayerController.Instance.GetFacingDirection() != Vector2.up) return;

        PlayerController.Instance.StopPlayer();
        PlayerController.Instance.gameObject.transform.DOMoveX(gameObject.transform.position.x, 0.1f);
        PlayerController.Instance.PlayAnimation("GetItem", () => {
            new Sound(SFX.Destello).SetOutput(Output.SFX).Play();
            if (_destello != null) Instantiate(_destello, _spawnPoint.position, Quaternion.identity);
            APickable item = Instantiate(_itemPrefab, _spawnPoint.position, Quaternion.identity);
            item.GetComponent<Collider2D>().enabled = false;
            if (_brillo != null) Instantiate(_brillo, item.transform);
            DialogueData dialogo = Instantiate(_dialogueData);
            foreach (DialogueManager.DialogueLine linea in dialogo.lines)
            {
                linea.text = linea.text.Replace("{ITEM}", item.Nombre);
            }

            dialogo.lines[0].onNext += () => GiveItem(item);
            DialogueManager.Instance.StartDialogue(dialogo);
        }, false);

        _opened = true;
        new Sound(SFX.ChestOpen).SetOutput(Output.SFX).Play();
        _renderer.transform.DOPunchScale(new Vector3(0.2f, -0.2f, 0f), 0.3f, 6, 0.5f);
        if (_openSprite != null) _renderer.sprite = _openSprite;
    }

    private void GiveItem(APickable item)
    {
        item.OnPickup();
        Destroy(item.gameObject);
        PlayerController.Instance.SetFacingDirection(Vector2.down);
    }
}
