using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.StateMachines;
using BehaviourAPI.UnityToolkit;
using DG.Tweening;
using MelenitasDev.SoundsGood;
using UnityEngine;

public class EnemyFrogVulnerable : EnemyBase
{
    [SerializeField] private float _rangoDeteccion = 4f;
    [SerializeField] private float _tiempoApuntando = 0.9f;
    [SerializeField] private float _velocidadSalto = 8f;
    [SerializeField] private float _toleranciaEje = 0.3f;
    [SerializeField] private float _distanciaMaximaSalto = 3f;
    [SerializeField, Range(0f, 1f)] private float _porcentajeSiLejos = 0.3f;
    [SerializeField] private float _tiempoAturdida = 3f;
    [SerializeField] private Color _colorAturdida = new Color(0.8f, 1f, 1f);
    [SerializeField] private Sprite[] _framesSalto;
    [SerializeField] private Sprite[] _framesMuerte;
    [SerializeField] private float _duracionFrameMuerte = 0.08f;
    [SerializeField] private GameObject _poof;

    private FSM _fsmRana;
    private FSM _fsmActivo;
    private PushPerception _golpeFrisbee;
    private PushPerception _sinVida;
    private float _tiempoMuriendo;
    private SpriteRenderer _visual;
    private Vector2 _direccionSalto;
    private float _distanciaSalto;
    private float _tiempoSaltando;

    private void Start()
    {
        _visual = GetComponentInChildren<SpriteRenderer>();

        _fsmRana = new FSM();
        _fsmActivo = new FSM();

        // Nivel 1
        State activo = _fsmRana.CreateState("Activo", new SubsystemAction(_fsmActivo));
        State aturdido = _fsmRana.CreateState("Aturdido", new FunctionalAction(AturdidoEntrar, null, AturdidoSalir));
        State morir = _fsmRana.CreateState("Morir", new FunctionalAction(MorirEntrar, MorirActualizar, null));

        // Nivel 2
        State esperando = _fsmActivo.CreateState("Esperando", new FunctionalAction(EsperandoEntrar));
        State apuntar = _fsmActivo.CreateState("Apuntar", new FunctionalAction(ApuntarEntrar));
        State saltar = _fsmActivo.CreateState("Saltar", new FunctionalAction(SaltarEntrar, SaltarActualizar, SaltarSalir));

        // Transiciones nivel 2
        _fsmActivo.CreateTransition("Ve al jugador", esperando, apuntar, new ConditionPerception(JugadorCerca));
        _fsmActivo.CreateTransition("Pierde al jugador", apuntar, esperando, new ConditionPerception(JugadorLejos));
        _fsmActivo.CreateTransition("Termina de apuntar", apuntar, saltar, new UnityTimePerception(_tiempoApuntando));
        _fsmActivo.CreateTransition("Aterriza", saltar, apuntar, statusFlags: StatusFlags.Finished);

        // Transiciones nivel 1
        StateTransition golpeada = _fsmRana.CreateTransition("Golpeada por frisbee", activo, aturdido, statusFlags: StatusFlags.None);
        _golpeFrisbee = new PushPerception(golpeada);
        _fsmRana.CreateTransition("Se recupera", aturdido, activo, new UnityTimePerception(_tiempoAturdida));
        StateTransition muere = _fsmRana.CreateTransition("Sin vida", aturdido, morir, statusFlags: StatusFlags.None);
        StateTransition muereActiva = _fsmRana.CreateTransition("Sin vida activa", activo, morir, statusFlags: StatusFlags.None);
        _sinVida = new PushPerception(muere, muereActiva);

        _fsmRana.Start();
    }

    private void Update()
    {
        _fsmRana.Update();
    }

    public override void Aturdir()
    {
        _golpeFrisbee.Fire();
    }

    public override void Die()
    {
        _sinVida.Fire();
    }

    private bool JugadorCerca()
    {
        return Vector2.Distance(transform.position, PlayerController.Instance.transform.position) <= _rangoDeteccion;
    }

    private bool JugadorLejos()
    {
        return !JugadorCerca();
    }

    private void EsperandoEntrar()
    {
        _rb.linearVelocity = Vector2.zero;
        _visual.sprite = _framesSalto[0];
        Escalar(Vector3.one, 0f);
    }

    private void ApuntarEntrar()
    {
        Vector2 haciaJugador = PlayerController.Instance.transform.position - transform.position;
        float x = Mathf.Abs(haciaJugador.x);
        float y = Mathf.Abs(haciaJugador.y);

        bool saltarEnX = y <= _toleranciaEje || (x > _toleranciaEje && x < y);

        if (saltarEnX)
        {
            _direccionSalto = new Vector2(Mathf.Sign(haciaJugador.x), 0f);
            _distanciaSalto = x;
        }
        else
        {
            _direccionSalto = new Vector2(0f, Mathf.Sign(haciaJugador.y));
            _distanciaSalto = y;
        }

        if (_distanciaSalto > _distanciaMaximaSalto)
        {
            _distanciaSalto *= _porcentajeSiLejos;
        }

        if (_direccionSalto.x != 0f)
        {
            _visual.flipX = _direccionSalto.x > 0f;
        }

        _visual.sprite = _framesSalto[1];
        Escalar(Vector3.one, 0f);
    }

    private void SaltarEntrar()
    {
        _tiempoSaltando = 0f;
        new Sound(SFX.SaltoRana).SetOutput(Output.SFX).SetRandomPitch().Play();
        Escalar(new Vector3(0.8f, 1.25f, 1f), 0.2f);
    }

    private Status SaltarActualizar()
    {
        float duracion = _distanciaSalto / _velocidadSalto;

        _rb.linearVelocity = _direccionSalto * _velocidadSalto;
        _tiempoSaltando += Time.deltaTime;

        int frame = 2 + (int)(_tiempoSaltando / duracion * (_framesSalto.Length - 2));
        _visual.sprite = _framesSalto[Mathf.Min(frame, _framesSalto.Length - 1)];

        if (_tiempoSaltando >= duracion) return Status.Success;
        return Status.Running;
    }

    private void SaltarSalir()
    {
        new Sound(SFX.AterrizajeRana).SetOutput(Output.SFX).SetRandomPitch().Play();
        _rb.linearVelocity = Vector2.zero;
        _visual.sprite = _framesSalto[0];
        Escalar(new Vector3(1.25f, 0.8f, 1f), 0.15f);
        Polvo(transform.position - (Vector3)_direccionSalto * 0.5f + Vector3.down * 0.65f, 0.5f);
    }

    private void AturdidoEntrar()
    {
        Escalar(Vector3.one, 0f);
        _rb.linearVelocity = Vector2.zero;
        _visual.color = _colorAturdida;
    }

    private void AturdidoSalir()
    {
        _visual.color = Color.white;
    }

    private void MorirEntrar()
    {
        _rb.simulated = false;
        _visual.color = Color.white;
        _tiempoMuriendo = 0f;
        new Sound(SFX.MuerteEnemigo).SetOutput(Output.SFX).SetRandomPitch().Play();
    }

    private Status MorirActualizar()
    {
        int frame = (int)(_tiempoMuriendo / _duracionFrameMuerte);

        if (frame >= _framesMuerte.Length)
        {
            Polvo(transform.position, 1f);
            base.Die();
            return Status.Success;
        }

        _visual.sprite = _framesMuerte[frame];
        _tiempoMuriendo += Time.deltaTime;
        return Status.Running;
    }

    private void Escalar(Vector3 desde, float tiempo)
    {
        _visual.transform.DOKill();
        _visual.transform.localScale = desde;
        if (tiempo > 0f) _visual.transform.DOScale(1f, tiempo).SetEase(Ease.OutQuad);
    }

    private void Polvo(Vector3 posicion, float tamaño)
    {
        if (_poof == null) return;
        GameObject polvo = Instantiate(_poof, posicion, Quaternion.identity);
        polvo.transform.localScale = Vector3.one * tamaño;
    }
}
