using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance { get; private set; }
    public bool LoadedExercise = false;
    public string statChoice;
    public string exerciseChoice;
    public GameObject loginPage;
    public TMP_Text tutorialTitle;
    public TMP_Text tutorialStat;

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
        //loginPage.SetActive(true);// Set the initial UI state to only show the login page.
    }

    public void LoadExercise()
    {
        // Load a new scene
        SceneManager.LoadSceneAsync("Exercise");
        LoadedExercise = true;
    }
    public void ChoseSTR()
    {
        statChoice = "Strength";
        tutorialTitle.text = statChoice;
        ChangeTutorialText(statChoice);
    }
    public void ChoseSPD()
    {
        statChoice = "Speed";
        tutorialTitle.text = statChoice;
        ChangeTutorialText(statChoice);
    }
    public void ChoseEND()
    {
        statChoice = "Endurance";
        tutorialTitle.text = statChoice;
        ChangeTutorialText(statChoice);
    }
    public void ChangeTutorialText(string stat)
    {
        if (stat == "Strength")
        {
            tutorialStat.text = "Each Rep must take at <b><u>Least</u></b> 3 seconds before completion";
        }
        if (stat == "Speed")
        {
            tutorialStat.text = "Each Rep must take at <b><u>Most</u></b> 2 seconds before completion";
        }
        if (stat == "Endurance")
        {
            tutorialStat.text = "You must complete at <b><u>Least</u></b> 12 Reps before completion";
        }
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
