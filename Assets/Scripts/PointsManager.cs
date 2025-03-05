using System.IO;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;  // Required for RTL TextMeshPro
using ArabicSupport;  

public class PointsManager : MonoBehaviour
{
    public static int score = 0; // Static so all objects share the same score
    public Text scoreText; // Assign this in the Unity Inspector
    public Text descriptionText; 
    static int counter = 1;
    public Text Title;
    public DragAndDrop[] dragAndDropObjects; // Array of DragAndDrop objects in the scene
    public float timeLimit = 60f; // Time limit in seconds (e.g., 60 seconds)
    private float timeRemaining;
    public Text timerText; // Assign a Text element for the timer in Unity Inspector

    // Audio variables
    public AudioSource audioSource; // Assign in Inspector
    public AudioClip goodgobAudioSource; 
    public AudioClip tryAgainAudioSource;
    public AudioClip startAudioClip; // Assign the "هيا ابدا" audio clip in Inspector
    private bool hasPlayedAudio = false; // To ensure audio plays only once

    private void Start()
    {
        timeRemaining = timeLimit; // Initialize the timer
        scoreText.text = "" + ConvertToArabicNumerals(score);
        descriptionText.text = ArabicFixer.Fix(" ترتيب الهرم الغذائي ضع الطعام في مكانه الصحيح حسب ");
        Title.text = ArabicFixer.Fix("الهرم الغذائي");

        // Find all DragAndDrop objects in the scene
        dragAndDropObjects = FindObjectsOfType<DragAndDrop>();

        // Ensure AudioSource is assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // Update the timer
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime; // Decrease time over time
            UpdateTimerDisplay(); // Update the timer UI
            no_interaction();
        }
        else
        {
            descriptionText.text = ArabicFixer.Fix("انتهى الوقت !");
            // Optionally, you could stop scoring or handle timeout behavior here
        }
    }

    // Update the timer UI text in Arabic numerals
    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        string formattedTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        timerText.text = ConvertToArabicNumeralsTime(formattedTime); // Convert the timer to Arabic numerals
    }

    // Function to convert time (mm:ss) to Arabic-Indic numerals
    private string ConvertToArabicNumeralsTime(string time)
    {
        string arabicTime = "";
        
        foreach (char c in time)
        {
            if (char.IsDigit(c))
            {
                arabicTime += (char)(c - '0' + '٠'); // Convert digits to Arabic-Indic numerals
            }
            else
            {
                arabicTime += c; // Keep the colon (:) as is
            }
        }

        return arabicTime;
    }

    public void AddPoints(int points)
    {
        if (points > 0)
        {
            if (counter <= 7)
            {
                score += 3;
                counter++;
            }
            else
            {
                score += 2;
                counter++;
            }
            scoreText.text = "" + ConvertToArabicNumerals(score);
            if (score == 35)
            {
                descriptionText.text = ArabicFixer.Fix("أحسنت! لقد أتممت اللعبة بنجاح");
            }
            else
            {
                descriptionText.text = ArabicFixer.Fix("أحسنت! واصل التقدم");
                audioSource.PlayOneShot(goodgobAudioSource);
            }
        }
    }

    public void GetHint(string name)
    {
        switch (name.ToLower())  // Use ToLower() to make it case-insensitive
        {
            case "apple":
                descriptionText.text = ArabicFixer.Fix(" هل يمكنك وضعها في المكان الصحيح؟ التفاحة تنتمي إلى مجموعة الفواكه.");
                break;

            case "banana":
                descriptionText.text = ArabicFixer.Fix("   بإمكانك وضعه في المكان الصحيح؟ الموز ينتمي إلى مجموعة الفواكه. هل");
                break;

            case "brocoli":
                descriptionText.text = ArabicFixer.Fix("  هل يمكنك وضعه في المكان الصحيح؟ البروكلي ينتمي إلى مجموعة الخضروات.");
                break;

            case "carrot":
                descriptionText.text = ArabicFixer.Fix("  بإمكانك وضعه في المكان الصحيح؟ الجزر ينتمي إلى مجموعة الخضروات. هل");
                break;

            case "tomatoes":
                descriptionText.text = ArabicFixer.Fix(" هل يمكنك وضعها في المكان الصحيح؟ الطماطم ينتمي إلى مجموعة الخضروات.");
                break;

            case "candy":
                descriptionText.text = ArabicFixer.Fix(" هل يمكنك وضعها في المكان الصحيح؟ الحلوى من الأغذية الغنية بالسكر.");
                break;

            case "liquid milk":
                descriptionText.text = ArabicFixer.Fix(" هل يمكنك وضعه في المكان الصحيح؟ الحليب ينتمي إلى مجموعة الألبان.");
                break;

            case "cheese":
                descriptionText.text = ArabicFixer.Fix(" يمكنك وضعه في المكان الصحيح؟ الجبن ينتمي إلى مجموعة الألبان هل ");
                break;

            case "rice":
                descriptionText.text = ArabicFixer.Fix("  يمكنك وضعه في المكان الصحيح؟ الأرز ينتمي إلى مجموعة الحبوب. هل");
                break;

            case "bread":
                descriptionText.text = ArabicFixer.Fix("  يمكنك وضعه في المكان الصحيح؟ الخبز ينتمي إلى مجموعة الحبوب. هل");
                break;

            case "chicken":
                descriptionText.text = ArabicFixer.Fix(" هل يمكنك وضعه في المكان الصحيح؟  الدجاج ينتمي إلى مجموعة البروتينات.");
                break;

            case "fish":
                descriptionText.text = ArabicFixer.Fix(" هل يمكنك وضعه في المكان الصحيح؟ السمك ينتمي إلى مجموعة البروتينات.");
                break;

            case "burger":
                descriptionText.text = ArabicFixer.Fix(" هل يمكنك وضعه في المكان الصحيح؟ البرجر ينتمي إلى مجموعة البروتينات.");
                break;

            default:
                descriptionText.text = ArabicFixer.Fix(" ترتيب الهرم الغذائي ضع الطعام في مكانه الصحيح حسب");
                break;
        }
        audioSource.PlayOneShot(tryAgainAudioSource);
    }

    private string ConvertToArabicNumerals(int number)
    {
        string arabicNumerals = "";
        string numberStr = number.ToString();

        foreach (char c in numberStr)
        {
            // Convert each digit to Arabic-Indic numerals (٠ to ٩)
            arabicNumerals += (char)(c - '0' + '٠');
        }

        return arabicNumerals;
    }

    private void no_interaction()
    {
        float timeLeft = timeLimit - timeRemaining;
        bool anyObjectStarted = false;

        // Check if any DragAndDrop object has started playing
        foreach (var dragObject in dragAndDropObjects)
        {
            if (dragObject.startPlay)
            {
                anyObjectStarted = true;
                break;
            }
        }

        if (!anyObjectStarted && timeLeft >= 10 && !hasPlayedAudio)
        {
            // Play the audio clip
            if (audioSource != null && startAudioClip != null)
            {
                audioSource.PlayOneShot(startAudioClip);
                hasPlayedAudio = true; // Ensure audio plays only once
            }
        }
    }
}