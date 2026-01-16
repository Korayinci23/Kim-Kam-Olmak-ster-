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
    
    [Header("Audio")]
    public AudioSource sfxSource; 
    public AudioClip questionLoadSound;
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip winSound;         
    public AudioClip loseSound;

    [Header("Win&Lose")] 
    public GameObject winScene;

    public GameObject loseScene;
    
    [Header("Timer")]
    public TextMeshProUGUI timerText;
    private float timeLeft=30f;
    private bool isTimerRunning = false;
    
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
    private void Update()
    {
        if (isTimerRunning)
        {
            timeLeft -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(timeLeft).ToString();

            
            if (timeLeft <= 0)
            {
                isTimerRunning = false;
                timeLeft = 0;
                timerText.text = "0";
                OnTimeExpired();
            }
        }
    }

    private async Task LoadNextQuestion()
    {
        foreach (var optionButton in optionButtons)
        {
            Color beyazRenk;
            ColorUtility.TryParseHtmlString("#FFFFFF", out beyazRenk);
            optionButton.image.color = beyazRenk;
        }
        if (questionNumber == 0)
        {
            foreach (var img in questionNumberImages)
            {
                Color c = img.color;
                c.a = 0.2f; 
                img.color = c;
            }
        }
        
        isLocked = false;
        fiftyFiftyUsedThisQuestion = false;

        currentQuestion = await questionService.GetNextQuestion();

        if (currentQuestion == null)
        {
            Debug.Log("❌ Soru kalmadı");
            return;
        }
        
        ShowQuestion(currentQuestion);

        if (sfxSource != null && questionLoadSound != null)
        {
            sfxSource.PlayOneShot(questionLoadSound);
        }
        
        doubleAnswerActive = false;
        firstAnswerWasWrong = false;
        doubleAnswerButton.interactable = doubleAnswerAvailable;

        fiftyFiftyButton.interactable = fiftyFiftyAvailable;
        if (questionNumber < 7) 
        {
            timeLeft = 30f;
            isTimerRunning = true;
            timerText.gameObject.SetActive(true);
        }
        else
        {
            isTimerRunning = false;
            timerText.gameObject.SetActive(false);
        }
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
            optionButtons[i].image.color = Color.white; 
            optionButtons[i].onClick.RemoveAllListeners();

            optionTexts[i].text = options[i];
            
            string answerText = options[i];
            Button myButton = optionButtons[i];
            optionButtons[i].onClick.AddListener(() =>
            {
                if (isLocked) return;
                OnOptionSelected(myButton, answerText);
            });
        }
    }

    private async void OnTimeExpired()
    {
        Debug.Log("⏰ SÜRE DOLDU!");
        isLocked = true;
        Lose();
       
    }
    
    private async void OnOptionSelected(Button clickedButton,string selected)
    {
        if (isLocked) return;

        
        isLocked = true;
        isTimerRunning = false;
        Color sariRenk;
        ColorUtility.TryParseHtmlString("#FFF578", out sariRenk);
        clickedButton.image.color = sariRenk;
        await Task.Delay(1500);
        
        bool correct = selected == currentQuestion.Data.correctAnswer;
       
        if (correct)
        {
            if(sfxSource != null && correctSound != null)
                sfxSource.PlayOneShot(correctSound);
           
            Color yesilRenk;
            ColorUtility.TryParseHtmlString("#CCFF78", out yesilRenk);
            clickedButton.image.color = yesilRenk;
            
            await Task.Delay(2000); 
            isLocked = true;
            questionService.RegisterAnswer(true);
            Debug.Log("✅ DOĞRU");

            DisableAllOptions();
            await Task.Delay(1200);
            questionNumber++;
            if (questionNumber == 15)
            {
                if (sfxSource != null && winSound != null)
                {
                    sfxSource.PlayOneShot(winSound);
                }
                Win(true);
                await Task.Delay(3000);
                Win(false);
                
                return;
            }
            geciciRenk.a = 0.5f; 
            questionNumberImages[questionNumber-1].color = geciciRenk;
            await LoadNextQuestion();
            return;
        }

        
        Debug.Log("❌ YANLIŞ");
        Color kirmiziRenk;
        ColorUtility.TryParseHtmlString("#FF5851", out kirmiziRenk);
        clickedButton.image.color = kirmiziRenk;
        if(sfxSource != null && wrongSound != null)
            sfxSource.PlayOneShot(wrongSound);
        
        await Task.Delay(2500);
      
        if (doubleAnswerActive && !firstAnswerWasWrong)
        {
            firstAnswerWasWrong = true;
            
            DisableSelectedOption(selected);
            isLocked = false; 

            return;
        }
        for (int i = 0; i < optionTexts.Length; i++)
        {
            
            if (optionTexts[i].text == currentQuestion.Data.correctAnswer)
            {
                Color yesilRenk;
                ColorUtility.TryParseHtmlString("#CCFF78", out yesilRenk);
                optionButtons[i].image.color = yesilRenk;
                break;
            }
        }
        await Task.Delay(2000);
        Lose();
        
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

    private void Win(bool isActive)
    {
        winScene.SetActive(isActive);
    }

    private async void Lose()
    {
        // 🔊 KAYBETME SESİ
        if (sfxSource != null && loseSound != null)
        {
            sfxSource.PlayOneShot(loseSound);
        }
        questionNumber=0;
        isLocked = true;
        questionService.RegisterAnswer(false);

       
        DisableAllOptions();
        loseScene.SetActive(true);
        await Task.Delay(2000);
        loseScene.SetActive(false);
        await Task.Delay(200);
        await LoadNextQuestion();
        
    }
    
}
