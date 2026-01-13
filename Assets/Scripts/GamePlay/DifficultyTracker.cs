using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyTracker 
{
    public int CurrentDifficulty { get; private set; } = 2;

    private int correctStreak = 0;
    private int wrongStreak = 0;

    public void RegisterCorrect()
    {
        correctStreak++;
        wrongStreak = 0;

        if (correctStreak >= 3)
        {
            CurrentDifficulty++;
            correctStreak = 0;
        }
    }

    public void RegisterWrong()
    {
        wrongStreak++;
        correctStreak = 0;

        if (wrongStreak >= 3 && CurrentDifficulty > 1)
        {
            CurrentDifficulty--;
            wrongStreak = 0;
        }
    }
}
