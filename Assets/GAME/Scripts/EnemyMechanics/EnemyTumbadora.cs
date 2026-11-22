using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.StateMachines;
using BehaviourAPI.UnityToolkit;
using DG.Tweening;
using MelenitasDev.SoundsGood;
using UnityEngine;

public class EnemyTumbadora : EnemyBase
{
    [SerializeField] private float _velocidadMerodeo = 1f;
    [SerializeField] private float _velocidadPersecucion = 2f;
    [SerializeField] private float _velocidadSprint = 5f;
    [SerializeField] private float _tiempoCambioDireccion = 2f;
    [SerializeField] private float _rangoVision = 5f;
    [SerializeField] private float _anguloVision = 90f;
    [SerializeField] private float _distanciaSprint = 2.5f;
    [SerializeField] private float _duracionSprint = 0.8f;
    [SerializeField] private float _esperaEntreSprints = 1.5f;
    [SerializeField] private LayerMask _capaParedes;
    [SerializeField] private float _tiempoAturdida = 2f;
    [SerializeField] private Color _colorAturdida = new Color(0.8f, 1f, 1f);
    [SerializeField] private Sprite[] _framesVuelo;
    [SerializeField] private Sprite[] _framesAtaque;
    [SerializeField] private Sprite[] _framesMuerte;
    [SerializeField] private float _fps = 10f;
    [SerializeField] private float _fpsAtaque = 12f;
    [SerializeField] private GameObject _poof;
    [SerializeField] private Transform _exclamacion;

    private FSM _fsmTumbadora;
    private FSM _fsmActivo;
    private PushPerception _golpeFrisbee;
    private PushPerception _sinVida;
    private bool _aturdida;
    private SpriteRenderer _visual;
    private Vector2 _mirada = Vector2.right;
    private Vector2 _direccionMerodeo;
    private float _tiempoMerodeando;
    private float _finUltimoSprint = -999f;
    private float _tiempoAnimacion;
    private bool _enSprint;
    private bool _vieneDeMerodear;
    private float _siguientePolvo;

    private void Start()
    {
        _visual = GetComponentInChildren<SpriteRenderer>();

        _fsmTumbadora = new FSM();
        _fsmActivo = new FSM();

        // Nivel 1
        State activo = _fsmTumbadora.CreateState("Activo", new SubsystemAction(_fsmActivo));
        State aturdido = _fsmTumbadora.CreateState("Aturdido", new FunctionalAction(AturdidoEntrar, null, AturdidoSalir));
        State morir = _fsmTumbadora.CreateState("Morir", new FunctionalAction(MorirEntrar, MorirActualizar, null));

        // Nivel 2
        State merodear = _fsmActivo.CreateState("Merodear", new FunctionalAction(MerodearEntrar, MerodearActualizar, null));
        State perseguir = _fsmActivo.CreateState("Perseguir", new FunctionalAction(PerseguirEntrar, PerseguirActualizar, null));
        State sprint = _fsmActivo.CreateState("Sprint", new FunctionalAction(SprintEntrar, SprintActualizar, SprintSalir));

        // Transiciones nivel 2
        _fsmActivo.CreateTransition("Ve al jugador", merodear, perseguir, new ConditionPerception(JugadorEnCono));
        _fsmActivo.CreateTransition("Se acerca", perseguir, sprint, new ConditionPerception(JugadorCercaParaSprint));
        _fsmActivo.CreateTransition("Lo pierde", perseguir, merodear, new ConditionPerception(JugadorFueraDeVista));
        _fsmActivo.CreateTransition("Lo pierde en sprint", sprint, merodear, new ConditionPerception(JugadorFueraDeVista));
        _fsmActivo.CreateTransition("Termina el sprint", sprint, perseguir, new UnityTimePerception(_duracionSprint));

        // Transiciones nivel 1
        StateTransition golpeada = _fsmTumbadora.CreateTransition("Golpeada por frisbee", activo, aturdido, statusFlags: StatusFlags.None);
        _golpeFrisbee = new PushPerception(golpeada);
        _fsmTumbadora.CreateTransition("Se recupera", aturdido, activo, new UnityTimePerception(_tiempoAturdida));
        StateTransition muere = _fsmTumbadora.CreateTransition("Sin vida", aturdido, morir, statusFlags: StatusFlags.None);
        _sinVida = new PushPerception(muere);

        _fsmTumbadora.Start();
    }

    private void Update()
    {
        _fsmTumbadora.Update();
    }

    public override void Aturdir()
    {
        _golpeFrisbee.Fire();
    }

    public override void Die()
    {
        _sinVida.Fire();
    }

    public override void OnSwordHit()
    {
        if (!_aturdida) return;

        base.OnSwordHit();
    }

    private bool JugadorEnCono()
    {
        Vector2 haciaJugador = PlayerController.Instance.transform.position - transform.position;

        if (haciaJugador.magnitude > _rangoVision) return false;
        if (Vector2.Angle(_mirada, haciaJugador) > _anguloVision / 2f) return false;
        if (VistaTapada()) return false;

        return true;
    }

    private bool VistaTapada()
    {
        Transform jugador = PlayerController.Instance.transform;
        RaycastHit2D[] golpes = Physics2D.LinecastAll(transform.position, jugador.position, _capaParedes);

        for (int i = 0; i < golpes.Length; i++)
        {
            Transform tocado = golpes[i].transform;

            if (golpes[i].collider.isTrigger) continue;
            if (tocado.IsChildOf(transform) || tocado.IsChildOf(jugador)) continue;

            return true;
        }

        return false;
    }

    private bool JugadorCercaParaSprint()
    {
        if (Time.time < _finUltimoSprint + _esperaEntreSprints) return false;

        return JugadorEnCono() && Vector2.Distance(transform.position, PlayerController.Instance.transform.position) <= _distanciaSprint;
    }

    private bool JugadorFueraDeVista()
    {
        return !JugadorEnCono();
    }

    private void Mover(Vector2 direccion, float velocidad)
    {
        _rb.linearVelocity = direccion * velocidad;
        _mirada = direccion;

        if (direccion.x != 0f)
        {
            _visual.flipX = direccion.x > 0f;
        }

        _tiempoAnimacion += Time.deltaTime;

        if (_enSprint)
        {
            int frame = (int)(_tiempoAnimacion * _fpsAtaque);
            _visual.sprite = _framesAtaque[Mathf.Min(frame, _framesAtaque.Length - 1)];
        }
        else
        {
            int frame = (int)(_tiempoAnimacion * _fps);
            _visual.sprite = _framesVuelo[frame % _framesVuelo.Length];
        }
    }

    private Vector2 DireccionAleatoria()
    {
        return Random.insideUnitCircle.normalized;
    }

    private Vector2 DireccionAlJugador()
    {
        return ((Vector2)(PlayerController.Instance.transform.position - transform.position)).normalized;
    }

    private void MerodearEntrar()
    {
        _vieneDeMerodear = true;
        _tiempoMerodeando = 0f;
        _direccionMerodeo = DireccionAleatoria();
    }

    private Status MerodearActualizar()
    {
        _tiempoMerodeando += Time.deltaTime;

        if (_tiempoMerodeando >= _tiempoCambioDireccion)
        {
            _tiempoMerodeando = 0f;
            _direccionMerodeo = DireccionAleatoria();
        }

        Mover(_direccionMerodeo, _velocidadMerodeo);
        return Status.Running;
    }

    private void PerseguirEntrar()
    {
        if (!_vieneDeMerodear) return;

        _vieneDeMerodear = false;
        new Sound(SFX.Alerta).SetOutput(Output.SFX).Play();

        _exclamacion.DOKill();
        _exclamacion.gameObject.SetActive(true);
        _exclamacion.localScale = Vector3.zero;
        DOTween.Sequence()
            .Append(_exclamacion.DOScale(1f, 0.2f).SetEase(Ease.OutBack))
            .AppendInterval(0.5f)
            .Append(_exclamacion.DOScale(0f, 0.1f))
            .AppendCallback(() => _exclamacion.gameObject.SetActive(false))
            .SetTarget(_exclamacion)
            .SetLink(gameObject);
    }

    private Status PerseguirActualizar()
    {
        Mover(DireccionAlJugador(), _velocidadPersecucion);
        return Status.Running;
    }

    private void SprintEntrar()
    {
        _enSprint = true;
        _tiempoAnimacion = 0f;
    }

    private Status SprintActualizar()
    {
        Mover(DireccionAlJugador(), _velocidadSprint);

        if (Time.time >= _siguientePolvo)
        {
            _siguientePolvo = Time.time + 0.1f;
            Polvo(0.4f);
        }

        return Status.Running;
    }

    private void SprintSalir()
    {
        _enSprint = false;
        _tiempoAnimacion = 0f;
        _finUltimoSprint = Time.time;
    }

    private void AturdidoEntrar()
    {
        _enSprint = false;
        _visual.sprite = _framesVuelo[0];
        _rb.linearVelocity = Vector2.zero;
        _visual.color = _colorAturdida;
        _aturdida = true;
    }

    private void AturdidoSalir()
    {
        _visual.color = Color.white;
        _aturdida = false;
    }

    private void MorirEntrar()
    {
        _rb.simulated = false;
        _visual.color = Color.white;
        new Sound(SFX.MuerteEnemigo).SetOutput(Output.SFX).SetRandomPitch().Play();
        _tiempoAnimacion = 0f;
    }

    private Status MorirActualizar()
    {
        int frame = (int)(_tiempoAnimacion * _fpsAtaque);

        if (frame >= _framesMuerte.Length)
        {
            Polvo(1f);
            base.Die();
            return Status.Success;
        }

        _visual.sprite = _framesMuerte[frame];
        _tiempoAnimacion += Time.deltaTime;
        return Status.Running;
    }

    private void Polvo(float tamaño)
    {
        if (_poof == null) return;
        GameObject polvo = Instantiate(_poof, transform.position, Quaternion.identity);
        polvo.transform.localScale = Vector3.one * tamaño;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _direccionMerodeo = DireccionAleatoria();

        if (!_enSprint || collision.gameObject.CompareTag("Player")) return;

        new Sound(SFX.Choque).SetOutput(Output.SFX).SetRandomPitch().Play();
        _visual.transform.DOKill(true);
        _visual.transform.DOPunchScale(new Vector3(0.3f, -0.3f, 0f), 0.25f, 6, 0.5f);
        Temblor.Camara(transform.position, 0.05f);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 izquierda = Quaternion.Euler(0, 0, _anguloVision / 2f) * _mirada;
        Vector3 derecha = Quaternion.Euler(0, 0, -_anguloVision / 2f) * _mirada;

        Gizmos.DrawWireSphere(transform.position, _rangoVision);
        Gizmos.DrawLine(transform.position, transform.position + izquierda * _rangoVision);
        Gizmos.DrawLine(transform.position, transform.position + derecha * _rangoVision);
    }
}
