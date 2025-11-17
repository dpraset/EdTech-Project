using System.Collections.Generic;
using UnityEngine;

public class Quiz5_Investing : MonoBehaviour
{
    public static QuizData GetQuizData()
    {
        QuizData quiz = new QuizData
        {
            quizID = 5,
            quizName = "Investment Basics",
            description = "Learn how to manage your money and build a strong financial foundation!",
            isUnlocked = false,
            isCompleted = false,
            questions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    questionText = "What is a budget?",
                    answers = new[] {"A plan for spending money", "A type of credit score", "A debt payment", "A loan agreement"},
                    correctAnswerIndex = 0
                },
                new QuizQuestion
                {
                    questionText = "Which of these is considered a 'fixed' expense?",
                    answers = new[] {"Groceries", "Entertainment", "Rent", "Gasoline"},
                    correctAnswerIndex = 2
                },
                new QuizQuestion
                {
                    questionText = "What does the 50/30/20 rule recommend you do with 20% of your income?",
                    answers = new[] { "Spend on fun", "Pay for needs", "Save or invest", "Use for emergencies" },
                    correctAnswerIndex = 2
                },
                new QuizQuestion
                {
                    questionText = "If you earn $3000 per month, what’s the recommended amount to save/invest using the 50/30/20 rule?",
                    answers = new[] { "$600", "$900", "$1500", "$300" },
                    correctAnswerIndex = 0
                },
                new QuizQuestion
                {
                    questionText = "Why is tracking your expenses important?",
                    answers = new[] { "To avoid taxes", "To see where your money goes", "To borrow more money", "To check your credit score" },
                    correctAnswerIndex = 1
                }
            }
        };

        return quiz;
    }

    public static UnlockRequirement GetUnlockRequirement()
    {
        return UnlockRequirement.DaysPassed;
    }

    public static int GetBalanceRequired()
    {
        return 10000; // Unlock after having $10,000
    }
}
