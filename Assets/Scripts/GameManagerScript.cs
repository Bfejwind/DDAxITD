using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance { get; private set; }
    public bool LoadedExercise = false;
    public string statChoice;
    public string exerciseChoice;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
    }

    public void LoadExercise()
    {
        Debug.Log("Loading Exercise");
        // Load a new scene
        SceneManager.LoadSceneAsync("Exercise");
        LoadedExercise = true;
    }
    public void LoadMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
        ResetExerciseChoice();
    }
    public void ChoseSTR()
    {
        statChoice = "Strength";
    }
    public void ChoseSPD()
    {
        statChoice = "Speed";
    }
    public void ChoseEND()
    {
        statChoice = "Endurance";
    }
    public void ResetStatChoice()
    {
        statChoice = null;
    }
    public void ChoseBicepCurl()
    {
        exerciseChoice = "BicepCurl";
    }
    public void ChoseSquat()
    {
        exerciseChoice = "Squat";
    }
    public void ChoseShoulder()
    {
        exerciseChoice = "ShoulderPress";
    }
    public void ResetExerciseChoice()
    {
        exerciseChoice = null;
    } 
}
