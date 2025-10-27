using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ScoreUI : MonoBehaviour
{
    public Button restartButton;
    public Button lookRecordButton;
    public Button lookReaderBoardButton;

    public TextMeshProUGUI bestRecordText;
    public GameObject recordPrefab;
    public Transform recordPrefabParent;

    public GameObject recordsWindow;

    public Button recordsResetButton;
    public Button recordsCloseButton;

    // ��������
    public GameObject leaderboardWindow;
    public Transform leaderboardRecordParent;
    public Button leaderboardRefreshButton;
    public Button leaderboardCloseButton;

    private void Start()
    {
        lookRecordButton.onClick.AddListener(() => ShowRecordsAsync().Forget());
        recordsResetButton.onClick.AddListener(() => ReFreshRecords());
        recordsCloseButton.onClick.AddListener(() => CloseRecordWindow());
    }

    private void ReFreshRecords()
    {
        CloseRecordWindow();
        ShowRecordsAsync().Forget();
    }

    private void CloseRecordWindow()
    {
        bestRecordText.text = "�ְ� ���: 0��";
        for(int i=0;i<recordPrefabParent.childCount;i++)
        {
            Destroy(recordPrefabParent.GetChild(i).gameObject);
        }
        recordsWindow.SetActive(false);
    }

    private async UniTaskVoid ShowLeaderboardAsync()
    {
        SetInteractableScoreWindow(false);

        try
        {
            /////////// ���� ����� ��
            var list = await LeaderBoardManager.Instance.LoadRankDatasAsync();
            for(int i = 0; i < list.Count; i++)
            {
                var tmp = Instantiate(recordPrefab, leaderboardRecordParent).GetComponentInChildren<TextMeshProUGUI>();
                tmp.text = $"{i, -5} {list[i].userId, - 15} {list[i].score,5}��";
            }

            leaderboardWindow.SetActive(true);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"���� ���� ���� ���� : {ex.Message}");
        }

        SetInteractableScoreWindow(true);
    }

    private async UniTaskVoid ShowRecordsAsync()
    {
        SetInteractableScoreWindow(false);

        try
        {
            await ScoreManager.Instance.LoadBestScoreAsync();
            bestRecordText.text = $"�ְ� ���: {ScoreManager.Instance.CachedBestScore}��";
            var list = await ScoreManager.Instance.LoadHistoryAsync();
            foreach(var item in list)
            {
                var tmp = Instantiate(recordPrefab, recordPrefabParent).GetComponentInChildren<TextMeshProUGUI>();
                tmp.text = $"{item.score}�� - {item.GetDateString()}";
            }

            recordsWindow.SetActive(true);
        }
        catch(System.Exception ex)
        {
            Debug.Log($"��� ���� ���� : {ex.Message}");
        }

        SetInteractableScoreWindow(true);
    }

    private void SetInteractableScoreWindow(bool active)
    {
        restartButton.interactable = active;
        lookRecordButton.interactable = active;
        lookReaderBoardButton.interactable = active;
    }
}