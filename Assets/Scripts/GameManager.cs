using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public ColorTheme[] allColorThemes;
    public ColorTheme colorTheme;
    public GamePlayTheme gameTheme;
    public GridManager gridManager;
    public PieceGenerator pieceGenerator;
    public Transform[] spawnPoints;
    public List<GameObject> pieces;
    public UnityEvent OnLose;
    public UnityEvent OnNewRound;
    public float sensitivity = 0.5f;

    public Transform gamePlayScreen;
    
    public float pieceSelectionBias = 3f; //The exponent for random indexing, where higher values skew the selection towards lower indexes(bigger shapes)
    public UnityEvent<float> OnNewGame = new UnityEvent<float>(); // UnityEvent with float argument

    private bool isPaused = false;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ApplyThemes();
        SetFrameRate();
    }

    public void SetFrameRate()
    {
        switch (PlayerPrefs.GetInt("GraphicsQuality", 2))
        {
            case 1:
                Application.targetFrameRate = 30;
                break;
            case 2:
                Application.targetFrameRate = 60;
                break;
            case 3:
                Application.targetFrameRate = 120;
                break;
            default:
                Application.targetFrameRate = 60; // Default to 60 if the value is unexpected
                break;
        }
    }

    public void SetGameModeAndStart(GamePlayTheme selectedGameTheme)
    {
        if (selectedGameTheme != null) gameTheme = selectedGameTheme;
        ApplyThemes();
        StartGame();
    }

    public void SetNewTheme(string newColorThemeName)
    {
        // Search for the color theme by name
        foreach (var theme in allColorThemes)
        {
            if (theme.name == newColorThemeName)
            {
                colorTheme = theme;
                ApplyThemes();
                return;
            }
        }
    }

    private void ApplyThemes()
    {
        // Apply color theme
        Camera.main.backgroundColor = colorTheme.backgroundColor;
        PieceDefinitions.SetPieceSet(gameTheme.shapeSet);
        pieceGenerator.SetVariables(colorTheme.blockPieceSprite, colorTheme.pieceColors.ToArray());
        gridManager.SetVariables(gameTheme, colorTheme);
        pieceSelectionBias = gameTheme.exponent;
    }

    public void ActivateGamePlayElements()
    {
        foreach (string elementName in gameTheme.gamePlayElementNames)
        {
            // Find the GameObject by name under the gamePlayScreen transform
            Transform element = gamePlayScreen.Find(elementName);

            if (element != null)
            {
                // Activate the GameObject (set it to active)
                element.gameObject.SetActive(true);
            }
        }
    }

    public void StartGame()
    {
        ActivateGamePlayElements();
        OnNewGame.Invoke(PlayerPrefs.GetInt(gameTheme.hs_playerPrefName, 0));
        // Generate the grid
        gridManager.CreateGrid();

        // Generate and spawn pieces
        pieces = new List<GameObject>();

        GenerateNewPieces();
    }

    public void UpdatePieceCount()
    {
        int amount = 0;
        foreach (GameObject piece in pieces)
        {
            if (piece != null)
            {
                amount++;
            }
        }

        if (amount > 0)
        {
            bool canPlaceAnyPiece = false;
            foreach (GameObject piece in pieces)
            {
                BlockPiece blockPiece = piece.GetComponent<BlockPiece>();
                if (blockPiece != null && gridManager.CanPlaceAnyPiece(blockPiece))
                {
                    canPlaceAnyPiece = true;
                    break;
                }
            }

            if (!canPlaceAnyPiece)
            {
                EndGame();
            }
        }
        else
        {
            GenerateNewPieces();
            OnNewRound.Invoke();
        }
    }

    public void EndGame()
    {
        int currentScore = gridManager.score;
        int highScore = PlayerPrefs.GetInt(gameTheme.hs_playerPrefName, 0);

        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt(gameTheme.hs_playerPrefName, currentScore);
            PlayerPrefs.Save();
        }

        foreach (GameObject piece in pieces)
        {
            Destroy(piece);
        }
        pieces.Clear();

        OnLose.Invoke();
    }

    public void PauseGame()
    {
        if (!isPaused)
        {
            Time.timeScale = 0f;  // Stops time, effectively pausing the game
            isPaused = true;
        }
    }
    public void ResumeGame()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;  // Resumes time, continuing the game
            isPaused = false;
        }
    }


    public void ExitApp()
    {
        Application.Quit();
    }
    public void GenerateNewPieces()
    {
        pieces.Clear();

        // Get all the shapes sorted by size (descending order)
        List<Vector2Int[]> sortedShapes = PieceDefinitions.Shapes.Values
            .SelectMany(shapes => shapes)
            .ToList();

        List<Vector2Int[]> possibleShapes = new List<Vector2Int[]>();  // To store all the fitting shapes
        List<Vector2Int[]> selectedShapes = new List<Vector2Int[]>();  // To store all the fitting shapes

        bool[,] simulationGrid = gridManager.GetCurrentGrid(); // Initialize a simulation grid
        bool isGridEmpty = IsGridEmpty(); // Check if the grid is empty

        if (isGridEmpty) //START OR CLEAR: CHOOSE BIG SHAPES
        {
            possibleShapes = GetPossiblePiecesThatFit(gridManager.GetCurrentGrid(), sortedShapes);
            
            possibleShapes = possibleShapes.OrderByDescending(shape => shape.Length).ToList();

            Vector2Int[] selectedShape = possibleShapes[0];// Initialization of selectedshape

            for (int i = 0; i < gameTheme.pieceAmount; i++)
            {
                float normalizedRandomIndex = Random.Range(0f, possibleShapes.Count) / (float)possibleShapes.Count;
                float transformedValue = Mathf.Pow(normalizedRandomIndex, pieceSelectionBias*1.5f);
                int finalIndex = Mathf.Clamp(Mathf.RoundToInt(transformedValue * possibleShapes.Count), 0, possibleShapes.Count - 1);

                selectedShape = possibleShapes[finalIndex];

                // Check if the selected shape is already added
                if (selectedShapes.Any(shape => shape.SequenceEqual(selectedShape)))
                {
                    possibleShapes.Remove(selectedShape);
                }

                selectedShapes.Add(selectedShape);
            }
        }
        else //SIMULATED GENERATION: GENERATE A RANDOM PIECE FAVORING BIGGER ONES, THEN SIMULATE IT AND GENERATE PIECES INTO THE SIMULATION TO ENSURE FIT
        {
            for (int i = 0; i < gameTheme.pieceAmount; i++)
            {
                possibleShapes = GetPossiblePiecesThatFit(simulationGrid, sortedShapes);
                possibleShapes = possibleShapes.OrderByDescending(shape => shape.Length).ToList();
                Vector2Int[] selectedShape = possibleShapes[0];// Initialization of selectedshape

                if (possibleShapes.Count < 4) // if less than 4 possible shapes, just choose biggest
                {
                    selectedShape = possibleShapes[0];

                    selectedShapes.Add(selectedShape);
                }
                else
                {
                    float normalizedRandomIndex = Random.Range(0f, possibleShapes.Count) / (float)possibleShapes.Count;
                    float transformedValue = Mathf.Pow(normalizedRandomIndex, pieceSelectionBias);
                    int finalIndex = Mathf.Clamp(Mathf.RoundToInt(transformedValue * possibleShapes.Count), 0, possibleShapes.Count - 1);

                    selectedShape = possibleShapes[finalIndex];
                    // Check if the selected shape is already added
                    if (selectedShapes.Any(shape => shape.SequenceEqual(selectedShape)))
                    {
                        possibleShapes.Remove(selectedShape);
                    }

                    selectedShapes.Add(selectedShape);
                }

                Vector2Int optimalPos = SimulateOptimalPlacement(selectedShape, simulationGrid);
                PlaceShapeOnGrid(simulationGrid, selectedShape, optimalPos);
                UpdateSimulationGrid(simulationGrid);
            }
        }

        // Randomize the selected shapes order
        selectedShapes = selectedShapes.OrderBy(_ => Random.value).ToList(); 

        // Generate pieces in the selected random order
        for (int i = 0; i < selectedShapes.Count; i++)
        {
            Vector2Int[] selectedShape = selectedShapes[i];

            // Generate a random color
            Color selectedColor = pieceGenerator.pieceColors[Random.Range(0, pieceGenerator.pieceColors.Length)];

            // Generate the final piece and place it under the corresponding spawn point
            GameObject newPiece = pieceGenerator.GeneratePiece(selectedShape, selectedColor, gridManager.pieceSpawnPoints[i], gridManager.pieceSpawnPoints[i].position);

            // Add the generated piece to the final list
            pieces.Add(newPiece);
        }
    }

    private Vector2Int SimulateOptimalPlacement(Vector2Int[] shape, bool[,] grid)
    {
        Vector2Int optimalPosition = Vector2Int.zero;
        int maxClearedLines = 0;
        int bestProximityScore = int.MaxValue;

        // Create a deep copy of the grid
        bool[,] simulationGrid = DeepCopyGrid(grid);
        bool[,] originalGrid = DeepCopyGrid(grid);

        // Simulate placement
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);

                if (CanPlaceShapeOnGrid(simulationGrid, shape, gridPosition))
                {
                    PlaceShapeOnGrid(simulationGrid, shape, gridPosition);

                    // Evaluate the placement (count cleared lines)
                    int clearedLines = CountClearedLines(simulationGrid);

                    // Calculate wall proximity score
                    int proximityScore = CalculateWallProximityScore(gridPosition, shape, grid.GetLength(0), grid.GetLength(1));

                    simulationGrid = DeepCopyGrid(originalGrid);  // Restore grid after evaluation

                    // If this placement clears more lines, or if it clears the same number of lines but is closer to the wall, set it as the optimal position
                    if (clearedLines > maxClearedLines || (clearedLines == maxClearedLines && proximityScore < bestProximityScore))
                    {
                        maxClearedLines = clearedLines;
                        bestProximityScore = proximityScore; 
                        optimalPosition = gridPosition;
                    }

                }
            }
        }

        return optimalPosition;
    }

    private int CalculateWallProximityScore(Vector2Int position, Vector2Int[] shape, int gridWidth, int gridHeight)
    {
        int minDistanceToWall = int.MaxValue;

        foreach (Vector2Int cell in shape)
        {
            int x = position.x + cell.x;
            int y = position.y + cell.y;

            // Calculate distances to all walls
            int distanceToLeftWall = x;
            int distanceToRightWall = gridWidth - x - 1;
            int distanceToTopWall = y;
            int distanceToBottomWall = gridHeight - y - 1;

            // Find the minimum distance to any wall for this cell
            int minDistanceForCell = Mathf.Min(distanceToLeftWall, distanceToRightWall, distanceToTopWall, distanceToBottomWall);

            // Update the overall minimum distance
            minDistanceToWall = Mathf.Min(minDistanceToWall, minDistanceForCell);
        }

        return minDistanceToWall;
    }


    private bool[,] DeepCopyGrid(bool[,] original)
    {
        int rows = original.GetLength(0);
        int cols = original.GetLength(1);
        bool[,] copy = new bool[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                copy[i, j] = original[i, j];
            }
        }
        return copy;
    }


    public bool IsGridEmpty()
    {
        // Get the current grid from gridManager
        bool[,] grid = gridManager.GetCurrentGrid();

        // Iterate through all cells in the grid
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y])  // As soon as we find a true value, we return false
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool CanPlaceShapeOnGrid(bool[,] grid, Vector2Int[] shape, Vector2Int position)
    {
        // Check if all the cells the shape occupies are available (i.e., false in the grid)
        foreach (Vector2Int cell in shape)
        {
            int x = position.x + cell.x;
            int y = position.y + cell.y;

            if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1) || grid[x, y])
            {
                return false; // Shape cannot be placed here
            }
        }

        return true;
    }

    private int CountClearedLines(bool[,] grid)
    {
        int clearedLines = 0;

        // Check for filled rows
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            bool isRowFull = true;
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (!grid[x, y])
                {
                    isRowFull = false;
                    break;
                }
            }
            if (isRowFull)
            {
                clearedLines++;
            }
        }

        // Check for filled columns
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            bool isColumnFull = true;
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (!grid[x, y])
                {
                    isColumnFull = false;
                    break;
                }
            }
            if (isColumnFull)
            {
                clearedLines++;
            }
        }

        return clearedLines;
    }



    private void PlaceShapeOnGrid(bool[,] grid, Vector2Int[] shape, Vector2Int position)
    {
        // Mark the cells the shape occupies as filled
        foreach (Vector2Int cell in shape)
        {
            int x = position.x + cell.x;
            int y = position.y + cell.y;

            // Check if the calculated position is within bounds
            if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
            {
                grid[x, y] = true; // Place the shape here
            }
        }
    }

    private List<Vector2Int[]> GetPossiblePiecesThatFit(bool[,] grid, List<Vector2Int[]> shapes)
    {
        List<Vector2Int[]> fittingShapes = new List<Vector2Int[]>(); // List to store the fitting shapes
        foreach (Vector2Int[] currentShape in shapes)
        {
            bool shapeAdded = false;

            for (int x = 0; x < grid.GetLength(0) && !shapeAdded; x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    Vector2Int gridPosition = new Vector2Int(x, y);

                    if (CanPlaceShapeOnGrid(grid, currentShape, gridPosition))
                    {
                        fittingShapes.Add(currentShape);
                        shapeAdded = true;
                        break; // Breaks out of the inner loop
                    }
                }
            }
        }
        return fittingShapes;
    }

    private void UpdateSimulationGrid(bool[,] grid)
    {
        List<int> rowsToClear = new List<int>();
        List<int> columnsToClear = new List<int>();

        // Check and add full rows to the rowsToClear list
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            bool isRowFull = true;

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (!grid[x, y]) // If any cell in the row is not occupied
                {
                    isRowFull = false;
                    break;
                }
            }

            if (isRowFull)
            {
                rowsToClear.Add(y); // Add row to the list to be cleared
            }
        }

        // Check and add full columns to the columnsToClear list
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            bool isColumnFull = true;

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (!grid[x, y]) // If any cell in the column is not occupied
                {
                    isColumnFull = false;
                    break;
                }
            }

            if (isColumnFull)
            {
                columnsToClear.Add(x); // Add column to the list to be cleared
            }
        }

        // Now clear the rows
        foreach (int row in rowsToClear)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                grid[x, row] = false; // Clear the row
            }
        }

        // Then clear the columns
        foreach (int column in columnsToClear)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                grid[column, y] = false; // Clear the column
            }
        }
    }

    string GetGridString(bool[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int y = rows - 1; y >= 0; y--) // Iterate rows in reverse order
        {
            for (int x = 0; x < cols; x++)
            {
                sb.Append(grid[x, y] ? "█ " : "░ ");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

}
