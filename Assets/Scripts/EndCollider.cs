using UnityEngine;

public class EndCollider : MonoBehaviour
{
    public StartCollider RepStartSwitch;
    void Update()
    {
        if (RepStartSwitch == null)
        {
            GameObject StartPrefab = GameObject.FindGameObjectWithTag("StartPoint");
            RepStartSwitch = StartPrefab.GetComponent<StartCollider>();
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (RepStartSwitch != null)
        {
            RepStartSwitch.repStart = true;
        }
    }
}
