using System;
using UnityEngine;

[Serializable]
public class ScoreData
{
    public int score;
    public long timestamp;

    public ScoreData()
    {

    }

    public ScoreData(int score, long timestamp)
    {
        this.score = score;
        this.timestamp = timestamp;
    }

    public DateTime GetDateTime()
    {
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
    }

    public string GetDateString()
    {
        return GetDateTime().ToString("yyyy-MM-dd HH:mm:ss");
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static ScoreData FromJson(string json)
    {
        return JsonUtility.FromJson<ScoreData>(json);
    }
}