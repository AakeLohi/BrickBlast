using System.Collections.Generic;
using UnityEngine;

public static class PieceDefinitions
{
    // Store all possible piece sets as a dictionary of dictionaries.
    private static Dictionary<string, Dictionary<string, Vector2Int[][]>> AllPieceSets = new Dictionary<string, Dictionary<string, Vector2Int[][]>>();

    // This will hold the active piece set used by other scripts.
    public static Dictionary<string, Vector2Int[][]> Shapes { get; private set; }

    static PieceDefinitions()
    {
        // Initialize some piece sets here.
        // You can add more sets later.
        AllPieceSets.Add("Default", new Dictionary<string, Vector2Int[][]>
        {
            { "Dot", new Vector2Int[][] { new Vector2Int[] { new Vector2Int(0, 0) } } },
            { "Square", new Vector2Int[][] { new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) } } },
            { "TShape", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0) }
            } },
            { "2Line", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1) }
            } },
            { "3Line", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1) }
            } },
            { "4Line", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
                new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) }
            } },
            { "5Line", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-2, 0), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
                new Vector2Int[] { new Vector2Int(0, -2), new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) }
            } },
            { "Porras", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, -1) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1) }
            } },
            { "3x3", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1) }
            } },
            { "2x3", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1) }
            } },
            { "3x2", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2) }
            } },
            { "LShape", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(2, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(0, -2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(-2, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(-2, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, -2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(2, 0) }
            } },
            { "LongLShape", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(2, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, -2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(0, -2), new Vector2Int(-2, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(-2, 0), new Vector2Int(0, 2) },
            } },
            { "Squiggly1", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(2, 1) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, 1), new Vector2Int(-1, 2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(-2, -1) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(1, -1), new Vector2Int(1, -2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(-2, 1) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(2, -1) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(-1, -1), new Vector2Int(-1, -2) }
            }}
        });

        // Example of adding another piece set
        AllPieceSets.Add("Advanced", new Dictionary<string, Vector2Int[][]>
        {
            { "Dot", new Vector2Int[][] { new Vector2Int[] { new Vector2Int(0, 0) } } },
            { "Square", new Vector2Int[][] { new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) } } },
            { "TShape", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0) }
            } },
            { "2Line", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1) }
            } },
            { "3Line", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1) }
            } },
            { "4Line", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
                new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) }
            } },
            { "5Line", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-2, 0), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
                new Vector2Int[] { new Vector2Int(0, -2), new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) }
            } },
            { "Porras", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, -1) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1) }
            } },
            { "3x3", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1) }
            } },
            { "2x3", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1) }
            } },
            { "3x2", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2) }
            } },
            { "LShape", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(2, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(0, -2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(-2, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(-2, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, -2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(2, 0) }
            } },
            { "LongLShape", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(2, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, -2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(0, -2), new Vector2Int(-2, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(-2, 0), new Vector2Int(0, 2) },
            } },
            { "Squiggly1", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(2, 1) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, 1), new Vector2Int(-1, 2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(-2, -1) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(1, -1), new Vector2Int(1, -2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(-2, 1) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(2, -1) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(-1, -1), new Vector2Int(-1, -2) }
            }}
        });

                // Example of adding another piece set
        AllPieceSets.Add("Big", new Dictionary<string, Vector2Int[][]>
        {
            { "Dot", new Vector2Int[][] { new Vector2Int[] { new Vector2Int(0, 0) } } },
            { "Square", new Vector2Int[][] { new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) } } },
            { "TShape", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0) }
            } },
            { "2Line", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1) }
            } },
            { "3Line", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1) }
            } },
            { "4Line", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
                new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) }
            } },
            { "5Line", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-2, 0), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
                new Vector2Int[] { new Vector2Int(0, -2), new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) }
            } },
            { "Porras", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, -1) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1) }
            } },
            { "3x3", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1) }
            } },
            { "2x3", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1) }
            } },
            { "3x2", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2) }
            } },
            { "LShape", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(2, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(0, -2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(-2, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(-2, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, -2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(2, 0) }
            } },
            { "LongLShape", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(2, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, -2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(0, -2), new Vector2Int(-2, 0) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(-2, 0), new Vector2Int(0, 2) }
            } },
            { "Squiggly1", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(2, 1) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, 1), new Vector2Int(-1, 2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(-2, -1) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(1, -1), new Vector2Int(1, -2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(-2, 1) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 2) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(2, -1) },
                new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(-1, -1), new Vector2Int(-1, -2) }
            } },

            // New shapes
            { "4x4", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1),
                                new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0),
                                new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1), new Vector2Int(2, -1),
                                new Vector2Int(-1, -2), new Vector2Int(0, -2), new Vector2Int(1, -2), new Vector2Int(2, -2) }
            } },
            { "5x5", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-2, 2), new Vector2Int(-1, 2), new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2),
                                new Vector2Int(-2, 1), new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1),
                                new Vector2Int(-2, 0), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0),
                                new Vector2Int(-2, -1), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1), new Vector2Int(2, -1),
                                new Vector2Int(-2, -2), new Vector2Int(-1, -2), new Vector2Int(0, -2), new Vector2Int(1, -2), new Vector2Int(2, -2) }
            } },
            { "3x5", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 2), new Vector2Int(0, 2), new Vector2Int(1, 2),
                                new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1),
                                new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0),
                                new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1),
                                new Vector2Int(-1, -2), new Vector2Int(0, -2), new Vector2Int(1, -2) }
            } },
            { "2x4", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1),
                                new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) }
            } },
            { "4x3", new Vector2Int[][] {
                new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1),
                                new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0),
                                new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1) }
            } }
        });


        // Set the default piece set
        Shapes = AllPieceSets["Default"];
    }

    // Function to activate a different piece set
    public static void SetPieceSet(string setName)
    {
        if (AllPieceSets.ContainsKey(setName))
        {
            Shapes = AllPieceSets[setName];
        }
        else
        {
            Debug.LogWarning($"Piece set '{setName}' not found. Keeping the current piece set.");
        }
    }
}


public class PieceGenerator : MonoBehaviour
{
    public GameObject blockPrefab;

    public Sprite blockSprite;

    public Sprite cellSprite;
    public Color[] pieceColors;

    public GameObject GeneratePiece(Vector2Int[] shape, Color color, Transform parentGrid, Vector3 spawnPosition)
    {
        // Create the piece GameObject
        GameObject piece = new GameObject("a");
        GameObject blockPiecesHolder = new GameObject("Pieces");
        blockPiecesHolder.transform.SetParent(piece.transform);  // Set parent first
        piece.transform.SetParent(parentGrid);  // Set parent first
        piece.transform.localPosition = Vector3.zero;  // Adjust local position after parenting

        // Add components to handle drag and collider
        BlockPiece blockPiece = piece.AddComponent<BlockPiece>();
        DragHandler dragHandler = piece.AddComponent<DragHandler>();

        // Initialize the block piece with shape and color, but don't do alignment yet
        blockPiece.Initialize(shape, color);

        // Instantiate blocks based on the shape
        foreach (Vector2Int position in shape)
        {
            GameObject block = Instantiate(blockPrefab, piece.transform.position + new Vector3(position.x, position.y, 0), Quaternion.identity);
            block.GetComponent<SpriteRenderer>().color = color;
            block.GetComponent<SpriteRenderer>().sprite = blockSprite;
            blockPiece.blockSprite = blockSprite;
            block.transform.SetParent(blockPiecesHolder.transform);
        }

        // After instantiating, align the piece and add the collider
        blockPiece.AddCircleCollider();
        blockPiece.AlignPieceToCenter();

        return piece;
    }

    public void SetVariables(Sprite newBlockSprite, Color[] newPieceColors)
    {
        blockSprite = newBlockSprite;
        pieceColors = newPieceColors;
    }

}

