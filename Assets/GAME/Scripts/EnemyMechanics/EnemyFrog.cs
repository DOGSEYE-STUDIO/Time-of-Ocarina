using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.StateMachines;
using BehaviourAPI.UnityToolkit;
using UnityEngine;

public class EnemyFrog : EnemyBase
{
    [SerializeField] private float _rangoDeteccion = 4f;
    [SerializeField] private float _tiempoApuntando = 0.9f;
    [SerializeField] private float _velocidadSalto = 8f;
    [SerializeField] private float _toleranciaEje = 0.3f;
    [SerializeField] private float _distanciaMaximaSalto = 3f;
    [SerializeField, Range(0f, 1f)] private float _porcentajeSiLejos = 0.3f;
    [SerializeField] private float _tiempoAturdida = 3f;

    private FSM _fsmRana;
    private FSM _fsmActivo;
    private PushPerception _golpeFrisbee;
    private bool _aturdida;
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
        State morir = _fsmRana.CreateState("Morir", new FunctionalAction(Die));

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

    public override void OnSwordHit()
    {
        if (!_aturdida) return;

        base.OnSwordHit();
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
        _visual.color = Color.green;
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

        _visual.color = Color.yellow;
    }

    private void SaltarEntrar()
    {
        _tiempoSaltando = 0f;
        _visual.color = new Color(1f, 0.5f, 0f);
    }

    private Status SaltarActualizar()
    {
        _rb.linearVelocity = _direccionSalto * _velocidadSalto;
        _tiempoSaltando += Time.deltaTime;

        if (_tiempoSaltando >= _distanciaSalto / _velocidadSalto) return Status.Success;
        return Status.Running;
    }

    private void SaltarSalir()
    {
        _rb.linearVelocity = Vector2.zero;
    }

    private void AturdidoEntrar()
    {
        _rb.linearVelocity = Vector2.zero;
        _visual.color = Color.cyan;
        _aturdida = true;
    }

    private void AturdidoSalir()
    {
        _aturdida = false;
    }
}
