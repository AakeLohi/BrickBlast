using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GamePlayTheme", menuName = "Themes/GamePlayTheme")]
public class GamePlayTheme : ScriptableObject
{
    public string hs_playerPrefName;
    public int rows=8;
    public int columns=8;

    public float comboMultiplier = 1;

    public int pieceAmount = 3;

    public float exponent = 5;

    [SerializeField] public string[] gamePlayElementNames;
    public string shapeSet = "Default";

}
