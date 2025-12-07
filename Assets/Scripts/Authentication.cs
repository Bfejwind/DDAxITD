using UnityEngine;
using Firebase.Auth;
using TMPro;
using Firebase.Extensions;
using UnityEngine.UI;
using Firebase;

public class Authentication : MonoBehaviour
{
    public TMP_InputField emailInput;         // registration email
    public TMP_InputField passwordInput;      // registration password

    public TMP_InputField loginEmailInput;    // login email
    public TMP_InputField loginPasswordInput; // login password

    public GameObject errorBackground;
    public GameObject errorMessage;
    public Button errorOkButton;

    public GameObject successBackground;
    public GameObject successMessage;
    public Button successOkButton;

    public GameObject loginFailBackground;
    public GameObject loginFailMessage;
    public Button loginFailOkButton;

    public GameObject registrationPage;
    public GameObject loginPage;
    public GameObject mainPage;

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

    public void Login()
    {
        if (string.IsNullOrEmpty(loginEmailInput.text) || string.IsNullOrEmpty(loginPasswordInput.text))
        {
            ShowLoginFailUI();
            return;
        }

        FirebaseAuth.DefaultInstance
            .SignInWithEmailAndPasswordAsync(loginEmailInput.text, loginPasswordInput.text)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    ShowLoginFailUI();
                    return;
                }

                OnLoginSuccess();
            });
    }

    void OnLoginSuccess()
    {
        loginPage.SetActive(false);
        mainPage.SetActive(true);
    }

    public void ShowErrorUI()
    {
        errorBackground.SetActive(true);
        errorMessage.SetActive(true);
        errorOkButton.gameObject.SetActive(true);
    }

    public void HideErrorUI()
    {
        errorBackground.SetActive(false);
        errorMessage.SetActive(false);
        errorOkButton.gameObject.SetActive(false);
    }

    public void ShowSuccessUI()
    {
        successBackground.SetActive(true);
        successMessage.SetActive(true);
        successOkButton.gameObject.SetActive(true);
    }

    public void HideSuccessUI()
    {
        successBackground.SetActive(false);
        successMessage.SetActive(false);
        successOkButton.gameObject.SetActive(false);
    }

    public void ShowLoginFailUI()
    {
        loginFailBackground.SetActive(true);
        loginFailMessage.SetActive(true);
        loginFailOkButton.gameObject.SetActive(true);
    }

    public void HideLoginFailUI()
    {
        loginFailBackground.SetActive(false);
        loginFailMessage.SetActive(false);
        loginFailOkButton.gameObject.SetActive(false);
    }

    void OnSuccessOkPressed()
    {
        HideSuccessUI();
        registrationPage.SetActive(false);
        loginPage.SetActive(true);
    }

    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == Firebase.DependencyStatus.Available)
            {
                Debug.Log("Firebase is ready");
            }
            else
            {
                Debug.LogError("Firebase not available: " + task.Result);
            }
        });

        HideErrorUI();
        HideSuccessUI();
        HideLoginFailUI();

        errorOkButton.onClick.AddListener(HideErrorUI);
        successOkButton.onClick.AddListener(OnSuccessOkPressed);
        loginFailOkButton.onClick.AddListener(HideLoginFailUI);
    }
}
