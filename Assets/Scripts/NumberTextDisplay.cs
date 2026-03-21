using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class NumberTextDisplay : MonoBehaviour
{
    public TextMeshProUGUI numberText; // Reference to the TextMeshProUGUI component

    [SerializeField] private string preText;

    [SerializeField] private string postText;
    public float lerpSpeed = 1f; // Speed of lerping
    public bool useLerp = true; // Whether to lerp the number or display instantly

    private float targetValue = 0f; // The value we want to display
    private float currentValue = 0f; // The current displayed value
    private bool valueChanged = false; // Flag to check if the value needs updating
    public Func<float> valueGetter; // 👈 Function that returns the current value

    // Automatically fetch value from bound source
    private void UpdateValueFromGetter()
    {
        if (valueGetter != null)
        {
            SetTargetValue(valueGetter());
        }
    }    
    private void Start()
    {
        if (numberText == null)
        {
            numberText = GetComponent<TextMeshProUGUI>(); // Automatically find the TextMeshProUGUI if not assigned
        }
    }

    private void Update()
    {
        if (useLerp)
        {
            if (Mathf.Abs(currentValue - targetValue) > 0.01f)
            {
                // Smoothly interpolate between currentValue and targetValue
                currentValue = Mathf.Lerp(currentValue, targetValue, lerpSpeed * Time.deltaTime);
                UpdateDisplay();
            }
        }
        else
        {
            // Only update when value changes and lerping is disabled
            if (valueChanged)
            {
                currentValue = targetValue;
                UpdateDisplay();
                valueChanged = false;
            }
        }
    }

    // Method to set the target value (e.g., from another script)
    public void SetTargetValue(float value)
    {
        if (!useLerp || Mathf.Abs(targetValue - value) > 0.01f)
        {
            targetValue = value;
            valueChanged = true;

            if (!useLerp)
            {
                UpdateDisplay(); // Update immediately if not using lerp
            }
        }
    }

    // Optionally, you can expose this method to allow external scripts to update the value
    public void AddToTargetValue(float valueToAdd)
    {
        SetTargetValue(targetValue + valueToAdd);
    }

    // Update the displayed number in TextMeshPro
    private void UpdateDisplay()
    {
        if (postText == "x" &&  Mathf.RoundToInt(currentValue).ToString() == "0")
        {
            numberText.text = "";
            return;
        }
        
        string text = Mathf.RoundToInt(currentValue).ToString();

        if (preText != "")
        {
            text = preText + text;
        }
        if (postText != "")
        {
            text = text + postText;
        }

        numberText.text = text;
    }
}
