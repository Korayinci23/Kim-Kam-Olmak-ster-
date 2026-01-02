using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;

[FirestoreData]
public class QuestionData
{
    [FirestoreProperty("correctAnswer")]
    public string correctAnswer { get; set; }

    [FirestoreProperty("options")] // Firestore'daki "options" listesi buraya gelecek
    public List<string> answers { get; set; }

    [FirestoreProperty("question")]
    public string question { get; set; }

    [FirestoreProperty("difficulty")]
    public int difficultyLevel { get; set; }

    [FirestoreProperty("isAvailable")]
    public bool isAvailable { get; set; }

    [FirestoreProperty("questionType")]
    public string questionTypeString { get; set; }
}

public enum QuestionType
{
    image,
    text
    
}

