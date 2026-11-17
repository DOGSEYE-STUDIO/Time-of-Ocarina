using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] _frames;
    [SerializeField] private float _fps = 12f;

    private SpriteRenderer _renderer;
    private float _time;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _time += Time.deltaTime;
        _renderer.sprite = _frames[(int)(_time * _fps) % _frames.Length];
    }
}
