using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonRead : MonoBehaviour
{
    private TextAsset questionJsonFile;

    public void Start()
    {
        ReadJson("QuestionsDL4");
    }
    public void ReadJson(string jsonName)
    {
        
        questionJsonFile=LoadResourceTextfile(jsonName);
        if (questionJsonFile == null)
        {
            Debug.LogError("JSON dosyası bulunamadı!");
            return;
        }
        Questions questionsInJson=JsonUtility.FromJson<Questions>(questionJsonFile.text);
        Debug.LogError(questionsInJson);
    }
    
    private TextAsset LoadResourceTextfile(string path)
    {
        string filePath = "Scripts/Jsons/" + path.Replace(".json", "");
        Debug.Log(filePath);
        return Resources.Load<TextAsset>(filePath);
    }
}
