using UnityEngine;

public class SwordBreakableDrop : SwordBreakable
{
    [System.Serializable]
    private class DropEntry
    {
        public GameObject prefab;
        public float weight = 1f;
    }

    [SerializeField] private DropEntry[] _dropsList;

    public override void OnSwordHit()
    {
        Drop();
        base.OnSwordHit();
    }

    public void Drop()
    {
        float total = 0f;
        foreach (DropEntry entry in _dropsList)
            total += entry.weight;

        float roll = Random.Range(0f, total);

        foreach (DropEntry entry in _dropsList)
        {
            roll -= entry.weight;
            if (roll > 0f) continue;

            if (entry.prefab != null) Instantiate(entry.prefab, transform.position, Quaternion.identity);
            return;
        }
    }
}
