using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public GameObject loginPanel;

    [SerializeField]
    private TMP_InputField emailInput;
    [SerializeField]
    private TMP_InputField passwdInput;

    [SerializeField]
    private Button loginButton;
    [SerializeField]
    private Button signUpButton;
    [SerializeField]
    private Button anonymouslyLoginButton;

    [SerializeField]
    private TextMeshProUGUI errorText;

    //
    public Button profileButton;
    public TextMeshProUGUI profileText;

    public Button LogOutButton;

    public ProfileUI profileUI;

    private async UniTaskVoid Start()
    {
        SetButtonsInteractable(false);

        await UniTask.WaitUntil(() => AuthManager.Instance != null && AuthManager.Instance.IsInitialized);

        loginButton.onClick.AddListener(() => OnLoginButtonClicked().Forget());
        signUpButton.onClick.AddListener(() => OnSignUpButtonClicked().Forget());
        anonymouslyLoginButton.onClick.AddListener(() => OnAnonyMouslyLoginButtonClicked().Forget());
        profileButton.onClick.AddListener(() =>
        {
            UpdateUI();
        });
        LogOutButton.onClick.AddListener(() =>
        {
            AuthManager.Instance.SignOut();
            UpdateUI();
        });

        SetButtonsInteractable(true);

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (AuthManager.Instance == null || !AuthManager.Instance.IsInitialized)
            return;

        bool isLoggedIn = AuthManager.Instance.IsLoggedIn;
        loginPanel.SetActive(!isLoggedIn);

        if (isLoggedIn)
        {
            string userId = AuthManager.Instance.UserId;
            profileText.text = userId;
        }
        else
        {
            profileText.text = string.Empty;
        }

        errorText.text = string.Empty;
    }

    private async UniTaskVoid OnLoginButtonClicked()
    {
        string email = emailInput.text;
        string password = passwdInput.text;

        SetButtonsInteractable(false);

        var (success, error) = await AuthManager.Instance.SignInWithEmailAsync(email, password);
        if (success)
        {
            await ProfileManager.Instance.LoadProfileAsync();
            await profileUI.ProfileUIActiveAsync();
        }
        else
        {
            ShowError(error);
        }

        SetButtonsInteractable(true);
        UpdateUI();
    }

    private async UniTaskVoid OnSignUpButtonClicked()
    {
        string email = emailInput.text;
        string password = passwdInput.text;

        SetButtonsInteractable(false);

        var (success, error) = await AuthManager.Instance.CreateUserWithEmailAsync(email, password);
        if (success)
        {

        }
        else
        {
            ShowError(error);
        }

        SetButtonsInteractable(true);
        UpdateUI();
    }

    private async UniTaskVoid OnAnonyMouslyLoginButtonClicked()
    {
        SetButtonsInteractable(false);

        var (success, error) = await AuthManager.Instance.SignInAnonymouslyAsync();
        if (success)
        {
            await ProfileManager.Instance.LoadProfileAsync();
            await profileUI.ProfileUIActiveAsync();
        }
        else
        {
            ShowError(error);
        }

        SetButtonsInteractable(true);
        UpdateUI();
    }

    private void ShowError(string message)
    {
        errorText.text = message;
        errorText.color = Color.red;
    }

    private void SetButtonsInteractable(bool active)
    {
        loginButton.interactable = active;
        signUpButton.interactable = active;
        anonymouslyLoginButton.interactable = active;
    }
}