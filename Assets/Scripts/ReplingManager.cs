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

    private FirebaseAuth auth;
    private DatabaseReference dbRef;

    void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Start()
    {
        // 🛑 CRITICAL CHANGE: Force sign out to destroy any persistent session.
        if (auth.CurrentUser != null)
        {
            auth.SignOut();
        }
        // Do NOT call CheckOrCreateRepling here; it will be called after a successful login.
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
        
        LoadHomeUIFromLocal(newRepling);
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
    }

    private void LoadHomeUIFromLocal(ReplingData data)
    {
        creationPage.SetActive(false);
        homePage.SetActive(true);

        replingNameText.text = data.replingName;
        if (data.appearanceIndex >= 0 && data.appearanceIndex < replingSprites.Length)
            replingImage.sprite = replingSprites[data.appearanceIndex];
        else
            Debug.LogWarning("Appearance index out of range for local data: " + data.appearanceIndex);
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