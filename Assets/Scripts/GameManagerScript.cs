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
    public TMP_Text exerciseReminder;
    public int repsDone;
    public TMP_Text modifyReps;
    [SerializeField]
    int statIncrease = 1;

    private void Awake()
    {
        repsDone = 0;
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
        ChangeReminderText(statChoice);
    }
    public void ChoseSPD()
    {
        statChoice = "Speed";
        tutorialTitle.text = statChoice;
        ChangeTutorialText(statChoice);
        ChangeReminderText(statChoice);
    }
    public void ChoseEND()
    {
        statChoice = "Endurance";
        tutorialTitle.text = statChoice;
        ChangeTutorialText(statChoice);
        ChangeReminderText(statChoice);
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
    public void ChangeReminderText(string stat)
    {
        if (stat == "Strength")
        {
            exerciseReminder.text = "Strength\n>3 seconds per rep";
        }
        if (stat == "Speed")
        {
            exerciseReminder.text = "Speed\n<2 secondsper rep";
        }
        if (stat == "Endurance")
        {
            exerciseReminder.text = "Endurance\n>12 Reps";
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
    public void RepSuccess()
    {
        repsDone++;
        modifyReps.text = "Reps: " + repsDone;
        SetComplete();
    }
    public void SetComplete()
    {
        if (statChoice == "Strength")
        {
            if (repsDone>= 8)
            {
                tutorialStat.text = "Congratulations!\nYou've Earned" + statIncrease + " " + statChoice + "!";
            }
            else
            {
                tutorialStat.text = "You completed " + repsDone + "/" + 8 + "reps!\nCome Back and Fight Me!";
            }
        }
        else if (statChoice == "Speed")
        {
            if (repsDone>= 12)
            {
                tutorialStat.text = "Congratulations!\nYou've Earned" + statIncrease + " " + statChoice + "!";
            }
            else
            {
                tutorialStat.text = "You completed " + repsDone + "/" + 12 + "reps!\nCome Back and Fight Me!";
            }
        }
        else if (statChoice == "Endurance")
        {
            if (repsDone>= 12)
            {
                tutorialStat.text = "Congratulations!\nYou've Earned" + statIncrease + " " + statChoice + "!";
            }
            else
            {
                tutorialStat.text = "You completed " + repsDone + "/" + 12 + "reps!\nCome Back and Fight Me!";
            }
        }
        else
        {
            Debug.Log("No stat selected");
        }
        repsDone = 0;
    }
}
