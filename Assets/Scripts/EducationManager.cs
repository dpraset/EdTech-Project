using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EducationManager : MonoBehaviour
{
    [Header("Managers")]
    public GameManager gameManager;
    public PointsManager pointsManager;
    
    [Header("UI Panels")]
    public GameObject quizListPanel;
    public GameObject quizPanel;
    public GameObject resultsPanel;
    
    [Header("Quiz Buttons (Manually Created)")]
    public Button quiz1Button;
    public Button quiz2Button;
    public Button quiz3Button;
    public Button quiz4Button;
    public Button quiz5Button;
    
    [Header("Quiz UI")]
    public TextMeshProUGUI quizQuestionText;
    public Button quizOption1;
    public Button quizOption2;
    public Button quizOption3;
    public Button quizOption4;
    public TextMeshProUGUI resultFeedbackText;
    public Button nextQuestionButton;
    
    [Header("Results UI")]
    public TextMeshProUGUI resultsText;
    public Button backToQuizzesButton;
    
    [Header("Quiz Settings")]
    public int pointsPerCorrectAnswer = 10;
    public int perfectQuizBonus = 10;
    
    // Track which quizzes are unlocked
    private Dictionary<int, bool> quizUnlockStatus = new Dictionary<int, bool>()
    {
        { 1, false }, // Quiz 1: Locked initially
        { 2, false },
        { 3, false },
        { 4, false },
        { 5, false }
    };
    
    // Track which quizzes are completed
    private Dictionary<int, bool> quizCompletionStatus = new Dictionary<int, bool>()
    {
        { 1, false },
        { 2, false },
        { 3, false },
        { 4, false },
        { 5, false }
    };
    
    // Currently active quiz
    private QuizData currentQuiz;
    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;
    
    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
            
        if (pointsManager == null)
            pointsManager = FindObjectOfType<PointsManager>();
        
        SetupQuizButtons();
        ShowQuizListPanel();
        UpdateButtonStates();
        
        // Button listeners
        if (nextQuestionButton != null)
            nextQuestionButton.onClick.AddListener(LoadNextQuestion);
            
        if (backToQuizzesButton != null)
            backToQuizzesButton.onClick.AddListener(ShowQuizListPanel);
    }
    
    void SetupQuizButtons()
    {
        if (quiz1Button != null)
            quiz1Button.onClick.AddListener(() => StartQuiz(Quiz1_Budgeting.GetQuizData()));
            
        if (quiz2Button != null)
            quiz2Button.onClick.AddListener(() => StartQuiz(Quiz2_Expenses.GetQuizData()));
            
        if (quiz3Button != null)
            quiz3Button.onClick.AddListener(() => StartQuiz(Quiz3_EmergencyFunds.GetQuizData()));
            
        if (quiz4Button != null)
            quiz4Button.onClick.AddListener(() => StartQuiz(Quiz4_CreditDebt.GetQuizData()));
            
        if (quiz5Button != null)
            quiz5Button.onClick.AddListener(() => StartQuiz(Quiz5_Investing.GetQuizData()));
    }
    
    public void UnlockQuiz(int quizID)
    {
        if (quizUnlockStatus.ContainsKey(quizID))
        {
            quizUnlockStatus[quizID] = true;
            UpdateButtonStates();
            Debug.Log($"Quiz {quizID} unlocked!");
        }
    }
    
    void UpdateButtonStates()
    {
        UpdateQuizButton(quiz1Button, 1);
        UpdateQuizButton(quiz2Button, 2);
        UpdateQuizButton(quiz3Button, 3);
        UpdateQuizButton(quiz4Button, 4);
        UpdateQuizButton(quiz5Button, 5);
    }
    
    void UpdateQuizButton(Button button, int quizID)
    {
        if (button == null) return;
        
        bool isUnlocked = quizUnlockStatus[quizID];
        bool isCompleted = quizCompletionStatus[quizID];
        
        // Enable/disable button based on unlock status
        button.interactable = isUnlocked;
        
        // Update button appearance
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        Image buttonImage = button.GetComponent<Image>();
        
        if (buttonText != null)
        {
            string statusIcon = isCompleted ? "✓" : (isUnlocked ? "📝" : "🔒");
            string quizName = GetQuizName(quizID);
            
            if (isUnlocked)
            {
                buttonText.text = $"Quiz {quizID}: {quizName}";
                buttonText.color = Color.black;
            }
            else
            {
                buttonText.text = $"{statusIcon} Quiz {quizID}: Locked";
                buttonText.color = Color.gray;
            }
        }
        
        if (buttonImage != null)
        {
            if (isCompleted)
            {
                buttonImage.color = new Color(0.5f, 1f, 0.5f, 1f); // Light green
            }
            else if (isUnlocked)
            {
                buttonImage.color = Color.white;
            }
            else
            {
                buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Grayed out
            }
        }
    }
    
    string GetQuizName(int quizID)
    {
        switch (quizID)
        {
            case 1: return "Budgeting Basics";
            case 2: return "Expenses & Bills";
            case 3: return "Savings";
            case 4: return "Credit vs Debt";
            case 5: return "Investing";
            default: return "Unknown Quiz";
        }
    }
    
    void ShowQuizListPanel()
    {
        if (quizListPanel != null) quizListPanel.SetActive(true);
        if (quizPanel != null) quizPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(false);
    }
    
    void StartQuiz(QuizData quiz)
    {
        currentQuiz = quiz;
        currentQuestionIndex = 0;
        correctAnswers = 0;
        
        if (quizListPanel != null) quizListPanel.SetActive(false);
        if (quizPanel != null) quizPanel.SetActive(true);
        if (resultsPanel != null) resultsPanel.SetActive(false);
        
        LoadQuestion();
        
        Debug.Log($"Started quiz: {quiz.quizName}");
    }
    
    void LoadQuestion()
    {
        if (currentQuestionIndex >= currentQuiz.questions.Count)
        {
            ShowResults();
            return;
        }
        
        QuizQuestion question = currentQuiz.questions[currentQuestionIndex];
        
        if (quizQuestionText != null)
        {
            quizQuestionText.text = $"Quiz {currentQuiz.quizID}: {currentQuiz.quizName}\n" +
                                   $"Question {currentQuestionIndex + 1}/{currentQuiz.questions.Count}\n\n" +
                                   $"{question.questionText}";
        }
        
        SetupAnswerButton(quizOption1, question.answers[0], question.correctAnswerIndex == 0);
        SetupAnswerButton(quizOption2, question.answers[1], question.correctAnswerIndex == 1);
        SetupAnswerButton(quizOption3, question.answers[2], question.correctAnswerIndex == 2);
        SetupAnswerButton(quizOption4, question.answers[3], question.correctAnswerIndex == 3);
        
        if (nextQuestionButton != null)
            nextQuestionButton.gameObject.SetActive(false);
            
        if (resultFeedbackText != null)
            resultFeedbackText.gameObject.SetActive(false);
    }
    
    void SetupAnswerButton(Button button, string answerText, bool isCorrect)
    {
        if (button == null) return;
        
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
            buttonText.text = answerText;
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => SelectAnswer(button, isCorrect));
        button.interactable = true;
        
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
            buttonImage.color = Color.black;
    }
    
    void SelectAnswer(Button selectedButton, bool isCorrect)
    {
        // Disable all buttons
        if (quizOption1 != null) quizOption1.interactable = false;
        if (quizOption2 != null) quizOption2.interactable = false;
        if (quizOption3 != null) quizOption3.interactable = false;
        if (quizOption4 != null) quizOption4.interactable = false;
        
        Image buttonImage = selectedButton.GetComponent<Image>();
        
        if (isCorrect)
        {
            if (buttonImage != null)
                buttonImage.color = Color.green;
                
            correctAnswers++;
            
            resultFeedbackText.gameObject.SetActive(true);
            nextQuestionButton.gameObject.SetActive(true);
            resultFeedbackText.text = $"Correct! +{pointsPerCorrectAnswer} points";


            if (pointsManager != null)
            {
                pointsManager.AddPoints(pointsPerCorrectAnswer);
                
                if (resultFeedbackText != null)
                {
                    resultFeedbackText.text = $"Correct! +{pointsPerCorrectAnswer} points";
                    resultFeedbackText.color = Color.green;
                }
            }
            
        }
        else
        {
            resultFeedbackText.gameObject.SetActive(true);
            nextQuestionButton.gameObject.SetActive(true);

            if (buttonImage != null)
                buttonImage.color = Color.red;
                
            if (resultFeedbackText != null)
            {
                resultFeedbackText.text = "Incorrect";
                resultFeedbackText.color = Color.red;
            }
        }
        
        if (resultFeedbackText != null)
            resultFeedbackText.gameObject.SetActive(true);
            
        if (nextQuestionButton != null)
            nextQuestionButton.gameObject.SetActive(true);
    }
    
    void LoadNextQuestion()
    {
        currentQuestionIndex++;
        LoadQuestion();
    }
    
    void ShowResults()
    {
        if (quizPanel != null) quizPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(true);
        
        float percentage = (float)correctAnswers / currentQuiz.questions.Count * 100f;
        int totalPointsEarned = correctAnswers * pointsPerCorrectAnswer;
        
        // Award perfect quiz bonus
        if (correctAnswers == currentQuiz.questions.Count)
        {
            if (pointsManager != null)
            {
                pointsManager.AddPoints(perfectQuizBonus);
                totalPointsEarned += perfectQuizBonus;
            }
        }
        
        // Mark quiz as completed
        quizCompletionStatus[currentQuiz.quizID] = true;
        UpdateButtonStates();
        
        string grade;
        if (percentage >= 90) grade = "A";
        else if (percentage >= 80) grade = "B";
        else if (percentage >= 70) grade = "C";
        else if (percentage >= 60) grade = "D";
        else grade = "F";
        
        if (resultsText != null)
        {
            resultsText.text = $"<b>Quiz {currentQuiz.quizID} Complete!</b>\n\n" +
                             $"<b>{currentQuiz.quizName}</b>\n\n" +
                             $"<b>Score:</b> {correctAnswers}/{currentQuiz.questions.Count} ({percentage:F0}%)\n" +
                             $"<b>Grade:</b> {grade}\n\n" +
                             $"<color=#FFD700><b>Points Earned: {totalPointsEarned}</b></color>";
        }
        
        Debug.Log($"Quiz {currentQuiz.quizID} completed: {correctAnswers}/{currentQuiz.questions.Count} - {totalPointsEarned} points earned");
    }
}