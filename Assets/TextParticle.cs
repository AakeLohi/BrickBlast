using UnityEngine;
using TMPro;

public class TextParticle : MonoBehaviour
{
    public TextMeshPro text;
    public float fadeDuration = 1f;  // Duration over which to fade the text
    public float raiseSpeed = 1f;    // Speed at which the text rises

    private float elapsedTime = 0f;
    private Color initialColor;
    private bool isActive = false;

    // Call this method to initialize and activate the text particle
    public void Initialize(string message, float duration)
    {
        text.text = message;
        fadeDuration = duration;

        // Initialize text color and alpha
        initialColor = text.color;
        initialColor.a = 1f;  // Set initial alpha to 1 (fully opaque)
        text.color = initialColor;

        // Activate the particle
        isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            // Update movement and fading
            elapsedTime += Time.deltaTime;

            // Move the text upwards
            transform.position += Vector3.up * raiseSpeed * Time.deltaTime;

            // Fade the text
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            text.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

            // If the particle has completed its fade duration, deactivate it
            if (elapsedTime >= fadeDuration)
            {
                Deactivate();
            }
        }
    }

    // Deactivate the particle and reset the values
    private void Deactivate()
    {
        isActive = false;
        text.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
        // Reuse the object for the next particle or destroy it if not using object pooling
        Destroy(gameObject);  // Deactivate the GameObject (if using pooling, don't destroy it)
    }
}
