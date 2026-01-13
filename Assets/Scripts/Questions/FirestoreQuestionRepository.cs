using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FirestoreQuestionRepository : IQuestionRepository
{
    private readonly FirebaseFirestore db;

    public FirestoreQuestionRepository()
    {
        db = FirebaseFirestore.DefaultInstance;
    }

    public async Task<List<QuestionItem>> GetAvailableQuestionsByDifficulty(int difficulty)
    {
        Query query = db.Collection("questions")
            .WhereEqualTo("difficulty", difficulty)
            .WhereEqualTo("isAvailable", true);

        QuerySnapshot snapshot = await query.GetSnapshotAsync();

        List<QuestionItem> result = new List<QuestionItem>();

        foreach (DocumentSnapshot doc in snapshot.Documents)
        {
            QuestionData data = doc.ConvertTo<QuestionData>();

            result.Add(new QuestionItem
            {
                Id = doc.Id,
                Data = data
            });
        }

        return result;
    }

    public async Task SetQuestionAvailability(string questionId, bool isAvailable)
    {
        await db.Collection("questions")
            .Document(questionId)
            .UpdateAsync("isAvailable", isAvailable);
    }

    public async Task ResetAvailabilityByDifficulty(int difficulty)
    {
        Query query = db.Collection("questions")
            .WhereEqualTo("difficulty", difficulty)
            .WhereEqualTo("isAvailable", false);

        QuerySnapshot snapshot = await query.GetSnapshotAsync();

        foreach (var doc in snapshot.Documents)
        {
            await doc.Reference.UpdateAsync("isAvailable", true);
        }
    }
}
