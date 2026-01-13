using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;

[FirestoreData]
public class QuestionData
{
    [FirestoreProperty]
    public int difficulty { get; set; }

    [FirestoreProperty]
    public string question { get; set; }

    [FirestoreProperty]
    public List<string> options { get; set; }

    [FirestoreProperty]
    public string correctAnswer { get; set; }

    [FirestoreProperty]
    public bool isAvailable { get; set; }

    [FirestoreProperty]
    public string questionType { get; set; }
    [FirestoreProperty]
    public Timestamp createdAt { get; set; }
}



