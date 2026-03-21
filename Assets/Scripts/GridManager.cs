using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
public class GridManager : MonoBehaviour
{
    public int rows = 10;
    public int columns = 10;
    public GameObject gridCellPrefab;  // Prefab for grid cells
    public float cellSize = 1.0f;      // Size of each cell

    public int score = 0;

    public int currentCombo;

    public float scoreComboMultiplier; // How much the combo affects the score
    public UnityEvent<float> OnGridUpdate = new UnityEvent<float>(); // UnityEvent with float argument
    public UnityEvent<float> OnComboUpdate = new UnityEvent<float>(); // UnityEvent with float argument

    public UnityEvent onRowClear;

    public Color emptycellColor;
    public Sprite emptycellSprite;

    private GridCell[,] gridCells;      // Store references to all cells

    public Transform[] pieceSpawnPoints; // Array of spawn points for the pieces

    public AudioSource audioSource;
    public AudioClip[] placementSounds;
    public AudioClip comboLoseSound;
    public AudioClip[] rowClearedSounds;

    public GameObject textParticle;

    private int oldCombo;

    private int scoreToAdd;

    private Vector3 lastPlacePos;

    public void UpdateScore()
    {
        int comboScore = Mathf.RoundToInt(scoreToAdd * (1 + currentCombo * scoreComboMultiplier));
        scoreToAdd = 0;
        score += comboScore;

        // Notify listeners about the score update
        OnGridUpdate.Invoke(score);
        OnComboUpdate.Invoke(currentCombo);
    }

    public void UpdateCombo()
    {
        if (currentCombo <= oldCombo)
        {
            currentCombo = 0;
            audioSource.PlayOneShot(comboLoseSound);
        }

        oldCombo = currentCombo;
        OnComboUpdate.Invoke(currentCombo);
    }

    public void SetVariables(GamePlayTheme gameTheme, ColorTheme colorTheme)
    {
        rows = gameTheme.rows;
        columns = gameTheme.columns;
        emptycellSprite = colorTheme.gridCellSprite;
        emptycellColor = colorTheme.emptyCellColor;
        scoreComboMultiplier = gameTheme.comboMultiplier;
        placementSounds = colorTheme.placementSounds;
        rowClearedSounds = colorTheme.rowClearSounds;
        comboLoseSound = colorTheme.comboLoseSound;
    }


    // Create the grid at the start (destroy the old grid first)
    public void CreateGrid()
    {
        // Destroy the existing grid before creating a new one
        DestroyGrid();

        gridCells = new GridCell[columns, rows];

        // Initialize grid cells
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                // Position cells directly in world space
                Vector3 worldPosition = new Vector3(x * cellSize, y * cellSize, 0);

                // Instantiate a new cell
                GameObject cellObject = Instantiate(gridCellPrefab, worldPosition, Quaternion.identity);

                // Get the GridCell component from the instantiated prefab
                GridCell cell = cellObject.GetComponent<GridCell>();
                cell.clearEffect = GameManager.Instance.colorTheme.rowClearEffect;
                cell.placeEffect = GameManager.Instance.colorTheme.blockPlaceEffect;
                cellObject.GetComponent<SpriteRenderer>().sprite = emptycellSprite;
                cellObject.GetComponent<SpriteRenderer>().color = emptycellColor;

                // Set the position and parent of the cell
                cell.transform.position = worldPosition;
                cell.transform.SetParent(transform);  // Parent it to the grid manager

                // Store the reference to this cell
                gridCells[x, y] = cell;
            }
        }

        Camera.main.transform.position = new Vector3(rows * cellSize * 0.5f - 0.5f, rows * 0.35f, -10f);
        Camera.main.orthographicSize = rows * 1.3f;

        GamePlayTheme theme = GameManager.Instance.gameTheme;
        int pieceAmount = theme.pieceAmount;
        pieceSpawnPoints = new Transform[pieceAmount];

        // Calculate spacing based on the number of pieces and the columns from the theme
        float spacing = (float)(theme.columns + 1) / (pieceAmount + 1);

        for (int i = 0; i < pieceAmount; i++)
        {
            // Calculate the x position for each spawn point
            float xPosition = spacing * (i + 1);

            // Instantiate a new spawn point
            GameObject spawnPoint = new GameObject("Spawnpoint");
            spawnPoint.transform.position = new Vector3(xPosition - 1f, -3.2f, 0f);
            pieceSpawnPoints[i] = spawnPoint.transform;
        }
        OnComboUpdate.Invoke(currentCombo);
    }

    // A method to destroy the entire grid (can be used when the game is lost)
    public void DestroyGrid()
    {
        // Check if gridCells is initialized before destroying
        if (gridCells != null)
        {
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (gridCells[x, y] != null)
                    {
                        Destroy(gridCells[x, y].gameObject); // Destroy each cell's gameObject
                    }
                }
            }
        }

        // Reset the gridCells array
        gridCells = null;
        score = 0;
        currentCombo = 0;
        OnComboUpdate.Invoke(currentCombo);
        OnGridUpdate.Invoke(0);
    }

    // Check if a piece can be placed anywhere on the grid
    public bool CanPlaceAnyPiece(BlockPiece blockPiece)
    {
        // Try placing the block at each possible position
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);

                if (CanBePlacedAt(blockPiece, gridPosition))
                {
                    return true; // Valid placement found
                }
            }
        }

        return false; // No valid placement found
    }

    // Check if the piece can be placed at a specific position
    public bool CanBePlacedAt(BlockPiece blockPiece, Vector2Int gridPosition)
    {
        foreach (Vector2Int relativePosition in blockPiece.shape)
        {
            Vector2Int checkPosition = gridPosition + relativePosition;

            if (!IsValidGridPosition(checkPosition) || gridCells[checkPosition.x, checkPosition.y].isOccupied)
            {
                return false; // Invalid placement
            }
        }

        return true; // Valid placement
    }

    // Check if a shape can be placed at a specific grid position
    public bool CanPlaceShapeAt(Vector2Int[] shape, Vector2Int gridPosition)
    {
        foreach (Vector2Int relativePosition in shape)
        {
            Vector2Int checkPosition = gridPosition + relativePosition;

            // Ensure the position is valid and not occupied
            if (!IsValidGridPosition(checkPosition) || gridCells[checkPosition.x, checkPosition.y].isOccupied)
            {
                return false;
            }
        }

        return true; // The shape can be placed
    }

    public void PlaySoundEffect(AudioClip[] clips, float pitch, float volume)
    {
        if (clips.Length > 0)
        {
            // Pick a random clip from the array
            AudioClip selectedClip = clips[Random.Range(0, clips.Length)];

            // Set the pitch
            audioSource.pitch = pitch;

            audioSource.volume = volume;

            // Play the selected clip'
            audioSource.Stop();
            audioSource.PlayOneShot(selectedClip);
        }
    }


    // Place a piece at a specific grid position
    public bool CheckAndPlacePiece(GameObject piece)
    {
        BlockPiece blockPiece = piece.GetComponent<BlockPiece>();
        if (blockPiece == null) return false;

        // Get the grid position based on piece's center
        Vector3 centerPosition = piece.transform.position;
        Vector2Int gridPosition = WorldToGridPosition(centerPosition);

        // Check if the piece can be placed at this position
        if (!CanBePlacedAt(blockPiece, gridPosition))
        {
            return false; // Can't place piece here
        }

        // Place the block
        foreach (Vector2Int relativePosition in blockPiece.shape)
        {
            Vector2Int placePosition = gridPosition + relativePosition;
            GridCell cell = gridCells[placePosition.x, placePosition.y];
            cell.SetBlock(blockPiece.blockSprite, blockPiece.blockColor);
            scoreToAdd++;
        }

        // Update the grid after placement
        UpdateGrid(piece);
        UpdateScore();
        GameManager.Instance.pieces.Remove(piece);

        // Destroy the piece
        Destroy(piece);

        // Update piece count
        GameManager.Instance.UpdatePieceCount();
        return true;
    }

    // Helper function to get affected rows and columns of the placed piece
    public (List<int> affectedRows, List<int> affectedColumns) GetAffectedRowsAndColumns(GameObject piece)
    {
        BlockPiece blockPiece = piece.GetComponent<BlockPiece>();
        if (blockPiece == null) return (new List<int>(), new List<int>());

        Vector3 centerPosition = piece.transform.position;
        Vector2Int gridPosition = WorldToGridPosition(centerPosition);

        List<int> affectedRows = new List<int>();
        List<int> affectedColumns = new List<int>();

        // Iterate over all cells the piece occupies
        foreach (Vector2Int relativePosition in blockPiece.shape)
        {
            Vector2Int occupiedPosition = gridPosition + relativePosition;

            // Check if the position is valid
            if (IsValidGridPosition(occupiedPosition))
            {
                // Add affected row and column to the lists
                int affectedRow = occupiedPosition.y;
                int affectedColumn = occupiedPosition.x;

                if (!affectedRows.Contains(affectedRow))
                    affectedRows.Add(affectedRow);
                if (!affectedColumns.Contains(affectedColumn))
                    affectedColumns.Add(affectedColumn);
            }
        }

        return (affectedRows, affectedColumns);
    }

    // Function to update the grid and clear filled rows/columns
    public void UpdateGrid(GameObject placedPiece)
    {
        var (affectedRows, affectedColumns) = GetAffectedRowsAndColumns(placedPiece);

        // Track rows and columns that need to be cleared
        List<int> rowsToClear = new List<int>();
        List<int> columnsToClear = new List<int>();

        // First, check for full rows and add score
        foreach (int row in affectedRows)
        {
            if (IsRowFull(row))
            {
                // Add score for row
                currentCombo += 1;
                scoreToAdd += rows;  // Adjust row score as needed
                rowsToClear.Add(row);
            }
        }

        // Then, check for full columns and add score
        foreach (int column in affectedColumns)
        {
            if (IsColumnFull(column))
            {
                // Add score for column
                currentCombo += 1;
                scoreToAdd += columns;  // Adjust column score as needed
                columnsToClear.Add(column);
            }
        }

        if (rowsToClear.Count > 0 || columnsToClear.Count > 0)
        {
            float centerX = columnsToClear.Count > 0 ? (float)columnsToClear.Average() : columns / 2f;
            float centerY = rowsToClear.Count > 0 ? (float)rowsToClear.Average() : rows / 2f;
            lastPlacePos = new Vector3(centerX-0.5f, centerY, -2f);
            PlaySoundEffect(rowClearedSounds, Mathf.Min(1 + (currentCombo / 30f), 2f), 1f);
            TextParticle newScoreParticle = Instantiate(textParticle, lastPlacePos, Quaternion.identity).GetComponent<TextParticle>();
            if (newScoreParticle != null)
            {
                newScoreParticle.Initialize(Mathf.RoundToInt(scoreToAdd * (1 + currentCombo * scoreComboMultiplier)).ToString("F0"), 0.8f);
            }
            onRowClear.Invoke();
        }
        else
        {
            PlaySoundEffect(placementSounds, 1f, 1f);
        }

        // Now clear the rows
        foreach (int row in rowsToClear)
        {
            ClearRow(row);
        }

        // Then clear the columns
        foreach (int column in columnsToClear)
        {
            ClearColumn(column);
        }

        OnGridUpdate.Invoke(score);
        OnComboUpdate.Invoke(currentCombo);
    }


    // Check if a row is full
    private bool IsRowFull(int row)
    {
        for (int x = 0; x < columns; x++)
        {
            if (!gridCells[x, row].isOccupied)  // If any cell is not occupied
                return false;
        }
        return true;  // All cells in the row are occupied
    }

    // Check if a column is full
    private bool IsColumnFull(int column)
    {
        for (int y = 0; y < rows; y++)
        {
            if (!gridCells[column, y].isOccupied)  // If any cell is not occupied
                return false;
        }
        return true;  // All cells in the column are occupied
    }

    // Clear a full row (reset cells or handle cleanup)
    private void ClearRow(int row)
    {
        for (int x = 0; x < columns; x++)
        {
            gridCells[x, row].ResetCell(emptycellSprite, emptycellColor);  // Reset or clear the occupied cell
        }
    }

    // Clear a full column (reset cells or handle cleanup)
    private void ClearColumn(int column)
    {
        for (int y = 0; y < rows; y++)
        {
            gridCells[column, y].ResetCell(emptycellSprite, emptycellColor);  // Reset or clear the occupied cell
        }
    }

    // Utility function to convert world position to grid position
    private Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        // Convert world position to grid position based on cell size
        int x = Mathf.RoundToInt(worldPosition.x / cellSize);
        int y = Mathf.RoundToInt(worldPosition.y / cellSize);
        return new Vector2Int(x, y);
    }

    // Check if the grid position is valid
    private bool IsValidGridPosition(Vector2Int position)
    {
        // Check if the position is within bounds
        return position.x >= 0 && position.x < columns &&
               position.y >= 0 && position.y < rows;
    }

    public void ResetDisplayPieces()
    {
        // Iterate over all grid cells
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GridCell cell = gridCells[x, y];

                if (!cell.isOccupied)
                {
                    // Reset the sprite and color of the grid cell
                    cell.GetComponent<SpriteRenderer>().sprite = emptycellSprite;
                    cell.GetComponent<SpriteRenderer>().color = emptycellColor;
                }
            }
        }
    }
    public void DisplayPiece(GameObject piece, Vector2 worldPosition)
    {
        BlockPiece blockPiece = piece.GetComponent<BlockPiece>();
        if (blockPiece == null) return;
        ResetDisplayPieces();
        if (!CanPlaceShapeAt(blockPiece.shape.ToArray(), WorldToGridPosition(worldPosition))) return;

        // Iterate over the block's shape
        foreach (Vector2Int relativePosition in blockPiece.shape)
        {
            Vector2Int displayPosition = WorldToGridPosition(worldPosition) + relativePosition;

            // Check if the position is within the grid bounds
            if (IsValidGridPosition(displayPosition))
            {
                GridCell cell = gridCells[displayPosition.x, displayPosition.y];

                // Set the cell's sprite and color to be semi-transparent
                cell.GetComponent<SpriteRenderer>().sprite = blockPiece.blockSprite;
                cell.GetComponent<SpriteRenderer>().color = new Color(blockPiece.blockColor.r, blockPiece.blockColor.g, blockPiece.blockColor.b, 0.3f);
            }
        }
    }


    // Reset grid to empty cells
    public void ClearGrid()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                gridCells[x, y].ResetCell(emptycellSprite, emptycellColor);
            }
        }
    }

    // Get the snapped position of the dragged piece
    public Vector3 GetSnappedPosition(GameObject draggedPiece)
    {
        BlockPiece blockPiece = draggedPiece.GetComponent<BlockPiece>();
        if (blockPiece == null)
        {
            return draggedPiece.transform.position;
        }

        // Get the center position of the dragged piece in world space
        Vector3 centerPosition = draggedPiece.transform.position;
        Vector2Int gridPosition = WorldToGridPosition(centerPosition);

        // For pieces that extend over multiple grid cells, snap to the top-left position of the block's bounding box
        Vector3 snappedPosition = new Vector3(gridPosition.x * cellSize, gridPosition.y * cellSize, draggedPiece.transform.position.z);

        // Optionally, adjust for the block's shape or offset if needed
        return snappedPosition;
    }

    public bool[,] GetCurrentGrid()
    {
        bool[,] grid = new bool[columns, rows];

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                grid[x, y] = gridCells[x, y].isOccupied;
            }
        }

        return grid;
    }
}
