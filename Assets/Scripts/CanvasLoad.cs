using UnityEngine;

public class CanvasLoad : MonoBehaviour
{
    public GameObject CanvasExercises;
    private void Start()
    {
        if (GameManagerScript.Instance.LoadedExercise)
        {
            gameObject.SetActive(false);
            CanvasExercises.SetActive(true);
            GameManagerScript.Instance.LoadedExercise = false;
        }
    }
}
