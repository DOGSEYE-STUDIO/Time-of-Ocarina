using DG.Tweening;
using MelenitasDev.SoundsGood;
using UnityEngine;

public class SwitchBlock : MonoBehaviour
{
    public enum BlockColor
    {
        Red,
        Blue
    }

    [SerializeField] private BlockColor blockColor;
    [SerializeField] private ASwitch targetSwitch;

    private Collider2D blockCollider;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator animatorTop;

    private void Start()
    {
        blockCollider = GetComponent<Collider2D>();

        UpdateBlock(targetSwitch.IsOn);
        targetSwitch.OnStateChanged += UpdateBlock;
        _listo = true;
    }

    private static int _frameSonido;
    private bool _listo;
    private bool _esperandoJugador;

    private void Update()
    {
        if (_esperandoJugador && !JugadorEncima())
        {
            _esperandoJugador = false;
            Aplicar(true);
        }
    }

    private void UpdateBlock(bool isOn)
    {
        bool shouldBeActive = blockColor == BlockColor.Red ? !isOn : isOn;

        _esperandoJugador = shouldBeActive && JugadorEncima();
        if (_esperandoJugador) return;

        Aplicar(shouldBeActive);
    }

    private bool JugadorEncima()
    {
        Bounds casilla = new Bounds(transform.position, new Vector3(0.95f, 0.95f, 1f));
        return casilla.Intersects(PlayerController.Instance.GetComponent<Collider2D>().bounds);
    }

    private void Aplicar(bool shouldBeActive)
    {
        if (_listo)
        {
            transform.DOKill(true);
            transform.DOPunchScale(new Vector3(0.15f, -0.15f, 0f), 0.25f, 6, 0.5f);

            if (_frameSonido != Time.frameCount)
            {
                _frameSonido = Time.frameCount;
                new Sound(SFX.Bloque).SetOutput(Output.SFX).Play();
            }
        }

        if (animator != null)
        {
            animator.SetBool("cerrado", shouldBeActive);
            animatorTop.SetBool("cerrado", shouldBeActive);
            animatorTop.GetComponent<SpriteRenderer>().enabled = shouldBeActive;
        }

        if (blockCollider != null)
        {
            blockCollider.enabled = shouldBeActive;
        }
    }
}
