using System;
using UnityEngine;

public class BoardSetup : MonoBehaviour
{
    enum tileCol { A, B, C, D, E, F, G, H }

    public GameObject whiteTilePrefab;
    public GameObject blackTilePrefab;
    public Transform tileParent;
    public int rows = 8;
    public int cols = 8;
    public float size = 1.0f;

    void Start()
    { 
        GenerateBoard();
        //UnityEngine.Object.FindAnyObjectByType<PieceSetup>().SetupPieces();
    }

    void GenerateBoard()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                float offset = (size * (rows - 1)) / 2;  // Center the board based on size
                                                 // Instantiate and position the tile
                Vector3 position = new Vector3(col * size - offset, row * size - offset, -1);
                GameObject tile = (row + col) % 2 == 0 ? Instantiate(blackTilePrefab, position, Quaternion.identity, tileParent) : Instantiate(whiteTilePrefab, position, Quaternion.identity, tileParent);
                
                tile.transform.localPosition = new Vector3(col * size - offset, row * size - offset, -1);


                // Name the tile
                tile.name = $"{Enum.GetName(typeof(tileCol), col)}_{row+1}";

                // Set tile color
                Renderer tileRenderer = tile.GetComponent<Renderer>();
                SpriteRenderer tileSpriteRenderer = tileRenderer.GetComponent<SpriteRenderer>();
                tileSpriteRenderer.sortingOrder = -1;
            }
        }
    }
}
