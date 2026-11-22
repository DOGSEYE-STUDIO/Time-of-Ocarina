using UnityEngine;

public class EnemyKillSwitch : ASwitch
{
    [SerializeField] private EnemyBase[] enemies;

    private int deadEnemies;

    public bool Completado { get { return deadEnemies >= enemies.Length; } }

    private void OnEnable()
    {
        EnemyBase.OnEnemyDeath += CheckEnemyDeath;
    }

    private void OnDisable()
    {
        EnemyBase.OnEnemyDeath -= CheckEnemyDeath;
    }

    private void Start()
    {
        Toggle();
    }

    private void CheckEnemyDeath(EnemyBase enemy)
    {
        if (System.Array.IndexOf(enemies, enemy) == -1)
            return;

        deadEnemies++;

        if (deadEnemies >= enemies.Length)
        {
            if (IsOn) UpdateState();
            else Toggle();
        }
    }

    public bool Controla(EnemyBase enemigo)
    {
        foreach (EnemyBase enemy in enemies)
        {
            if (ReferenceEquals(enemy, enemigo)) return true;
        }

        return false;
    }

    public void Sustituir(EnemyBase viejo, EnemyBase nuevo)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (ReferenceEquals(enemies[i], viejo)) enemies[i] = nuevo;
        }

        deadEnemies--;
    }
}