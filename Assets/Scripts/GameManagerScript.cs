using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;

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
    public GameObject canvasBackground;
    public GameObject exerciseCanvas;
    public GameObject repStatusReport;
    public int ReplingIndex = 0;
    [SerializeField]
    AudioSource PunchingSFX;
    [SerializeField]
    AudioSource WinningSFX;
    [SerializeField]
    AudioSource SlashingSFX;
    [SerializeField]
    AudioSource Natural1SFX;
    [SerializeField]
    AudioSource RepUpSFX;
    [SerializeField]
    AudioSource SetCompleteSFX;
    [SerializeField]
    AudioSource EvolutionSFX;

    private FirebaseAuth auth;
    private DatabaseReference dbRef;

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

            auth = FirebaseAuth.DefaultInstance;
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;
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
            tutorialStat.text = "Each Rep must take at <b><u>Least</u></b> 3 seconds before completion\n\nComplete <b><u>8</u></b> Reps for\n+1 Strength";
        }
        if (stat == "Speed")
        {
            tutorialStat.text = "Each Rep must take at <b><u>Most</u></b> 2 seconds before completion\n\nComplete <b><u>12</u></b> Reps for\n+1 Speed";
        }
        if (stat == "Endurance")
        {
            tutorialStat.text = "You must complete at <b><u>Least</u></b> 12 Reps for\n+1 Endurance";
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
        RepUpSFX.Play();
        modifyReps.text = "Reps: " + repsDone;
        SetComplete();
    }
    public void ModifyReps()
    {
        modifyReps.text = "Reps: " + repsDone;
    }
    public void StopExercise()
    {
        tutorialStat.text = "You completed " + repsDone + " reps!\nI need MORE!";
        repsDone = 0;
        modifyReps.text = "Reps: " + repsDone;
    }
    public void SetComplete()
    {
        if (statChoice == "Strength")
        {
            if (repsDone>= 8)
            {
                CompleteCongrats();
                SetCompleteSFX.Play();
            }
            return;
        }
        else if (statChoice == "Speed")
        {
            if (repsDone>= 12)
            {
                CompleteCongrats();
                SetCompleteSFX.Play();
            }
            return;
        }
        else if (statChoice == "Endurance")
        {
            if (repsDone>= 12)
            {
                CompleteCongrats();
                SetCompleteSFX.Play();
            }
            return;
        }
        else
        {
            Debug.Log("No stat selected");
        }
    }
    public void CompleteCongrats()
    {
        tutorialStat.text = "Congratulations!\nYou've Earned " + statIncrease + " " + statChoice + "!";
        canvasBackground.SetActive(true);
        exerciseCanvas.SetActive(true);
        repStatusReport.SetActive(true);
        repsDone = 0;
        modifyReps.text = "Reps: " + repsDone;

        StartCoroutine(AddStatRoutine());
    }

    IEnumerator AddStatRoutine()
    {
        if (auth.CurrentUser == null) yield break;

        string userId = auth.CurrentUser.UserId;
        string dbKey = statChoice.ToLower(); // Lower the stat choice to match database keys

        var getTask = dbRef.Child("Users").Child(userId).Child("Repling").Child(dbKey).GetValueAsync();
        yield return new WaitUntil(() => getTask.IsCompleted);

        if (getTask.Exception != null) 
        {
            yield break;
        }

        long currentVal = 0;
        if (getTask.Result.Exists)
        {
            // Convert the data from Firebase into a number
            currentVal = long.Parse(getTask.Result.Value.ToString());
        }
        
        long newVal = currentVal + statIncrease;

        // Set the new value back to Firebase
        var setTask = dbRef.Child("Users").Child(userId).Child("Repling").Child(dbKey).SetValueAsync(newVal);
        yield return new WaitUntil(() => setTask.IsCompleted);
    
    }
    public void Punch()
    {
        PunchingSFX.Play();
    }
    public void Winning()
    {
        WinningSFX.Play();
    }
    public void Slash()
    {
        SlashingSFX.Play();
    }
    public void Dying()
    {
        Natural1SFX.Play();
    }
    public void Evolve()
    {
        EvolutionSFX.Play();
    }
}
