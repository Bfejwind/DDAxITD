using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Firebase.Extensions; // Make sure this is here

public class ReplingManager : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject loginPage;
    public GameObject creationPage;
    public GameObject homePage;

    [Header("Creation Inputs")]
    public TMP_InputField nameInput;
    public UIImageSwitcher imageSwitcher; // Ensure you have this script

    [Header("Home Page UI")]
    public TMP_Text replingNameText;
    public Image replingImage;
    public Sprite[] replingSprites;

    [Header("Stats UI")]
    public TMP_Text speedText;
    public TMP_Text strengthText;
    public TMP_Text enduranceText;

    private FirebaseAuth auth;
    private DatabaseReference dbRef;

    void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Start()
    {
        // Force Logout on start to prevent "Ghost Login" bugs while testing
        if (auth.CurrentUser != null)
        {
            auth.SignOut();
        }
    }

    // Called by Authentication.cs ONLY after password is verified
    public void BeginLoadingRoutine()
    {
        StartCoroutine(CheckOrCreateRepling());
    }

    IEnumerator CheckOrCreateRepling()
    {
        // 1. Wait for Firebase to finish the login handshake
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
        Debug.Log("Checking database for User: " + userId);

        // 2. Check Database
        var task = dbRef.Child("Users").Child(userId).Child("Repling").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError("Database Error: " + task.Exception);
            yield break;
        }

        // 3. Decide which page to show
        if (task.Result.Exists)
        {
            Debug.Log("Repling Found! Going to Home.");
            LoadHomeUI(task.Result); // Load data FIRST
            
            loginPage.SetActive(false);
            creationPage.SetActive(false);
            homePage.SetActive(true);
        }
        else
        {
            Debug.Log("No Repling Found. Going to Creation.");
            loginPage.SetActive(false);
            creationPage.SetActive(true);
            homePage.SetActive(false);
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
            appearanceIndex = (imageSwitcher != null) ? imageSwitcher.currentIndex : 0 // Safety check
        };

        string json = JsonUtility.ToJson(newRepling);
        string userId = auth.CurrentUser.UserId;

        var saveTask = dbRef.Child("Users").Child(userId).Child("Repling").SetRawJsonValueAsync(json);
        yield return new WaitUntil(() => saveTask.IsCompleted);

        if (saveTask.Exception == null)
        {
            // After save, reload the home page check
            StartCoroutine(CheckOrCreateRepling());
        }
    }

    private void LoadHomeUI(DataSnapshot snapshot)
    {
        // 1. Load Name
        if (snapshot.Child("replingName").Exists)
            replingNameText.text = snapshot.Child("replingName").Value.ToString();

        // 2. Load Appearance
        if (snapshot.Child("appearanceIndex").Exists)
        {
            int index = int.Parse(snapshot.Child("appearanceIndex").Value.ToString());
            if (replingSprites != null && index < replingSprites.Length)
                replingImage.sprite = replingSprites[index];
        }

        // 3. Load Stats (Safe parsing)
        speedText.text = snapshot.Child("speed").Exists ? snapshot.Child("speed").Value.ToString() : "0";
        strengthText.text = snapshot.Child("strength").Exists ? snapshot.Child("strength").Value.ToString() : "0";
        enduranceText.text = snapshot.Child("endurance").Exists ? snapshot.Child("endurance").Value.ToString() : "0";
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