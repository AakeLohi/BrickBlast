using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] private Image bar;  // Reference to the UI image that will act as the timer bar

    public float timerTime = 30f;  // Total time for the countdown (in seconds)
    public float progressSpeed = 1f;  // Speed at which the timer decreases (typically 1.0f)

    public UnityEvent onTimerEnd;  // Event to trigger when the timer ends

    private float currentProgress;  // Current progress of the timer (from 0 to 1)

    private bool isRunning = false;  // To check if the timer is running

    public Gradient colorGradient;

    public AudioSource audioSource;

    private float tickInterval;  // Time interval between ticks
    private float tickTimer;    // Timer to track elapsed time for ticks

    public void TimerStart()
    {
        currentProgress = 1f;  // Start the timer from full progress
        tickInterval = 1f;  // Start with a fast tick interval
        tickTimer = 0f;  // Initialize the tick timer
        isRunning = true;      // Set the timer running state
    }

    public void TimerResetProgress()
    {
        currentProgress = 1f;
        bar.fillAmount = 1f;
        progressSpeed *= 1.05f;
        tickInterval = 1f;  // Reset tick interval as progress speed increases
        tickTimer = 0f;  // Reset tick timer
        isRunning = true;  // Restart the timer
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            // Decrease the current progress over time
            if (currentProgress > 0f)
            {
                currentProgress -= progressSpeed * Time.deltaTime / timerTime;  // Decrease progress
                bar.fillAmount = currentProgress;  // Update the bar's fill amount based on the progress
                bar.color = colorGradient.Evaluate(currentProgress);

                // Adjust tick interval to make it shorter as time progresses
                tickInterval = 1f / progressSpeed;  // Make the interval shorter as the timer goes down
                
                // Update the tick timer
                tickTimer += Time.deltaTime;

                // Check if it's time to play the tick sound
                if (tickTimer >= tickInterval)  
                {
                    if (audioSource != null)
                    {
                        if (audioSource.isPlaying)  // If it's already playing, stop it
                        {
                            audioSource.Stop();
                        }
                        audioSource.Play();  // Play the tick sound from the beginning
                    }

                    // Reset the tick timer after playing the sound
                    tickTimer = 0f;
                }
            }
            else
            {
                // Once the progress reaches 0, trigger the onTimerEnd event
                if (onTimerEnd != null)
                {
                    onTimerEnd.Invoke();
                }
                isRunning = false;  // Stop the timer from updating further
                currentProgress = 0f;  // Ensure the progress is 0
                bar.fillAmount = 0f;  // Ensure the bar is empty
            }
        }
    }

    // Optional: Method to reset the timer
    public void ResetTimer()
    {
        currentProgress = 1f;
        bar.fillAmount = 1f;
        progressSpeed = 1f;
        isRunning = false;  // Stop the timer if it was running
    }
}
