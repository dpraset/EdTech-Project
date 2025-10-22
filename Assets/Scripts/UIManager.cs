using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

    private GameManager gameManager;

    public TextMeshProUGUI dayText;
    public TextMeshProUGUI balanceText;
    public Button nextButton;

    // Scenario UI
    public GameObject scenarioPanel;
    public TextMeshProUGUI scenarioText;
    public Button scenarioOption1;
    public Button scenarioOption2;

    // Jobs
    public Button findJobButton;
    public GameObject jobPanel;
    public Transform jobListContainer;
    public GameObject jobButtonPrefab;
    public Button closeJobPanelButton;
    public TextMeshProUGUI currentJobText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        nextButton.onClick.AddListener(() => gameManager.OnNextButtonPressed());
        scenarioPanel.SetActive(false);
        jobPanel.SetActive(false);
        findJobButton.onClick.AddListener(() => gameManager.OpenJobPanel());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDayText(int day, bool isDay)
    {
        string timeOfDay = isDay ? "Daytime" : "Nighttime";
        dayText.text = $"Day {day} - {timeOfDay}";
    }

    public void UpdateBalance(float balance)
    {
        balanceText.text = $"Balance: ${balance:F2}";
    }

    public void ShowScenarioPanel(string message)
    {
        scenarioPanel.SetActive(true);
        scenarioText.text = message;

        // Example options for now:
        scenarioOption1.GetComponentInChildren<TextMeshProUGUI>().text = "Pay rent (-$500)";
        scenarioOption2.GetComponentInChildren<TextMeshProUGUI>().text = "Delay payment";

        scenarioOption1.onClick.RemoveAllListeners();
        scenarioOption2.onClick.RemoveAllListeners();

        scenarioOption1.onClick.AddListener(() =>
        {
            gameManager.AdjustBalance(-500);
            HideScenarioPanel();
        });

        scenarioOption2.onClick.AddListener(() =>
        {
            Debug.Log("You chose to delay rent payment!");
            HideScenarioPanel();
        });
    }

    public void HideScenarioPanel()
    {
        scenarioPanel.SetActive(false);
    }

    public void ShowJobSelection(List<Job> jobs)
    {
        jobPanel.SetActive(true);

        // Clear previous job buttons
        foreach (Transform child in jobListContainer)
            Destroy(child.gameObject);

        foreach (var job in jobs)
        {
            GameObject button = Instantiate(jobButtonPrefab, jobListContainer);
            TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

            text.text = $"{job.jobName} - ${job.payAmount} ({job.paySchedule})";

            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                gameManager.currentJob = job;
                currentJobText.text = $"Current Job: {job.jobName} (${job.payAmount}/{job.paySchedule})";
                jobPanel.SetActive(false);
                Debug.Log("You chose the " + job.jobName + " job.");
            });
        }

        closeJobPanelButton.onClick.RemoveAllListeners();
        closeJobPanelButton.onClick.AddListener(() =>
        {
            jobPanel.SetActive(false);
        });
    }

    public void UpdateCurrentJob(Job job)
    {
        if (job == null)
            currentJobText.text = "Current Job: Unemployed";
        else
            currentJobText.text = $"Current Job: {job.jobName} (${job.payAmount}/{job.paySchedule})";
    }

}
