using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class QuizQuestion
{
    public string questionText;
    public string[] answers; // Always 4 options (A, B, C, D)
    public int correctAnswerIndex; // 0-3 (which answer is correct)
}



[System.Serializable]
public class QuizData
{
    public int quizID; // Unique ID for each quiz
    public string quizName;
    public string description;
    public List<QuizQuestion> questions;
    public bool isUnlocked;
    public bool isCompleted;  
}

public enum UnlockRequirement
{
    DaysPassed,           // Unlock after X days
    BalanceThreshold,     // Unlock when player has $X
    PaidRent,             // Unlock after paying rent
    CompletedScenarios,   // Unlock after completing X scenarios
    Immediate             // Unlocks right away
}


