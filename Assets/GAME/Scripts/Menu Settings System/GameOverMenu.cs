using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private string _menuScene = "MAIN MENU";
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private float _tiempoCamaraLenta = 0.6f;
    [SerializeField] private float _duracionFundido = 0.5f;
    [SerializeField, Range(0f, 1f)] private float _opacidadFundido = 0.9f;
    [SerializeField] private Vector2Int _resolucionVineta = new Vector2Int(128, 72);

    // Orden en que se van rellenando los píxeles de un bloque 4x4 (tramado Bayer)
    private static readonly int[] Bayer = { 0, 8, 2, 10, 12, 4, 14, 6, 3, 11, 1, 9, 15, 7, 13, 5 };

    private Sprite[] _framesFundido;
    private Sprite[] _framesVineta;
    private Image _vineta;
    private Image _fondo;
    private RectTransform _contenido;
    private Transform _titulo;

    public static GameOverMenu Instance { get; private set; }
    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        Instance = this;

        _panel.SetActive(false);
        _fondo = _panel.GetComponent<Image>();
        _contenido = (RectTransform)_panel.transform.GetChild(0);
        _titulo = _contenido.GetChild(0);

        _framesFundido = new Sprite[16];
        for (int i = 0; i < _framesFundido.Length; i++)
        {
            _framesFundido[i] = CrearFrameFundido(i + 1);
        }

        _framesVineta = new Sprite[8];
        for (int i = 0; i < _framesVineta.Length; i++)
        {
            _framesVineta[i] = CrearFrameVineta(Mathf.Lerp(1.4f, 0.65f, i / 7f));
        }

        _vineta = new GameObject("Vineta", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        _vineta.transform.SetParent(_panel.transform.parent, false);
        _vineta.transform.SetSiblingIndex(_panel.transform.GetSiblingIndex());
        RectTransform rectVineta = _vineta.rectTransform;
        rectVineta.anchorMin = Vector2.zero;
        rectVineta.anchorMax = Vector2.one;
        rectVineta.offsetMin = Vector2.zero;
        rectVineta.offsetMax = Vector2.zero;
        _vineta.color = Color.black;
        _vineta.raycastTarget = false;
        _vineta.gameObject.SetActive(false);

        _retryButton.onClick.AddListener(Retry);
        _menuButton.onClick.AddListener(GoToMenu);
    }

    private void Update()
    {
        if (IsGameOver && _panel.activeSelf && EventSystem.current.currentSelectedGameObject == null) EventSystem.current.SetSelectedGameObject(_retryButton.gameObject);
    }

    public void Show()
    {
        IsGameOver = true;
        PlayerController.Instance.StopPlayer();
        Time.timeScale = 0.3f;

        _vineta.gameObject.SetActive(true);
        DOVirtual.Int(0, _framesVineta.Length - 1, _tiempoCamaraLenta, frame => _vineta.sprite = _framesVineta[frame]).SetUpdate(true).SetLink(gameObject);

        DOVirtual.DelayedCall(_tiempoCamaraLenta, MostrarPanel).SetUpdate(true).SetLink(gameObject);
    }

    private void MostrarPanel()
    {
        Time.timeScale = 0f;
        _panel.SetActive(true);

        _fondo.color = new Color(0f, 0f, 0f, _opacidadFundido);
        _fondo.type = Image.Type.Tiled;
        _fondo.pixelsPerUnitMultiplier = 0.125f;
        DOVirtual.Int(0, _framesFundido.Length - 1, _duracionFundido, frame => _fondo.sprite = _framesFundido[frame]).SetUpdate(true).SetLink(gameObject);

        _contenido.anchoredPosition = new Vector2(0f, 900f);
        _contenido.DOAnchorPosY(0f, 0.8f).SetEase(Ease.OutBounce).SetDelay(_duracionFundido * 0.5f).SetUpdate(true).SetLink(gameObject);

        _titulo.localRotation = Quaternion.Euler(0f, 0f, -3f);
        _titulo.DOLocalRotate(new Vector3(0f, 0f, 3f), 0.8f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetUpdate(true).SetLink(gameObject);

        EventSystem.current.SetSelectedGameObject(_retryButton.gameObject);
    }

    private Sprite CrearFrameFundido(int pixelesRellenos)
    {
        Texture2D textura = new Texture2D(4, 4);
        textura.filterMode = FilterMode.Point;
        textura.wrapMode = TextureWrapMode.Repeat;

        for (int i = 0; i < 16; i++)
        {
            textura.SetPixel(i % 4, i / 4, Bayer[i] < pixelesRellenos ? Color.white : Color.clear);
        }

        textura.Apply();
        return Sprite.Create(textura, new Rect(0, 0, 4, 4), Vector2.zero, 100f, 0, SpriteMeshType.FullRect);
    }

    private Sprite CrearFrameVineta(float radio)
    {
        int ancho = _resolucionVineta.x;
        int alto = _resolucionVineta.y;
        Texture2D textura = new Texture2D(ancho, alto);
        textura.filterMode = FilterMode.Point;
        textura.wrapMode = TextureWrapMode.Clamp;

        for (int x = 0; x < ancho; x++)
        {
            for (int y = 0; y < alto; y++)
            {
                Vector2 desdeCentro = new Vector2((x + 0.5f) / ancho - 0.5f, (y + 0.5f) / alto - 0.5f) * 2f;
                float oscuridad = Mathf.Clamp01((desdeCentro.magnitude - radio) / 0.5f);
                bool negro = Bayer[x % 4 + y % 4 * 4] < oscuridad * 16f;
                textura.SetPixel(x, y, negro ? Color.white : Color.clear);
            }
        }

        textura.Apply();
        return Sprite.Create(textura, new Rect(0, 0, ancho, alto), Vector2.zero, 100f, 0, SpriteMeshType.FullRect);
    }

    private void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_menuScene);
    }
}
