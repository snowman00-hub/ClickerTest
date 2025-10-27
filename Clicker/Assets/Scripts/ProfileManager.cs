using Cysharp.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    private static ProfileManager instance;
    public static ProfileManager Instance => instance;

    private DatabaseReference databaseRef;
    private DatabaseReference usersRef;

    private UserProfile cachedProfile;
    public UserProfile CachedProfile => cachedProfile;

    private bool isInitialized = false;
    public bool IsInitialized => isInitialized;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {

        }
    }

    private async UniTaskVoid Start()
    {
        await UniTask.WaitUntil(()=>AuthManager.Instance.IsInitialized);

        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        usersRef = databaseRef.Child("users");
        await LoadProfileAsync();

        Debug.Log("[Profile] ProfileManager 초기화 완료");

        isInitialized = true;
    }

    public async UniTask<(bool success, string error)> SaveProfileAsync(string nickname)
    {
        if (!AuthManager.Instance.IsLoggedIn)
        {
            return (false, "[Profile] 로그인 X");
        }

        string userId = AuthManager.Instance.UserId;
        string email = AuthManager.Instance.CurrentUser.Email ?? "익명";

        try
        {
            Debug.Log($"[Profile] 프로필 저장 시도 {nickname}");

            UserProfile profile = new UserProfile(nickname, email);
            string json = profile.ToJson();

            await usersRef.Child(userId).SetRawJsonValueAsync(json).AsUniTask();

            cachedProfile = profile;

            Debug.Log($"[Profile] 프로필 저장 성공 {nickname}");
            return (true, null);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Profile] 프로필 저장 실패 {ex.Message}");
            return (false, ex.Message);
        }
    }

    public async UniTask<(UserProfile profile, string error)> LoadProfileAsync()
    {
        if (!AuthManager.Instance.IsLoggedIn)
        {
            return (null, "[Profile] 로그인 X");
        }

        string userId = AuthManager.Instance.UserId;

        try
        {
            Debug.Log($"[Profile] 프로필 로드 시도 {userId}");
            DataSnapshot snapshot = await usersRef.Child(userId).GetValueAsync().AsUniTask();

            if (!snapshot.Exists)
            {
                Debug.Log($"[Profile] 프로필 없음");
                return (null, "[Profile] 프로필 없음");
            }

            string json = snapshot.GetRawJsonValue();
            cachedProfile = UserProfile.FromJson(json);

            Debug.Log($"[Profile] 프로필 로드 성공 {userId}");
            return (cachedProfile, null);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Profile] 프로필 로드 실패 {ex.Message}");
            return (null, ex.Message);
        }
    }

    public async UniTask<(bool success, string error)> UpdateNicknameAsync(string newNickname)
    {
        if (!AuthManager.Instance.IsLoggedIn)
        {
            return (false, "로그인 X");
        }

        string userId = AuthManager.Instance.UserId;

        try
        {
            Debug.Log($"[Profile] 닉네임 변경 시도 {newNickname}");

            await usersRef.Child(userId).Child("nickname").SetValueAsync(newNickname).AsUniTask();

            cachedProfile.nickName = newNickname;

            Debug.Log($"[Profile] 닉네임 변경 성공 {newNickname}");
            return (true, null);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Profile] 닉네임 변경 실패 {ex.Message}");
            return (false, ex.Message);
        }
    }

    public async UniTask<bool> ProfileExistAsync()
    {
        if (!AuthManager.Instance.IsLoggedIn)
        {
            return false;
        }

        string userId = AuthManager.Instance.UserId;

        try
        {
            DataSnapshot snapshot = await usersRef.Child(userId).GetValueAsync().AsUniTask();
            return snapshot.Exists;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Profile] 프로필 확인 실패: {ex.Message}");
            return false;
        }
    }
}