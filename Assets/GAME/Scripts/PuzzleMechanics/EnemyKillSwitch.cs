using UnityEngine;

public class EnemyKillSwitch : ASwitch
{
    [SerializeField] private EnemyBase[] enemies;

    private int deadEnemies;

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
            Toggle();
        }
    }
}