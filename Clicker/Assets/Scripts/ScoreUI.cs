using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public Button restartButton;
    public Button lookRecordButton;
    public Button lookReaderBoardButton;

    public TextMeshProUGUI bestRecordText;
    public GameObject recordPrefab;
    public Transform recordPrefabParent;

    public GameObject recordsWindow;

    private void Start()
    {
        lookRecordButton.onClick.AddListener(() => ShowRecordsAsync().Forget());       
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