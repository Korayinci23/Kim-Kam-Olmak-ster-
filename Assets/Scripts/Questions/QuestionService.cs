using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public class QuestionService
{
    private readonly IQuestionRepository repository;

    private int currentDifficulty = 1;
    private const int MinDifficulty = 1;
    private const int MaxDifficulty = 3;

    private int correctStreak = 0;
    private int wrongStreak = 0;

    public QuestionService(IQuestionRepository repo, int startDifficulty = 1)
    {
        repository = repo;
        currentDifficulty = startDifficulty;
    }

    public async Task<QuestionItem> GetNextQuestion()
    {
        var questions = await repository.GetAvailableQuestionsByDifficulty(currentDifficulty);

        if (questions.Count == 0)
        {
            await repository.ResetAvailabilityByDifficulty(currentDifficulty);
            questions = await repository.GetAvailableQuestionsByDifficulty(currentDifficulty);
        }

        if (questions.Count == 0)
            return null;

        QuestionItem selected = questions[Random.Range(0, questions.Count)];

        // 🔥 soru sorulduğu anda isAvailable = false
        await repository.SetQuestionAvailability(selected.Id, false);

        Debug.Log("📊 Mevcut Zorluk Seviyesi: " + currentDifficulty);

        return selected;
    }

    public void RegisterAnswer(bool isCorrect)
    {
        if (isCorrect)
        {
            correctStreak++;
            wrongStreak = 0;

            if (correctStreak >= 3)
            {
                IncreaseDifficulty();
                correctStreak = 0;
            }
        }
        else
        {
            wrongStreak++;
            correctStreak = 0;

            if (wrongStreak >= 3)
            {
                DecreaseDifficulty();
                wrongStreak = 0;
            }
        }
    }

    private void IncreaseDifficulty()
    {
        currentDifficulty = Mathf.Min(currentDifficulty + 1, MaxDifficulty);
        Debug.Log("⬆ Zorluk arttı → " + currentDifficulty);
    }

    private void DecreaseDifficulty()
    {
        currentDifficulty = Mathf.Max(currentDifficulty - 1, MinDifficulty);
        Debug.Log("⬇ Zorluk düştü → " + currentDifficulty);
    }
}
