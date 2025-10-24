using UnityEngine;
using Firebase;
using Cysharp.Threading.Tasks;

public class FirebaseInitializer : MonoBehaviour
{
    private static FirebaseInitializer instance;
    public static FirebaseInitializer Instance => instance;

    private bool isInitialized = false;
    public bool IsInitialized => isInitialized;

    private FirebaseApp firebaseApp;
    public FirebaseApp FirebaseApp => firebaseApp;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeFirebaseAsync().Forget();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private async UniTaskVoid InitializeFirebaseAsync()
    {
        Debug.Log("[Firebase] �ʱ�ȭ ����");

        try
        {
            var status = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();
            if (status == DependencyStatus.Available)
            {
                firebaseApp = FirebaseApp.DefaultInstance;
                isInitialized = true;

                Debug.Log($"[Firebase] �ʱ�ȭ ����! {firebaseApp.Name}");
            }
            else
            {
                Debug.LogError($"[Firebase] �ʱ�ȭ ����: {status}");
                isInitialized = false;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Firebase] �ʱ�ȭ ����: {ex.Message}");
            isInitialized = false;
        }
    }

    public async UniTask WaitForInitilazationAsync()
    {
        await UniTask.WaitUntil(() => isInitialized);
    }
}