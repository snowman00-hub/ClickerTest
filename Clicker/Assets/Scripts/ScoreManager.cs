using UnityEngine;
using Firebase.Database;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance;
    public static ScoreManager Instance => instance;

    private DatabaseReference scoresRef;

    private int cachedBestScore = 0;
    public int CachedBestScore => cachedBestScore;

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

        scoresRef = FirebaseDatabase.DefaultInstance.RootReference.Child("scores");

        Debug.Log("[Score] �ʱ�ȭ �Ϸ�");
        await LoadBestScoreAsync();
    }

    private async UniTask<int> LoadBestScoreAsync()
    {
        if (!AuthManager.Instance.IsLoggedIn)
            return 0;

        string uid = AuthManager.Instance.UserId;

        try
        {
            DataSnapshot snapshot = await scoresRef.Child(uid).Child("bestScore").GetValueAsync().AsUniTask();
            if (snapshot.Exists)
            {
                cachedBestScore = int.Parse(snapshot.Value.ToString());
                Debug.Log($"[Score] �ְ� ��� �ε�: {cachedBestScore}");
            }
            else
            {
                cachedBestScore = 0;
                Debug.Log("[Score] �ְ� ��� ����");
            }

        }
        catch(System.Exception ex)
        {
            Debug.LogError($"[Score] �ְ� ��� �ε� ���� : {ex.Message}");
        }

        return 0;
    }

    public async UniTask<(bool success, string error)> SaveScoreAsync(int score)
    {
        if (!AuthManager.Instance.IsLoggedIn)
            return (false, "�α��� �ʿ�");

        string uid = AuthManager.Instance.UserId;

        try
        {
            Debug.Log($"[Score] ���� ���� �õ�: {score}");

            DatabaseReference historyRef = scoresRef.Child(uid).Child("history");
            DatabaseReference newHistoryRef = historyRef.Push(); // pushId Ref ��ȯ

            // json or Dictionary �� ���� ����
            var scoreData = new Dictionary<string, object>();
            scoreData.Add("score", score);
            scoreData.Add("timestamp", ServerValue.Timestamp);

            await newHistoryRef.UpdateChildrenAsync(scoreData).AsUniTask();

            bool shouldUpdateBestScore = false;
            if(cachedBestScore == 0)
            {
                var bestScoreSnapshot = await scoresRef.Child(uid).Child("bestScore")
                    .GetValueAsync().AsUniTask();

                if (!bestScoreSnapshot.Exists)
                {
                    shouldUpdateBestScore = true;
                }
                else if ( score> cachedBestScore)
                {
                    shouldUpdateBestScore = true;
                }
            }
            else if (score > cachedBestScore) 
            {
                shouldUpdateBestScore = true;
            }

            if (shouldUpdateBestScore)
            {
                await UpdateBestScoreAsync(score);
            }

            Debug.Log($"���� ���� ����: {score}");
            return (true, null);
        }
        catch(System.Exception ex)
        {
            Debug.LogError($"[Score] ���� ���� ����! {ex.Message}");
            return (false, ex.Message);
        }
    }

    private async UniTask UpdateBestScoreAsync(int newBestScore)
    {
        if (!AuthManager.Instance.IsLoggedIn)
            return;

        string uid = AuthManager.Instance.UserId;

        try
        {
            await scoresRef.Child(uid).Child("bestScore").SetValueAsync(newBestScore).AsUniTask();
            cachedBestScore = newBestScore;

            Debug.Log($"[Score] �ְ� ��� ����: {newBestScore}");
        }
        catch(System.Exception ex)
        {
            Debug.LogErrorFormat("[Score] �ְ� ��� ���� ���� {0}", ex.Message );
        }
    }

    public async UniTask<List<ScoreData>> LoadHistoryAsync(int limit = 10)
    {
        var list = new List<ScoreData>();

        if (!AuthManager.Instance.IsLoggedIn)
        {
            return list;
        }

        string uid = AuthManager.Instance.UserId;
        try
        {
            Debug.Log($"[Score] �����丮 �ε� �õ�");
            DatabaseReference historyRef = scoresRef.Child(uid).Child("history");
            Query query = historyRef.OrderByChild("timestamp").LimitToLast(limit);

            DataSnapshot snapshot = await query.GetValueAsync().AsUniTask();
            if (snapshot.Exists)
            {
                foreach(DataSnapshot child in snapshot.Children)
                {
                    string json = child.GetRawJsonValue();
                    ScoreData data = ScoreData.FromJson(json);
                    list.Add(data);
                }
            }

            Debug.Log($"[Score] �����丮 �ε� ����: {list.Count}��");
        }
        catch(System.Exception ex)
        {
            Debug.LogError($"[Score] �����丮 �ε� ����: {ex.Message}");
        }

        return list;
    }
}