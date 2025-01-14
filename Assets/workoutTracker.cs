using UnityEngine;

public class workoutTracker : MonoBehaviour
{

    // Variables to track the child’s movement
    public Transform trackedBodyPart; // The body part to track (e.g., hand or leg)
    public Vector3 correctPosition;   // The correct position for the workout
    public float tolerance = 0.1f;    // Allowable error margin

    // Feedback UI or sound
    public GameObject feedbackUI;     // UI element for feedback
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize components
        if (feedbackUI != null)
        {
            feedbackUI.SetActive(false);
        }
        audioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        // Track the body part's position
        if (trackedBodyPart != null)
        {
            CheckWorkout();
        }
    }

    void CheckWorkout()
    {
        // Calculate the distance between the tracked body part and the correct position
        float distance = Vector3.Distance(trackedBodyPart.position, correctPosition);

        if (distance <= tolerance)
        {
            // Correct workout
            ProvideFeedback(true);
        }
        else
        {
            // Incorrect workout
            ProvideFeedback(false);
        }
    }

    void ProvideFeedback(bool isCorrect)
    {
        if (feedbackUI != null)
        {
            feedbackUI.SetActive(true);
            feedbackUI.GetComponent<UnityEngine.UI.Text>().text = isCorrect ? "Good Job!" : "Try Again!";
        }

        if (audioSource != null)
        {
            audioSource.PlayOneShot(isCorrect ? correctSound : incorrectSound);
        }
    }
}  
    


