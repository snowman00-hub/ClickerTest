using System;
using UnityEngine;

[Serializable]
public class UserProfile
{
    public string nickName;
    public string email;
    public long createdAt;

    public UserProfile()
    {

    }

    public UserProfile(string nickName, string email)
    {
        this.nickName = nickName;
        this.email = email;
        createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static UserProfile FromJson(string json)
    {
        return JsonUtility.FromJson<UserProfile>(json);
    }
}