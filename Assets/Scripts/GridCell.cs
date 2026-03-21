using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GridCell : MonoBehaviour
{
    public bool isOccupied = false;
    public SpriteRenderer spriteRenderer;

    public GameObject placeEffect;
    public GameObject clearEffect;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Reset the cell, clearing the block and resetting the occupation status
    public void ResetCell(Sprite emptySprite, Color emptyColor)
    {
        isOccupied = false;
        spriteRenderer.sprite = emptySprite;
        spriteRenderer.color = emptyColor;  // Reset color to default
        if (clearEffect != null) Instantiate(clearEffect, transform.position, Quaternion.identity);
    }

    // Set the block's sprite and mark the cell as occupied
    public void SetBlock(Sprite sprite, Color blockColor)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = blockColor;
        if (placeEffect != null) Instantiate(placeEffect, transform.position, Quaternion.identity);
        isOccupied = true;
    }

    public void CreateParticle()
    {
        Instantiate(placeEffect, transform.position, Quaternion.identity);
    }
}
