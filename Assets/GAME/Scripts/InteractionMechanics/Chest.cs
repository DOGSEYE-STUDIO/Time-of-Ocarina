using DG.Tweening;
using MelenitasDev.SoundsGood;
using UnityEngine;

public class Chest : APlayerInteractable
{
    [SerializeField] private APickable _itemPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite _openSprite;

    [Header("Animación del objeto")]
    [SerializeField] private float _riseHeight = 1f;
    [SerializeField] private float _riseTime = 1.5f;
    [SerializeField] private float _holdTime = 1.5f;

    private bool _opened;

    public override void OnPlayerInteract()
    {
        if (_opened) return;

        _opened = true;

        if (_openSprite != null) _renderer.sprite = _openSprite;

        APickable item = Instantiate(_itemPrefab, _spawnPoint.position, Quaternion.identity);
        
        item.GetComponent<Collider2D>().enabled = false;


        DOTween.Sequence()
            .Append(item.transform.DOMoveY(_spawnPoint.position.y + _riseHeight, _riseTime).SetEase(Ease.OutQuad))
            .AppendInterval(_holdTime)
            .AppendCallback(() => GiveItem(item))
            .SetLink(item.gameObject);
    }

    private void GiveItem(APickable item)
    {
        item.OnPickup();
        Destroy(item.gameObject);

        new Sound(SFX.coin).SetOutput(Output.SFX).Play();
    }
}
