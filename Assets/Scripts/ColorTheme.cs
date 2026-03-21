using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorTheme", menuName = "Themes/ColorTheme")]
public class ColorTheme : ScriptableObject
{
    public Sprite blockPieceSprite;       // Sprite for block pieces
    public Sprite gridCellSprite;         // Sprite for grid cells
    public List<Color> pieceColors;       // List of colors for the pieces

    public Color emptyCellColor;
    public Color backgroundColor;         // Background color
    public GameObject rowClearEffect;     // Prefab for row clear effect
    public GameObject blockPlaceEffect;   // Prefab for block place effect

    public AudioClip[] placementSounds;

    public AudioClip[] rowClearSounds;

    public AudioClip comboLoseSound;
    public AudioClip loseSound;
    public AudioClip timerSound;

    // Menu related colors
    public Color titleColor;              // Color for the title text
    public Color textColor;               // Color for menu text
    public Color menuBackgroundColor;     // Color for the background of the menu
}
