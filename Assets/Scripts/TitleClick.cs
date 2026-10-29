
using UnityEngine;

public class TileClick : MonoBehaviour
{
    private GameManager gameManager;
    private int row;
    private int col;

    private Renderer tileRenderer;
    private Color originalColor;

    [SerializeField]
    private Color hoverColor = new Color(0.2f, 0.85f, 1f);

    void Awake()
    {
        tileRenderer = GetComponent<Renderer>();

        if (tileRenderer != null)
        {
            originalColor = tileRenderer.material.color;
        }
    }

    public void Initialize(GameManager manager, int r, int c)
    {
        gameManager = manager;
        row = r;
        col = c;
    }

    void OnMouseEnter()
    {
        if (gameManager != null &&
            gameManager.CanChoose(row, col) &&
            tileRenderer != null)
        {
            tileRenderer.material.color = hoverColor;
        }
    }

    void OnMouseExit()
    {
        if (tileRenderer != null)
        {
            tileRenderer.material.color = originalColor;
        }
    }

    void OnMouseDown()
    {
        if (tileRenderer != null)
        {
            tileRenderer.material.color = originalColor;
        }

        if (gameManager != null)
        {
            gameManager.SelectTile(row, col);
        }
    }
}
