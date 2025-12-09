using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ReplingManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject loginPage;
    public GameObject creationPage;
    public TMP_InputField nameInput;
    public UIImageSwitcher imageSwitcher;
    public GameObject homePage;
    public TMP_Text replingNameText;
    public Image replingImage;
    public Sprite[] replingSprites;

    [Header("Stats UI - Assign the NUMBER text objects here")]
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

    public void OnLoginSuccess()
    {
        StartCoroutine(CheckOrCreateRepling());
    }

    IEnumerator CheckOrCreateRepling()
    {
        while (auth.CurrentUser == null)
        {
            yield return null;
        }

        string userId = auth.CurrentUser.UserId;

        var task = dbRef.Child("Users").Child(userId).Child("Repling").GetValueAsync();
        
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError("Failed to check repling: " + task.Exception);
            yield break;
        }

        if (task.Result.Exists)
        {
            loginPage.SetActive(false);
            creationPage.SetActive(false); 
            homePage.SetActive(true);     
            
            LoadHomeUI(task.Result);
        }
        else
        {
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
        while (auth.CurrentUser == null)
            yield return null;

        string replingName = nameInput.text;
        if (string.IsNullOrWhiteSpace(replingName)) 
        {
            Debug.LogWarning("Repling name cannot be empty.");
            yield break;
        }

        ReplingData newRepling = new ReplingData
        {
            replingName = replingName,
            speed = 0,
            strength = 0,
            endurance = 0,
            evoCount = 0,
            appearanceIndex = imageSwitcher.currentIndex
        };

        string json = JsonUtility.ToJson(newRepling);
        string userId = auth.CurrentUser.UserId;

        var saveTask = dbRef.Child("Users").Child(userId).Child("Repling").SetRawJsonValueAsync(json);
        yield return new WaitUntil(() => saveTask.IsCompleted);

        if (saveTask.Exception != null)
        {
            Debug.LogError("Failed to save repling: " + saveTask.Exception);
            yield break;
        }
        
        StartCoroutine(CheckOrCreateRepling());
    }

    private void LoadHomeUI(DataSnapshot snapshot)
    {
        string name = snapshot.Child("replingName").Value.ToString();
        
        if (int.TryParse(snapshot.Child("appearanceIndex").Value.ToString(), out int appearanceIndex))
        {
            replingNameText.text = name;
            
            if (appearanceIndex >= 0 && appearanceIndex < replingSprites.Length)
                replingImage.sprite = replingSprites[appearanceIndex];
            else
                Debug.LogWarning("Appearance index out of range: " + appearanceIndex);
        }
        else
        {
            Debug.LogError("Failed to parse appearanceIndex from Firebase.");
        }

        var speedVal = snapshot.Child("speed").Value;
        var strengthVal = snapshot.Child("strength").Value;
        var enduranceVal = snapshot.Child("endurance").Value;

        speedText.text = speedVal != null ? speedVal.ToString() : "0";
        strengthText.text = strengthVal != null ? strengthVal.ToString() : "0";
        enduranceText.text = enduranceVal != null ? enduranceVal.ToString() : "0";
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