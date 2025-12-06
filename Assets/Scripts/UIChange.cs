using UnityEngine;

public class UIChange : MonoBehaviour
{
    public GameObject SelectUI;
    public GameObject ErrorUser;
    public GameObject ErrorPass;
    public GameObject RegisterUI;
    public GameObject LoginUI;
    void Awake()
    {
        ErrorUser.SetActive(false);
        ErrorPass.SetActive(false);

    }
    // public void ActivateRegister()
    // {
    //     LoginUI.SetActive(false);
    //     RegisterUI.SetActive(true);
    // }
    // public void ActivateLogin()
    // {
    //     RegisterUI.SetActive(false);
    //     LoginUI.SetActive(true);
        
    // }
    
}
