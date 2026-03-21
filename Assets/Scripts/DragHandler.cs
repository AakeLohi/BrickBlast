using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Vector3 offset;
    private Camera mainCamera;
    private Vector3 snappedPosition;

    [SerializeField]
    private float scaleSmoothness = 15f; // Smoothness factor for scaling during drag

    [SerializeField]
    private float moveSmoothness = 15f; // Smoothness factor for moving the object during drag

    private bool isDragging = false; // To track dragging state

    void Start()
    {
        // Cache the camera reference
        mainCamera = Camera.main;
                // Store the original position and scale of the dragged object
        originalPosition = transform.position;
    }

public void OnBeginDrag(PointerEventData eventData)
{
    // Only perform actions when beginning to drag
    if (!isDragging)
    {
        isDragging = true;

        originalScale = new Vector3(1, 1, 1);

        // Calculate the inverse parent scale to counteract the parent's scaling
        Vector3 inverseParentScale = new Vector3(
            1f / transform.parent.localScale.x,
            1f / transform.parent.localScale.y,
            1f / transform.parent.localScale.z
        );

        // Start the coroutine to scale the object smoothly to counteract parent's scale
        StartCoroutine(ScaleObject(Vector3.Scale(transform.localScale, inverseParentScale), scaleSmoothness));

        // Calculate the offset between the object and where the pointer starts
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(eventData.position);
        worldPosition.z = -3f;  // Set z position for 2D interaction
        offset = transform.position - worldPosition;  // Store the offset in world space
    }
}

public void OnDrag(PointerEventData eventData)
{
    if (isDragging)
    {
        // Update the object's position based on the current pointer position
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(eventData.position);
        worldPosition.z = -3f;  // Set z position for 2D interaction

        // Calculate the distance between the current world position and the original position
        Vector3 distance = worldPosition - originalPosition;

        // Move the object with an added multiplier based on the distance
        transform.position = worldPosition + offset + distance * (GameManager.Instance.sensitivity * 2f); // Multiplier to adjust the movement rate

        // Calculate the snapped position (this will be used for the gizmo)
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            snappedPosition = gridManager.GetSnappedPosition(this.gameObject);

            // Now call DisplayPiece with the calculated grid position
            gridManager.DisplayPiece(this.gameObject, transform.position);
        }
    }
}

    public void OnEndDrag(PointerEventData eventData)
    {
        // Only perform actions when dragging ends
        if (isDragging)
        {
            GridManager gridManager = FindObjectOfType<GridManager>(); // Find the single grid manager in the scene
            if (gridManager != null)
            {
                // Snap to grid (even if it's not placed, it should still snap to the nearest grid position when released)
                transform.position = snappedPosition;

                // Check and place the piece correctly on the grid (return value indicates valid placement)
                bool isValidPlacement = gridManager.CheckAndPlacePiece(this.gameObject);

                if (!isValidPlacement)
                {
                    // If placement is invalid, return the block to its original position using a coroutine
                    StartCoroutine(SmoothReturnToOriginalPosition());
                }
            }

            isDragging = false; // Reset dragging state
        }
    }

    private IEnumerator SmoothReturnToOriginalPosition()
    {
        Vector3 initialPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(initialPosition, originalPosition, elapsedTime);
            elapsedTime += Time.deltaTime * moveSmoothness;
            yield return null;
        }

        // Ensure it reaches the exact original position
        transform.position = originalPosition;

        // Smoothly return the scale to its original value
        StartCoroutine(ScaleObject(originalScale, scaleSmoothness));
    }
    private IEnumerator ScaleObject(Vector3 targetScale, float smoothness)
    {
        Vector3 initialScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime);
            elapsedTime += Time.deltaTime * smoothness;
            yield return null;
        }
        // Ensure it reaches the exact target scale
        transform.localScale = targetScale;
    }
}