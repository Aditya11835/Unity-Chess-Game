using System;
using UnityEngine;

public class BoardSetup : MonoBehaviour
{
    enum tileCol { A, B, C, D, E, F, G, H }

    public GameObject whiteTilePrefab;
    public GameObject blackTilePrefab;
    public GameObject borderTilePrefab; // Prefab for border tiles
    public Transform tileParent;
    public Transform borderParent; // Parent for border tiles
    public Transform textParent; // Parent for text labels

    public int rows = 8;
    public int cols = 8;
    public float size = 1.0f;

    void Start()
    {
        GenerateBoard();
        GenerateBorder();
    }

    void GenerateBoard()
    {
        float offset = (size * (rows - 1)) / 2; // Center the board

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector3 position = new Vector3(col * size - offset, row * size - offset, -1);
                GameObject tile = (row + col) % 2 == 0
                    ? Instantiate(blackTilePrefab, position, Quaternion.identity, tileParent)
                    : Instantiate(whiteTilePrefab, position, Quaternion.identity, tileParent);

                tile.transform.localPosition = position;
                tile.name = $"{Enum.GetName(typeof(tileCol), col)}_{row + 1}";

                // **Set sorting order to ensure tiles render behind pieces**
                SetSortingOrder(tile, -1);
            }
        }
    }

    void GenerateBorder()
    {
        float offset = (size * (rows - 1)) / 2; // Centering the board
        float borderOffset = size * 0.5f; // Adjust border position

        for (int i = 0; i < 8; i++)
        {
            // **Bottom border (A-H)**
            PlaceBorderTile(new Vector3(i * size - offset, -4.5f, -1));
            PlaceTextMesh(new Vector3(i * size - offset, -4.5f, -0.9f), $"{(tileCol)i}", false);

            // **Top border (A-H) - Rotated**
            PlaceBorderTile(new Vector3(i * size - offset, 4.5f, -1));
            PlaceTextMesh(new Vector3(i * size - offset, 4.5f, -0.9f), $"{(tileCol)i}", true);

            // **Left border (1-8)**
            PlaceBorderTile(new Vector3(-4.5f, i * size - offset, -1));
            PlaceTextMesh(new Vector3(-4.5f, i * size - offset, -0.9f), $"{8 - i}", false);

            // **Right border (1-8) - Rotated**
            PlaceBorderTile(new Vector3(4.5f, i * size - offset, -1));
            PlaceTextMesh(new Vector3(4.5f, i * size - offset, -0.9f), $"{8 - i}", true);
        }

        // **Add Corner Border Tiles (No Text)**
        PlaceBorderTile(new Vector3(-4.5f, -4.5f, -1)); // Bottom-left corner
        PlaceBorderTile(new Vector3(4.5f, -4.5f, -1));  // Bottom-right corner
        PlaceBorderTile(new Vector3(-4.5f, 4.5f, -1));  // Top-left corner
        PlaceBorderTile(new Vector3(4.5f, 4.5f, -1));   // Top-right corner
    }


    void PlaceBorderTile(Vector3 position)
    {
        GameObject borderTile = Instantiate(borderTilePrefab, position, Quaternion.identity, borderParent);
        borderTile.transform.localPosition = position;
        borderTile.transform.localScale = new Vector3(size, size, 1);

        // **Set sorting order to ensure border renders behind pieces**
        SetSortingOrder(borderTile, -1);
    }

    void PlaceTextMesh(Vector3 position, string text, bool rotate)
    {
        GameObject textObject = new GameObject("Text_" + text);
        TextMesh textMesh = textObject.AddComponent<TextMesh>();

        // **Set TextMesh properties**
        textMesh.text = text;
        textMesh.fontSize = 30;
        textMesh.characterSize = 0.1f;
        textMesh.color = Color.white;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;

        textObject.transform.position = position;
        textObject.transform.parent = textParent;

        // **Rotate only top and right labels**
        if (rotate)
        {
            textObject.transform.rotation = Quaternion.Euler(0, 0, 180);
        }

        // **Set sorting order to ensure text renders in front of border**
        SetSortingOrder(textObject, 0);
    }

    void SetSortingOrder(GameObject obj, int sortingOrder)
    {
        Renderer objRenderer = obj.GetComponent<Renderer>();
        if (objRenderer != null)
        {
            objRenderer.sortingOrder = sortingOrder;
        }
    }
}
