using UnityEngine;

public class StairSpawn : MonoBehaviour
{
    [SerializeField] private ASwitch targetSwitch;

    public GameObject topRow;
    public GameObject bottomRow;
    public GameObject pit;

    private ChangeSpeedPlayer speedPlayer;

    void Start()
    {
        speedPlayer = GetComponent<ChangeSpeedPlayer>();
        speedPlayer.enabled = false;

        topRow.SetActive(false);
        bottomRow.SetActive(false);

        targetSwitch.OnStateChanged += SpawnStairs;
    }

    private void SpawnStairs(bool isOn)
    {
        topRow.SetActive(isOn);
        bottomRow.SetActive(isOn);
        pit.SetActive(!isOn);

        speedPlayer.enabled = isOn;
    }
}
