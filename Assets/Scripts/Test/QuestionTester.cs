using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionTester : MonoBehaviour
{
    private QuestionService questionService;

    async void Start()
    {
        questionService = new QuestionService(new FirestoreQuestionRepository());

        QuestionItem q = await questionService.GetNextQuestion();

        Debug.Log(q.Data.question);
        Debug.Log("Correct Answer: " + q.Data.correctAnswer);
    }
}
