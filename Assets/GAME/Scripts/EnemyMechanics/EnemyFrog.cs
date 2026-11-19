using BehaviourAPI.Core.Actions;
using BehaviourAPI.StateMachines;
using UnityEngine;

public class EnemyFrog : EnemyBase
{
    private FSM _frogFsm;
    private FSM _activeFsm;

    private void Start()
    {
        // Nivel 2: lo que hace la rana mientras está Activa
        _activeFsm = new FSM();
        State waiting = _activeFsm.CreateState("Esperando", new FunctionalAction(() => Debug.Log("Rana: Esperando")));
        State aiming = _activeFsm.CreateState("Apuntar", new FunctionalAction(() => Debug.Log("Rana: Apuntar")));
        State jumping = _activeFsm.CreateState("Saltar", new FunctionalAction(() => Debug.Log("Rana: Saltar")));

        // Nivel 1: la rana entera
        _frogFsm = new FSM();
        State active = _frogFsm.CreateState("Activo", new SubsystemAction(_activeFsm));
        State stunned = _frogFsm.CreateState("Aturdido", new FunctionalAction(() => Debug.Log("Rana: Aturdido")));
        State dying = _frogFsm.CreateState("Morir", new FunctionalAction(Die));

        _frogFsm.Start();
    }

    private void Update()
    {
        _frogFsm.Update();
    }
}
