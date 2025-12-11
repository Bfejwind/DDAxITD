using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Firebase.Extensions; 

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
        if (snapshot.Child("replingName").Exists)
            replingNameText.text = snapshot.Child("replingName").Value.ToString();

        if (snapshot.Child("appearanceIndex").Exists)
        {
            int index = int.Parse(snapshot.Child("appearanceIndex").Value.ToString());
            if (replingSprites != null && index < replingSprites.Length)
                replingImage.sprite = replingSprites[index];
        }

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