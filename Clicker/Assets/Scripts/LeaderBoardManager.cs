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

        Debug.Log("[LeaderBoard] �ʱ�ȭ �Ϸ�");
    }

    public async UniTask<(bool success, string error)> SaveRankDataAsync()
    {
        if (!AuthManager.Instance.IsLoggedIn)
            return (false, "�α��� �ʿ�");

        string uid = AuthManager.Instance.UserId;

        try
        {
            Debug.Log($"[Leaderboard] �������� ���� �õ�");

            var rankData = new Dictionary<string, object>();
            rankData.Add("nickname", ProfileManager.Instance.CachedProfile.nickName);
            rankData.Add("score", ScoreManager.Instance.CachedBestScore);
            rankData.Add("timestamp", ServerValue.Timestamp);
            await leaderboardRef.Child(uid).SetValueAsync(rankData).AsUniTask();

            Debug.Log($"[Leaderboard] �������� ���� ����");
            return (true, null);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Leaderboard] �������� ���� ����! {ex.Message}");
            return (false, ex.Message);
        }
    }

    public async UniTask<List<RankData>> LoadRankDatasAsync(int limit = 10)
    {
        var list = new List<RankData>();

        if (!AuthManager.Instance.IsLoggedIn)
        {
            return list;
        }

        try
        {
            Debug.Log($"[LeaderBoard] �������� �ε� �õ�");
            Query query = leaderboardRef.OrderByChild("score").LimitToLast(limit);

            DataSnapshot snapshot = await query.GetValueAsync().AsUniTask();
            if (snapshot.Exists)
            {
                foreach (DataSnapshot child in snapshot.Children)
                {
                    string json = child.GetRawJsonValue();
                    RankData data = RankData.FromJson(json);
                    list.Add(data);
                }
            }

            Debug.Log($"[LeaderBoard] �������� �ε� ����");
        }
        catch (System.Exception ex) 
        {
            Debug.Log($"[LeaderBoard] �������� �ε� ���� {ex.Message}");
        }

        list.Reverse();
        return list;
    }
}