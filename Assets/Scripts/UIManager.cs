using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

    private GameManager gameManager;

    [Header("Main UI")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI balanceText;
    public Button nextButton;

    [Header("Scenario UI")]
    public GameObject scenarioPanel;
    public TextMeshProUGUI scenarioText;
    public Button scenarioOption1;
    public Button scenarioOption2;
    public Button scenarioOption3;

    [Header("Feedback UI")]
    public GameObject feedbackPanel;
    public TextMeshProUGUI feedbackText;

    [Header("Jobs UI")]
    public Button findJobButton;
    public GameObject jobPanel;
    public Transform jobListContainer;
    public GameObject jobButtonPrefab;
    public TextMeshProUGUI currentJobText;

    [Header("Budget UI")]
    public GameObject budgetPanel;
    public TextMeshProUGUI monthlyIncomeText;
    public TextMeshProUGUI monthlyExpensesText;
    public TextMeshProUGUI budgetStatusText;
    public Button toggleBudgetButton;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button restartButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        nextButton.onClick.AddListener(() => gameManager.OnNextButtonPressed());
        scenarioPanel.SetActive(false);
        jobPanel.SetActive(false);
        
        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
            
        if (budgetPanel != null)
            budgetPanel.SetActive(false);
            
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Toggle panels on button click
        findJobButton.onClick.AddListener(ToggleJobPanel);
        toggleBudgetButton.onClick.AddListener(ToggleBudgetPanel);
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
        // Color code balance based on amount, green if positive, red if negative
        string colorCode = balance >= 0 ? "#00FF00" : "#FF0000";
        balanceText.text = $"Balance: <color={colorCode}>${balance:F2}</color>";
    }

    // Feedback System
    public void ShowFeedback(string message)
    {
        if (feedbackPanel == null) return;
        
        StopAllCoroutines();
        feedbackPanel.SetActive(true);
        feedbackText.text = message;
    }


    public void ShowScenarioPanel(ScenarioData scenarioData)
    {
        scenarioPanel.SetActive(true);
        scenarioText.text = scenarioData.scenarioText;

        // Set up choice buttons
        SetupChoiceButton(scenarioOption1, scenarioData.choices[0]);
        SetupChoiceButton(scenarioOption2, scenarioData.choices[1]);
        SetupChoiceButton(scenarioOption3, scenarioData.choices[2]);
    }

    void SetupChoiceButton(Button button, ScenarioChoice choice)
    {
        button.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => HandleScenarioChoice(choice));
    }

    void HandleScenarioChoice(ScenarioChoice choice)
    {
        gameManager.AdjustBalance(choice.moneyChange);
        
        // Record as expense if money was spent
        if (choice.moneyChange < 0)
        {
            gameManager.financeManager.RecordExpense(
                gameManager.day, 
                "Scenario", 
                Mathf.Abs(choice.moneyChange), 
                choice.outcomeText
            );
        }
        
        ShowFeedback(choice.outcomeText);
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
                ShowFeedback("Day " + gameManager.day + " - " + "Started Job: " + job.jobName);
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
            currentJobText.text = "";
        else
            currentJobText.text = $"Current Job: {job.jobName} (${job.payAmount}/{job.paySchedule})";
    }

    // Budget Panel
    void ToggleBudgetPanel()
    {
        if (budgetPanel == null) return;
        
        bool isActive = budgetPanel.activeSelf;
        budgetPanel.SetActive(!isActive);
        
        if (!isActive)
        {
            UpdateBudgetDisplay();
        }
    }

    public void UpdateBudgetDisplay()
    {
        if (gameManager.financeManager == null) return;
        
        float income = gameManager.financeManager.GetMonthlyIncome();
        float expenses = gameManager.financeManager.GetMonthlyExpenses();
        float surplus = income - expenses;
        
        monthlyIncomeText.text = $"Monthly Income: ${income:F2}";
        monthlyExpensesText.text = $"Monthly Expenses: ${expenses:F2}";
        
        // Green for positive, red for negative
        string statusColor = surplus >= 0 ? "#00FF00" : "#FF0000";
        string statusText = surplus >= 0 ? "Surplus" : "Deficit";
        budgetStatusText.text = $"<color={statusColor}>{statusText}: ${Mathf.Abs(surplus):F2}</color>";
    }

    // Game Over
    public void ShowGameOver(string reason)
    {
        if (gameOverPanel == null) return;
        
        gameOverPanel.SetActive(true);
        gameOverText.text = $"Game Over!\n\n{reason}";
        
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() => 
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                );
            });
        }
    }

}
