using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPiece : MonoBehaviour
{
    public List<Vector2Int> shape; // Relative positions of the block's cells
    public Color blockColor;
    public Sprite blockSprite;

    public void Initialize(Vector2Int[] shapeData, Color color)
    {
        shape = new List<Vector2Int>(shapeData); // Convert to list for easier manipulation
        blockColor = color;
    }

    // Align the piece's position based on its shape's bounding box
    public void AlignPieceToCenter()
    {
        if (shape == null || shape.Count == 0)
            return;

        // Initialize min and max with the first point in the shape
        Vector2Int min = shape[0];
        Vector2Int max = shape[0];

        // Find the actual min and max bounds
        foreach (Vector2Int relativePos in shape)
        {
            if (relativePos.x < min.x) min.x = relativePos.x;
            if (relativePos.y < min.y) min.y = relativePos.y;
            if (relativePos.x > max.x) max.x = relativePos.x;
            if (relativePos.y > max.y) max.y = relativePos.y;
        }

        // Calculate the center of the bounding box
        Vector2 center = new Vector2((min.x + max.x) / 2f, (min.y + max.y) / 2f);

        // Adjust the local position of the piece to align with the center
        transform.localPosition = -center;
    }


    // Add a circle collider that fits the bounding box of the piece
    public void AddCircleCollider()
    {
        if (shape == null || shape.Count == 0)
            return;

        // Initialize min and max with the first point in the shape
        Vector2Int min = shape[0];
        Vector2Int max = shape[0];

        // Find the actual min and max bounds
        foreach (Vector2Int relativePos in shape)
        {
            if (relativePos.x < min.x) min.x = relativePos.x;
            if (relativePos.y < min.y) min.y = relativePos.y;
            if (relativePos.x > max.x) max.x = relativePos.x;
            if (relativePos.y > max.y) max.y = relativePos.y;
        }

        // Calculate the width and height of the bounding box
        float width = max.x - min.x + 1;
        float height = max.y - min.y + 1;

        // Calculate the radius as half of the larger dimension
        float radius = Mathf.Max(width, height) / 2f;

        // Add and configure the CircleCollider2D
        BoxCollider2D circleCollider = gameObject.AddComponent<BoxCollider2D>();
        circleCollider.size = new Vector2(radius*2, radius*3);

        // Offset the collider to match the center
        Vector2 center = new Vector2((min.x + max.x) / 2f, (min.y + max.y) / 2f);
        circleCollider.offset = center; // Align the collider to the local position adjustment
        float scalingMultiplier = 1f;

        transform.parent.localScale = new Vector3( Mathf.Clamp01((1f / radius) * scalingMultiplier), Mathf.Clamp01((1f / radius) * scalingMultiplier), 1);
        transform.localScale = new Vector3(1, 1, 1);
    }
}
