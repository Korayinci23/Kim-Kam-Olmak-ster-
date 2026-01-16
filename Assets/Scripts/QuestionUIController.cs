using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

public class QuestionUIController : MonoBehaviour
{
    private QuestionService questionService;
    private QuestionItem currentQuestion;
    
    private bool isLocked = false;

    private Color geciciRenk;
    
    [Header("Question Number")] 
    public int questionNumber;
    public List<Image> questionNumberImages=new List<Image>();
    
    [Header("UI")]
    public TextMeshProUGUI questionText;
    public Button[] optionButtons;
    public TextMeshProUGUI[] optionTexts;

    [Header("Jokers")]
    public Button fiftyFiftyButton;
    public Button doubleAnswerButton;

    // Double Answer state
    private bool doubleAnswerAvailable = true;
    private bool doubleAnswerActive = false;
    private bool firstAnswerWasWrong = false;

    // 50:50 state
    private bool fiftyFiftyAvailable = true;
    private bool fiftyFiftyUsedThisQuestion = false;

    private async void Start()
    {
        IQuestionRepository repo = new FirestoreQuestionRepository();
        questionService = new QuestionService(repo);

        fiftyFiftyButton.interactable = true;
        doubleAnswerButton.interactable = true;

        fiftyFiftyButton.onClick.AddListener(UseFiftyFifty);
        doubleAnswerButton.onClick.AddListener(UseDoubleAnswer);

        await LoadNextQuestion();
    }
    

    private async Task LoadNextQuestion()
    {
        isLocked = false;
        fiftyFiftyUsedThisQuestion = false;

        currentQuestion = await questionService.GetNextQuestion();

        if (currentQuestion == null)
        {
            Debug.Log("❌ Soru kalmadı");
            return;
        }
        
        ShowQuestion(currentQuestion);

        doubleAnswerActive = false;
        firstAnswerWasWrong = false;
        doubleAnswerButton.interactable = doubleAnswerAvailable;

        fiftyFiftyButton.interactable = fiftyFiftyAvailable;
    }

    private void ShowQuestion(QuestionItem item)
    {
        QuestionData data = item.Data;
        questionText.text = data.question;
        geciciRenk = questionNumberImages[questionNumber].color;
        geciciRenk.a = 1f; 
        questionNumberImages[questionNumber].color = geciciRenk;

        List<string> options = new List<string>(data.options);
        Shuffle(options);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionButtons[i].gameObject.SetActive(true);
            optionButtons[i].interactable = true;
            optionButtons[i].image.color = Color.white; // Rengi sıfırla
            optionButtons[i].onClick.RemoveAllListeners();

            optionTexts[i].text = options[i];
            
            // DEĞİŞEN KISIM BURASI
            string answerText = options[i];
            Button myButton = optionButtons[i]; // O anki butonu bir değişkene alıyoruz

            // Tıklanınca hem butonu (myButton) hem yazıyı (answerText) gönderiyoruz
            optionButtons[i].onClick.AddListener(() =>
            {
                if (isLocked) return;
                OnOptionSelected(myButton, answerText);
            });
        }
    }

    private async void OnOptionSelected(Button clickedButton,string selected)
    {
        if (isLocked) return;

        
        isLocked = true;
        Color sariRenk;
        ColorUtility.TryParseHtmlString("#FFF578", out sariRenk);
        clickedButton.image.color = sariRenk;
        await Task.Delay(3000);
        
        bool correct = selected == currentQuestion.Data.correctAnswer;

        // ✅ DOĞRU
        if (correct)
        {
            isLocked = true;
            questionService.RegisterAnswer(true);

            Debug.Log("✅ DOĞRU");

            DisableAllOptions();
            await Task.Delay(1200);
            questionNumber++;
            
            await LoadNextQuestion();
            return;
        }

        // ❌ YANLIŞ
        Debug.Log("❌ YANLIŞ");

        // 🃏 Double Answer AKTİFSE ve ilk yanlışsa
        if (doubleAnswerActive && !firstAnswerWasWrong)
        {
            firstAnswerWasWrong = true;

            Debug.Log("🃏 İkinci cevap hakkı verildi");

            // Yanlış seçilen şıkkı kapat
            DisableSelectedOption(selected);

            return; // SORU DEVAM EDİYOR
        }
        
        isLocked = true;
        questionService.RegisterAnswer(false);

        geciciRenk.a = 0.2f; 
        questionNumberImages[questionNumber-1].color = geciciRenk;
        DisableAllOptions();
        
        await Task.Delay(200);
        await LoadNextQuestion();
    }

    
    public void UseFiftyFifty()
{
    if (!fiftyFiftyAvailable || fiftyFiftyUsedThisQuestion)
        return;

    fiftyFiftyUsedThisQuestion = true;
    fiftyFiftyAvailable = false;
    fiftyFiftyButton.interactable = false;

    List<int> wrongIndexes = new List<int>();

    for (int i = 0; i < optionTexts.Length; i++)
    {
        if (optionTexts[i].text != currentQuestion.Data.correctAnswer)
            wrongIndexes.Add(i);
    }

    Shuffle(wrongIndexes);

    for (int i = 0; i < 2; i++)
    {
        int index = wrongIndexes[i];

        // ❌ Tıklanamaz yap
        optionButtons[index].interactable = false;
    }
}

    public void UseDoubleAnswer()
{
    if (!doubleAnswerAvailable)
        return;

    doubleAnswerAvailable = false;
    doubleAnswerActive = true;
    doubleAnswerButton.interactable = false;

    Debug.Log("🃏 Çift cevap jokeri aktif");
}

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }
    
    private void DisableAllOptions()
    {
        foreach (var btn in optionButtons)
            btn.interactable = false;
    }

    private void DisableSelectedOption(string selected)
    {
        for (int i = 0; i < optionTexts.Length; i++)
        {
            if (optionTexts[i].text == selected)
            {
                optionButtons[i].interactable = false;

                var colors = optionButtons[i].colors;
                colors.disabledColor = Color.gray;
                optionButtons[i].colors = colors;
                break;
            }
        }
    }


}
