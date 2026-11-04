using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [System.Serializable]
    private class DropEntry
    {
        public GameObject prefab;
        public float weight = 1f;
    }

    [SerializeField] private DropEntry[] _dropsList;

    public void Drop()
    {
        float total = 0f;
        for (int i = 0; i < _dropsList.Length; i++)
            total += _dropsList[i].weight;

        float roll = Random.Range(0f, total);

        for (int i = 0; i < _dropsList.Length; i++)
        {
            roll -= _dropsList[i].weight;
            if (roll > 0f) continue;

            if (_dropsList[i].prefab != null)
            {
                GameObject item = Instantiate(_dropsList[i].prefab, transform.position, Quaternion.identity);
                item.GetComponent<DropFall>().Play();
            }
            return;
        }
    }
}
