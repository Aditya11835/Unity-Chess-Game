using System;
using System.Collections.Generic;
using UnityEngine;

public class PieceSetup : MonoBehaviour
{
    public float boardOffset = -3.5f;
   enum FileName
    {
        A, B, C, D, E, F, G, H
    };
    public GameObject[] piecePrefabs = new GameObject[12];
    public Transform piecesParent;

    public int[,] piecePositions = new int[8, 8]
    {
        {4, 2, 3, 5, 6, 3, 2, 4},
        {1, 1, 1, 1, 1, 1, 1, 1},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {7, 7, 7, 7, 7, 7, 7, 7},
        {10, 8, 9, 11, 12, 9, 8, 10}
    };

    public Dictionary<Vector2, GameObject> pieceDictionary = new Dictionary<Vector2, GameObject>();
    private GameObject GetPiecePrefab(int pieceCode)
    {
        switch (pieceCode)
        {
            case 1: return piecePrefabs[0]; // White Pawn
            case 7: return piecePrefabs[1]; // Black Pawn
            case 2: return piecePrefabs[2]; // White Knight
            case 8: return piecePrefabs[3]; // Black Knight
            case 3: return piecePrefabs[4]; // White Bishop
            case 9: return piecePrefabs[5]; // Black Bishop
            case 4: return piecePrefabs[6]; // White Rook
            case 10: return piecePrefabs[7]; // Black Rook
            case 5: return piecePrefabs[8]; // White Queen
            case 11: return piecePrefabs[9]; // Black Queen
            case 6: return piecePrefabs[10]; // White King
            case 12: return piecePrefabs[11]; // Black King
            default: return null;
        }
    }
    public void SetupPieces()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                GameObject piecePrefab = GetPiecePrefab(piecePositions[row, col]);
                Vector2 boardPosition = new Vector2(col+boardOffset, row+boardOffset);

                if (piecePrefab != null)
                {
                    Vector3 position = new Vector3(col + boardOffset, row + boardOffset, 0);
                    GameObject piece = Instantiate(piecePrefab, position, Quaternion.identity, piecesParent);
                    piece.name = $"{piecePrefab.name}_{Enum.GetName(typeof(FileName), col)}";
                    piece.transform.localScale = new Vector3(4, 4, 0);

                    //  Store piece position in Dictionary

                    pieceDictionary[boardPosition] = piece;
                }
            }
        }
    }

    private void Start()
    {
        SetupPieces();
        foreach(KeyValuePair<Vector2, GameObject> x in pieceDictionary)
        {
            Debug.Log($"{x.Key}: {x.Value}");
        }
    }   
}
