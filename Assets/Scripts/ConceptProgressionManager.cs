using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConceptProgressionManager : MonoBehaviour
{
    public GameManager gameManager;
    public EducationManager educationManager;
    public UIManager uiManager;
    
    // Track which quizzes have been unlocked
    private List<int> unlockedQuizIDs = new List<int>();
    
    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
            
        if (educationManager == null)
            educationManager = FindObjectOfType<EducationManager>();
            
        if (uiManager == null)
            uiManager = FindObjectOfType<UIManager>();
    }
    
    // Called by GameManager each day
    public void CheckUnlocks(int currentDay)
    {
        CheckQuiz1_Budgeting(currentDay);
        CheckQuiz2_Expenses(currentDay);
        CheckQuiz3_EmergencyFunds();
        CheckQuiz4_CreditDebt(currentDay);
        CheckQuiz5_Investing();
    }
    
    void CheckQuiz1_Budgeting(int currentDay)
    {
        // Quiz 1: Budgeting - Unlocks after 3 days

        // Already unlocked
        if (unlockedQuizIDs.Contains(1))
        {
            Debug.Log("Quiz Unlocked");
            return;
        }        
        
        if (currentDay >= Quiz1_Budgeting.GetDaysRequired())
        {
            UnlockQuiz(1, "Budgeting Basics");
            Debug.Log("Unlocking Quiz");
        }

        else
        {
            Debug.Log($"Quiz not unlocked. Need day {Quiz1_Budgeting.GetDaysRequired()}, currently on day {currentDay}");
        }
    }
    
    void CheckQuiz2_Expenses(int currentDay)
    {
        // Quiz 2: Expenses - Unlocks after paying rent once
        if (unlockedQuizIDs.Contains(2)) return;
        
        if (currentDay >= Quiz2_Expenses.GetDaysRequired())
        {
            UnlockQuiz(2, "Expenses & Bills");
        }
    }
    
    void CheckQuiz3_EmergencyFunds()
    {
        // Quiz 3: Emergency Funds - Unlocks when balance >= $500
        if (unlockedQuizIDs.Contains(3)) return;
        
        if (gameManager.playerBalance >= Quiz3_EmergencyFunds.GetBalanceRequired())
        {
            UnlockQuiz(3, "Income");
        }
    }
    
    void CheckQuiz4_CreditDebt(int currentDay)
    {
        // Quiz 4: Credit & Debt - Unlocks after 14 days
        if (unlockedQuizIDs.Contains(4)) return;
        
        if (currentDay >= Quiz4_CreditDebt.GetDaysRequired())
        {
            UnlockQuiz(4, "Credit & Debt");
        }
    }
    
    void CheckQuiz5_Investing()
    {
        // Quiz 5: Investing - Unlocks when balance >= $1000
        if (unlockedQuizIDs.Contains(5)) return;
        
        if (gameManager.playerBalance >= Quiz5_Investing.GetBalanceRequired())
        {
            UnlockQuiz(5, "Saving & Investing");
        }
    }
    
    void UnlockQuiz(int quizID, string quizName)
    {
        unlockedQuizIDs.Add(quizID);
        
        // Tell EducationManager to unlock this quiz
        if (educationManager != null)
        {
            educationManager.UnlockQuiz(quizID);
        }
        
        // Show notification to player
        if (uiManager != null)
        {
            uiManager.ShowFeedback($"New Quiz Unlocked!\n Quiz {quizID}: {quizName} Available in Education menu!");
        }
        
        Debug.Log($"Quiz {quizID} unlocked: {quizName}");
    }
    
    bool HasPaidRent()
    {
        if (gameManager.financeManager == null) return false;
        
        foreach (var expense in gameManager.financeManager.expenseHistory)
        {
            if (expense.expenseType == "Rent" && expense.amount > 0)
                return true;
        }
        return false;
    }
}