using System.Collections.Generic;
using System.Threading.Tasks;

public interface IQuestionRepository
{
    Task<List<QuestionItem>> GetAvailableQuestionsByDifficulty(int difficulty);
    Task SetQuestionAvailability(string questionId, bool isAvailable);
    Task ResetAvailabilityByDifficulty(int difficulty);
}
