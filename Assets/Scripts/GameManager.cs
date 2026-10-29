
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject xPiecePrefab;
    public GameObject oPiecePrefab;

    [Header("Board Settings")]
    public float spacing = 2f;
    public float tileY = 0.08f;
    public float spawnHeight = 5f;
    public float cellSize = 1.8f;

    // 0 = ô trống, 1 = X, 2 = O
    private int[] board = new int[9];

    private bool isXTurn = true;
    private bool gameOver = false;

    private readonly int[,] winLines =
    {
        { 0, 1, 2 },
        { 3, 4, 5 },
        { 6, 7, 8 },

        { 0, 3, 6 },
        { 1, 4, 7 },
        { 2, 5, 8 },

        { 0, 4, 8 },
        { 2, 4, 6 }
    };

    void Start()
    {
        CreateBoard();
        Debug.Log("Bắt đầu game! Lượt của X.");
    }

    void CreateBoard()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("Chưa gán Tile Prefab!");
            return;
        }

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                Vector3 position = new Vector3(
                    (col - 1) * spacing,
                    tileY,
                    (1 - row) * spacing
                );

                GameObject tile = Instantiate(
                    tilePrefab,
                    position,
                    Quaternion.identity,
                    transform
                );

                tile.name = "Tile_" + row + "_" + col;

                tile.transform.localScale = new Vector3(
                    cellSize, 0.16f, cellSize
                );

                TileClick clickScript =
                    tile.GetComponent<TileClick>();

                if (clickScript != null)
                {
                    clickScript.Initialize(this, row, col);
                }
                else
                {
                    Debug.LogError(
                        "TilePrefab chưa có script TileClick!"
                    );
                }
            }
        }
    }

    public bool CanChoose(int row, int col)
    {
        int index = row * 3 + col;

        return !gameOver && board[index] == 0;
    }

    public void SelectTile(int row, int col)
    {
        if (!CanChoose(row, col))
        {
            return;
        }

        if (xPiecePrefab == null || oPiecePrefab == null)
        {
            Debug.LogError("Chưa gán prefab X hoặc O!");
            return;
        }

        int index = row * 3 + col;

        int player = isXTurn ? 1 : 2;
        board[index] = player;

        GameObject prefab = isXTurn
            ? xPiecePrefab
            : oPiecePrefab;

        Vector3 spawnPosition = new Vector3(
            (col - 1) * spacing,
            spawnHeight,
            (1 - row) * spacing
        );

        GameObject piece = Instantiate(
            prefab,
            spawnPosition,
            prefab.transform.rotation
        );

        // Cho phép click xuyên qua quân cờ
        // để có thể chọn các ô nằm bên dưới.
        int ignoreRaycastLayer =
            LayerMask.NameToLayer("Ignore Raycast");

        SetLayerRecursively(piece, ignoreRaycastLayer);

        Rigidbody rb = piece.GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = piece.AddComponent<Rigidbody>();
        }

        if (piece.GetComponent<Collider>() == null)
        {
            piece.AddComponent<BoxCollider>();
        }

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Kiểm tra chiến thắng
        if (CheckWinner(player))
        {
            gameOver = true;

            Debug.Log(
                (player == 1 ? "X" : "O") + " chiến thắng!"
            );

            return;
        }

        // Kiểm tra hòa
        if (IsDraw())
        {
            gameOver = true;
            Debug.Log("Trận đấu hòa!");
            return;
        }

        // Chuyển lượt
        isXTurn = !isXTurn;

        Debug.Log(
            "Lượt của " + (isXTurn ? "X" : "O")
        );
    }

    bool CheckWinner(int player)
    {
        for (int i = 0; i < 8; i++)
        {
            int a = winLines[i, 0];
            int b = winLines[i, 1];
            int c = winLines[i, 2];

            if (board[a] == player &&
                board[b] == player &&
                board[c] == player)
            {
                return true;
            }
        }

        return false;
    }

    bool IsDraw()
    {
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] == 0)
            {
                return false;
            }
        }

        return true;
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        if (layer < 0)
        {
            return;
        }

        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
