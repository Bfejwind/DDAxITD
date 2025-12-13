using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Firebase.Extensions;
using System.Collections.Generic;

public class ReplingManager : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject loginPage;
    public GameObject creationPage;
    public GameObject homePage;

    [Header("Creation Inputs")]
    public TMP_InputField nameInput;
    public UIImageSwitcher imageSwitcher;

    [Header("Home Page UI")]
    public TMP_Text replingNameText;
    public Image replingImage;
    public Sprite[] replingSprites;

    [Header("Stats UI")]
    public TMP_Text speedText;
    public TMP_Text strengthText;
    public TMP_Text enduranceText;

    [Header("Evolution Settings")]
    public int reqStrength = 10;
    public int reqSpeed = 5;
    public int reqEndurance = 5;
    public GameObject evolutionUIPanel;
    public GameObject alreadyEvolvedUI;
    public GameObject evolutionSuccessUI;
    public GameObject evolutionFailUI;

    [Header("Login Inputs")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public GameObject logOutPrompt;

    // Local cached stats
    private int curSpeed;
    private int curStrength;
    private int curEndurance;
    private int curEvoCount;
    private int curAppearanceIndex;

    private FirebaseAuth auth;
    private DatabaseReference dbRef;

    void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Start()
    {
        if (auth.CurrentUser != null)
        {
            auth.SignOut();
        }
    }

    public void BeginLoadingRoutine()
    {
        StartCoroutine(KeepUpdatingStats());
    }

    public void AttemptEvolve()
    {
        if (curEvoCount >= 1)
        {
            if (evolutionUIPanel != null) evolutionUIPanel.SetActive(false);

            if (alreadyEvolvedUI != null) alreadyEvolvedUI.SetActive(true);
            
            return;
        }

        //Debug.Log($"Attempting Evolve... Stats: STR:{curStrength}/{reqStrength} SPD:{curSpeed}/{reqSpeed} END:{curEndurance}/{reqEndurance}");

        if (curStrength >= reqStrength && curSpeed >= reqSpeed && curEndurance >= reqEndurance)
        {
            StartCoroutine(EvolveSuccessRoutine());
        }
        else
        {
            if(evolutionUIPanel != null) evolutionUIPanel.SetActive(false);

            if(evolutionFailUI != null) evolutionFailUI.SetActive(true);
        }
    }

    private IEnumerator EvolveSuccessRoutine()
    {
        if (auth.CurrentUser == null) yield break;

        // Calculate new data 
        int newEvoCount = curEvoCount + 1;
        int newAppearanceIndex = curAppearanceIndex;

        string userId = auth.CurrentUser.UserId;
        DatabaseReference replingRef = dbRef.Child("Users").Child(userId).Child("Repling");

        if (curAppearanceIndex == 0)
        {
            newAppearanceIndex = 2;
        }
        else if (curAppearanceIndex == 1)
        {
            newAppearanceIndex = 3;
        }

        // Prepare updates 
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "evoCount", newEvoCount },
            { "appearanceIndex", newAppearanceIndex }
        };

        // Push to Firebase
        var task = replingRef.UpdateChildrenAsync(updates);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception == null)
        {

            if (evolutionUIPanel != null) evolutionUIPanel.SetActive(false);

            if(evolutionSuccessUI != null) evolutionSuccessUI.SetActive(true);

            StartCoroutine(CheckOrCreateRepling());
        }
        else
        {
            Debug.LogError("Failed to save evolution: " + task.Exception);
        }
    }

    IEnumerator KeepUpdatingStats()
    {
        yield return StartCoroutine(CheckOrCreateRepling());

        while (true)
        {
            // Wait 3 seconds
            yield return new WaitForSeconds(3f);

            if (homePage.activeInHierarchy)
            {
                yield return StartCoroutine(CheckOrCreateRepling());
            }
        }
    }

    IEnumerator CheckOrCreateRepling()
    {
        float timeout = 5f;
        while (auth.CurrentUser == null && timeout > 0)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        if (auth.CurrentUser == null)
        {
            Debug.LogError("Login Timed Out - No User Found");
            yield break;
        }

        string userId = auth.CurrentUser.UserId;

        var task = dbRef.Child("Users").Child(userId).Child("Repling").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError("Database Error: " + task.Exception);
            yield break;
        }

        if (task.Result.Exists)
        {
            // Only force page switches if we aren't already there (prevents flickering)
            if (!homePage.activeSelf)
            {
                loginPage.SetActive(false);
                creationPage.SetActive(false);
                homePage.SetActive(true);
            }

            LoadHomeUI(task.Result);
        }
        else
        {
            if (!creationPage.activeSelf)
            {
                loginPage.SetActive(false);
                creationPage.SetActive(true);
                homePage.SetActive(false);
            }
        }
    }

    public void SaveRepling()
    {
        StartCoroutine(SaveReplingCoroutine());
    }

    private IEnumerator SaveReplingCoroutine()
    {
        if (auth.CurrentUser == null) yield break;

        string replingName = nameInput.text;
        if (string.IsNullOrWhiteSpace(replingName))
        {
            Debug.LogWarning("Name is empty!");
            yield break;
        }

        ReplingData newRepling = new ReplingData
        {
            replingName = replingName,
            speed = 0,
            strength = 0,
            endurance = 0,
            evoCount = 0,
            appearanceIndex = (imageSwitcher != null) ? imageSwitcher.currentIndex : 0
        };

        string json = JsonUtility.ToJson(newRepling);
        string userId = auth.CurrentUser.UserId;

        var saveTask = dbRef.Child("Users").Child(userId).Child("Repling").SetRawJsonValueAsync(json);
        yield return new WaitUntil(() => saveTask.IsCompleted);

        if (saveTask.Exception == null)
        {
            StartCoroutine(CheckOrCreateRepling());
        }
    }

    private void LoadHomeUI(DataSnapshot snapshot)
    {
        // Cache variables
        curAppearanceIndex = snapshot.Child("appearanceIndex").Exists ? int.Parse(snapshot.Child("appearanceIndex").Value.ToString()) : 0;
        curSpeed = snapshot.Child("speed").Exists ? int.Parse(snapshot.Child("speed").Value.ToString()) : 0;
        curStrength = snapshot.Child("strength").Exists ? int.Parse(snapshot.Child("strength").Value.ToString()) : 0;
        curEndurance = snapshot.Child("endurance").Exists ? int.Parse(snapshot.Child("endurance").Value.ToString()) : 0;
        curEvoCount = snapshot.Child("evoCount").Exists ? int.Parse(snapshot.Child("evoCount").Value.ToString()) : 0;

        // UI Updates
        if (snapshot.Child("replingName").Exists)
        {
            replingNameText.text = snapshot.Child("replingName").Value.ToString();
        }

        if (replingSprites != null && curAppearanceIndex < replingSprites.Length)
            replingImage.sprite = replingSprites[curAppearanceIndex];

        speedText.text = curSpeed.ToString();
        strengthText.text = curStrength.ToString();
        enduranceText.text = curEndurance.ToString();
    }

    public void LogOut()
    {
        auth.SignOut();

        if (emailInput != null) emailInput.text = "";
        if (passwordInput != null) passwordInput.text = "";

        homePage.SetActive(false);
        creationPage.SetActive(false);
        logOutPrompt.SetActive(false);
        
        loginPage.SetActive(true);
    }
}

[System.Serializable]
public class ReplingData
{
    public string replingName;
    public int speed;
    public int strength;
    public int endurance;
    public int evoCount;
    public int appearanceIndex;
}