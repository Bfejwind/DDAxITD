using UnityEngine;

public class UIChange : MonoBehaviour
{
    public GameObject ErrorUser;
    public GameObject ErrorPass;
    void Awake()
    {
        ErrorUser.SetActive(false);
        ErrorPass.SetActive(false);

    }
}
