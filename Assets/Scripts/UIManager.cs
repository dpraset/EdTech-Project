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
    public Button scenarioOption3;

    // Jobs
    public Button findJobButton;
    public GameObject jobPanel;
    public Transform jobListContainer;
    public GameObject jobButtonPrefab;
    //public Button closeJobPanelButton;
    public TextMeshProUGUI currentJobText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        nextButton.onClick.AddListener(() => gameManager.OnNextButtonPressed());
        scenarioPanel.SetActive(false);
        jobPanel.SetActive(false);
        
        // Toggle job panel on button click
        findJobButton.onClick.AddListener(ToggleJobPanel);
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

    public void ShowScenarioPanel(ScenarioData scenarioData)
    {
        scenarioPanel.SetActive(true);
        //scenarioText.text = message;
        scenarioText.text = scenarioData.scenarioText;

        // Example options for now:
        scenarioOption1.GetComponentInChildren<TextMeshProUGUI>().text =
        scenarioData.choices[0].choiceText;

        scenarioOption2.GetComponentInChildren<TextMeshProUGUI>().text =
            scenarioData.choices[1].choiceText;

        scenarioOption3.GetComponentInChildren<TextMeshProUGUI>().text =
            scenarioData.choices[2].choiceText;

        scenarioOption1.onClick.RemoveAllListeners();
        scenarioOption2.onClick.RemoveAllListeners();
        scenarioOption3.onClick.RemoveAllListeners();

        scenarioOption1.onClick.AddListener(() => HandleScenarioChoice(scenarioData.choices[0]));
        scenarioOption2.onClick.AddListener(() => HandleScenarioChoice(scenarioData.choices[1]));
        scenarioOption3.onClick.AddListener(() => HandleScenarioChoice(scenarioData.choices[2]));
    }

    void HandleScenarioChoice(ScenarioChoice choice)
    {
        gameManager.AdjustBalance(choice.moneyChange);
        Debug.Log(choice.outcomeText);
        HideScenarioPanel();
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
    }

    void ToggleJobPanel()
    {
        bool isActive = jobPanel.activeSelf;
        if (!isActive)
        {
            // Open the job panel
            gameManager.OpenJobPanel();
        }
        else
        {
            // Close the panel
            jobPanel.SetActive(false);
        }
    }

    public void UpdateCurrentJob(Job job)
    {
        if (job == null)
            currentJobText.text = "Current Job: Unemployed";
        else
            currentJobText.text = $"Current Job: {job.jobName} (${job.payAmount}/{job.paySchedule})";
    }

}
