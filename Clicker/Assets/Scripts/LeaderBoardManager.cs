using Cysharp.Threading.Tasks;
using Firebase.Database;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class LeaderBoardManager : MonoBehaviour
{
    private static LeaderBoardManager instance;
    public static LeaderBoardManager Instance => instance;

    private DatabaseReference leaderboardRef;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private async UniTaskVoid Start()
    {
        await FirebaseInitializer.Instance.WaitForInitilazationAsync();

        leaderboardRef = FirebaseDatabase.DefaultInstance.RootReference.Child("leaderboard");

        Debug.Log("[LeaderBoard] 초기화 완료");
    }

    public async UniTask<(bool success, string error)> SaveRankDataAsync()
    {
        if (!AuthManager.Instance.IsLoggedIn)
            return (false, "로그인 필요");

        string uid = AuthManager.Instance.UserId;

        try
        {
            Debug.Log($"[Leaderboard] 저장 시도");

            var rankData = new Dictionary<string, object>();
            rankData.Add("nickname", ProfileManager.Instance.CachedProfile.nickName);
            rankData.Add("score", ScoreManager.Instance.CachedBestScore);
            rankData.Add("timestamp", ServerValue.Timestamp);
            await leaderboardRef.Child(uid).SetValueAsync(rankData).AsUniTask();

            Debug.Log($"[Leaderboard] 저장 성공");
            return (true, null);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Leaderboard] 저장 실패! {ex.Message}");
            return (false, ex.Message);
        }
    }

    public async UniTask<List<RankData>> LoadRankDataAsync(int limit = 10)
    {
        var list = new List<RankData>();

        try
        {
            Debug.Log($"[LeaderBoard] 로드 시도");
            
        }
        catch (System.Exception ex) 
        {
            Debug.Log($"[LeaderBoard] 로드 실패 {ex.Message}");
        }

        return list;
    }
}