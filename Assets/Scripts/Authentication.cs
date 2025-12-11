using UnityEngine;
using Firebase.Auth;
using TMPro;
using Firebase.Extensions;
using UnityEngine.UI;
using Firebase;

public class Authentication : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPasswordInput;

    [Header("UI Groups")]
    public GameObject errorBackground;
    public GameObject errorMessage;
    public Button errorOkButton;

    public GameObject successBackground;
    public GameObject successMessage;
    public Button successOkButton;

    public GameObject loginFailBackground;
    public GameObject loginFailMessage;
    public Button loginFailOkButton;

    [Header("Pages")]
    public GameObject registrationPage;
    public GameObject loginPage;
    // public GameObject mainPage; // Removed this, ReplingManager handles the Home Page now

    [Header("Connections")]
    public ReplingManager replingManager; // Renamed for clarity. Drag ReplingManager here!

    void Start()
    {
        // 1. Check for Dependencies
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == Firebase.DependencyStatus.Available)
            {
                Debug.Log("Firebase is ready");
            }
            else
            {
                Debug.LogError("Firebase Error: " + task.Result);
            }
        });

        // 2. Hide Popups
        HideErrorUI();
        HideSuccessUI();
        HideLoginFailUI();

        // 3. Setup Buttons
        errorOkButton.onClick.AddListener(HideErrorUI);
        successOkButton.onClick.AddListener(OnSuccessOkPressed);
        loginFailOkButton.onClick.AddListener(HideLoginFailUI);


    }

    // --- SIGN UP LOGIC ---
    public void SignUp()
    {
        if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            ShowErrorUI();
            return;
        }

        FirebaseAuth.DefaultInstance
            .CreateUserWithEmailAndPasswordAsync(emailInput.text, passwordInput.text)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    ShowErrorUI();
                    return;
                }
                ShowSuccessUI();
            });
    }

    // --- LOGIN LOGIC ---
    public void Login()
    {
        if (string.IsNullOrEmpty(loginEmailInput.text) || string.IsNullOrEmpty(loginPasswordInput.text))
        {
            ShowLoginFailUI();
            return;
        }

        Debug.Log("Attempting Login..."); // Debug to see if button works

        FirebaseAuth.DefaultInstance
            .SignInWithEmailAndPasswordAsync(loginEmailInput.text, loginPasswordInput.text)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("Login Failed: Wrong Password");
                    ShowLoginFailUI();
                    return;
                }

                Debug.Log("Login Success! Handing off to ReplingManager...");
                // CHAINING: Only call this if login worked!
                OnLoginSuccess();
            });
    }

    void OnLoginSuccess()
    {
        if (replingManager != null)
        {
            replingManager.BeginLoadingRoutine(); // Changed name to be clearer
        }
        else
        {
            Debug.LogError("Cannot load game: ReplingManager is missing!");
        }
    }

    // --- UI HELPER FUNCTIONS ---
    public void ShowErrorUI() { errorBackground.SetActive(true); errorMessage.SetActive(true); errorOkButton.gameObject.SetActive(true); }
    public void HideErrorUI() { errorBackground.SetActive(false); errorMessage.SetActive(false); errorOkButton.gameObject.SetActive(false); }

    public void ShowSuccessUI() { successBackground.SetActive(true); successMessage.SetActive(true); successOkButton.gameObject.SetActive(true); }
    public void HideSuccessUI() { successBackground.SetActive(false); successMessage.SetActive(false); successOkButton.gameObject.SetActive(false); }

    public void ShowLoginFailUI() { loginFailBackground.SetActive(true); loginFailMessage.SetActive(true); loginFailOkButton.gameObject.SetActive(true); }
    public void HideLoginFailUI() { loginFailBackground.SetActive(false); loginFailMessage.SetActive(false); loginFailOkButton.gameObject.SetActive(false); }

    void OnSuccessOkPressed()
    {
        HideSuccessUI();
        registrationPage.SetActive(false);
        loginPage.SetActive(true);
    }
}