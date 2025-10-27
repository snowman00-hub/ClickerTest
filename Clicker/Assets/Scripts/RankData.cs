using System;
using UnityEngine;

[Serializable]
public class RankData : ScoreData
{
    public string userId;
    public string nickname;

    public RankData()
    {

    }

    public RankData(string userId, string nickname, int score, long timestamp)
        : base(score, timestamp)
    {
        this.userId = userId;
        this.nickname = nickname;
    }

    public static new RankData FromJson(string json)
    {
        return JsonUtility.FromJson<RankData>(json);
    }
}