using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoad : MonoBehaviour
{
    public GameObject EnterButton;

    public void LoadExercise()
    {
        SceneManager.LoadSceneAsync("Exercise");
    }

}
