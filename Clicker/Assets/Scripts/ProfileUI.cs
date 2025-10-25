using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    public GameObject nicknameInitPanel;
    public TMP_InputField nicknameInitInput;
    public Button nicknameInitButton;

    public TextMeshProUGUI nicknameInProfileWindow;
    public TextMeshProUGUI nicknameInEditWindow;

    public TMP_InputField nicknameEditInput;
    public Button nicknameEditButton;
    public Button editWindowCloseButton;

    private async UniTaskVoid Start()
    {
        await UniTask.WaitUntil(() => ProfileManager.Instance.IsInitialized);
        await ProfileUIActiveAsync();

        nicknameInitButton.onClick.AddListener(() => NicknameInitAsync().Forget());
        nicknameEditButton.onClick.AddListener(() => NicknameEditAsync().Forget());
    }

    public async UniTask ProfileUIActiveAsync()
    {
        bool isProfileExist = await ProfileManager.Instance.ProfileExistAsync();

        if (isProfileExist)
        {
            nicknameInitPanel.SetActive(false);
            UpdateProfileUI();
        }
        else if (AuthManager.Instance.IsLoggedIn)
        {
            nicknameInitPanel.SetActive(true);
        }
    }

    private async UniTaskVoid NicknameEditAsync()
    {
        SetInteractableEditWindow(false);

        try
        {
            await ProfileManager.Instance.UpdateNicknameAsync(nicknameEditInput.text);
            UpdateProfileUI();
            nicknameEditInput.text = string.Empty;
        }
        catch
        {

        }

        SetInteractableEditWindow(true);
    }

    private async UniTaskVoid NicknameInitAsync()
    {
        nicknameInitInput.interactable = false;
        nicknameInitButton.interactable = false;

        try
        {
            var (success, error) = await ProfileManager.Instance.SaveProfileAsync(nicknameInitInput.text);
            nicknameInitPanel.SetActive(false);

            UpdateProfileUI();
        }
        catch
        {

        }

        nicknameInitInput.interactable = true;
        nicknameInitButton.interactable = true;
    }

    private void UpdateProfileUI()
    {
        var profile = ProfileManager.Instance.CachedProfile;
        nicknameInProfileWindow.text = $"닉네임 : {profile?.nickName ?? string.Empty}";
        nicknameInEditWindow.text = $"현재 : {profile?.nickName ?? string.Empty}";
    }

    private void SetInteractableEditWindow(bool active)
    {
        editWindowCloseButton.interactable = active;
        nicknameEditButton.interactable = active;
        nicknameEditInput.interactable = active;
    }
}