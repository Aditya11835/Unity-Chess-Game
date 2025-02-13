using System;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PieceSetup : MonoBehaviour
{
    public GameObject[] piecePrefabs = new GameObject[12];
    public Transform piecesParent;
    private char[,] initialPositions = new char[8, 8]
    {
        {'R', 'N', 'B', 'Q', 'K', 'B', 'N', 'R'},
        {'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P'},
        {'0', '0', '0', '0', '0', '0', '0', '0',},
        {'0', '0', '0', '0', '0', '0', '0', '0',},
        {'0', '0', '0', '0', '0', '0', '0', '0',},
        {'0', '0', '0', '0', '0', '0', '0', '0',},
        {'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p'},
        {'r', 'n', 'b', 'q', 'k', 'b', 'n', 'r'}
    };
    enum FileName
    {
        A, B, C, D, E, F, G, H
    };

    public void SetupPieces()
    {
        float offset = (8 - 1) / 2f;
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                bool isWhite = row < 3;
                GameObject piecePrefab = GetPiecePrefab(initialPositions[row, col]);
                
                if(piecePrefab != null)
                {
                    Vector3 position = new Vector3(col-offset, row-offset, 0);
                    GameObject piece = Instantiate(piecePrefab, position, Quaternion.identity, piecesParent);
                    piece.name = $"{piecePrefab.name}_{Enum.GetName(typeof(FileName), col)}";
                    piece.transform.localScale = new Vector3(4, 4, 0);
                }
            };
        }
    }
    private GameObject GetPiecePrefab(char pieceCode)
    {
        GameObject tempGameObject;
        switch (pieceCode)
        {
            case 'P': tempGameObject = piecePrefabs[0]; break;
            case 'p': tempGameObject = piecePrefabs[1]; break;
            case 'N': tempGameObject = piecePrefabs[2]; break;
            case 'n': tempGameObject = piecePrefabs[3]; break;
            case 'B': tempGameObject = piecePrefabs[4]; break;
            case 'b': tempGameObject = piecePrefabs[5]; break;
            case 'R': tempGameObject = piecePrefabs[6]; break;
            case 'r': tempGameObject = piecePrefabs[7]; break;
            case 'Q': tempGameObject = piecePrefabs[8]; break;
            case 'q': tempGameObject = piecePrefabs[9]; break;
            case 'K': tempGameObject = piecePrefabs[10]; break;
            case 'k': tempGameObject = piecePrefabs[11]; break;
            default: tempGameObject = null; break;
        }
        return tempGameObject;
    }

}
