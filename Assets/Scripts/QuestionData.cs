using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionData : MonoBehaviour
{
    public string correctAnswer;
    public string[] answers;
    public string question;

    public int diffucultyLevel;
    public int questionID;

    public bool isAvailable;
    
    public QuestionType questionType;
}

public enum QuestionType
{
    image,
    text
    
}

public class Questions : Monobehaviour
{
    public string[] questions;
}