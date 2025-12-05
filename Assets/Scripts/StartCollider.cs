using TMPro;
using UnityEngine;

public class StartCollider : MonoBehaviour
{
    public bool repStart;
    [SerializeField]
    public int repsDone;
    void Start()
    {
        repStart = false;
        repsDone = 0;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (repStart)
        {
            repsDone++;
            TMP_Text changeRepsDone = GameObject.Find("repNumber").GetComponent<TMP_Text>();
            changeRepsDone.text = "Reps: " + repsDone;
            repStart = false;
        }
    }
}
