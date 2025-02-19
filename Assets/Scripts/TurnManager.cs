using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    private bool isWhiteTurn = true;
    private KeyValuePair<Vector2, Vector2> lastMove;
    private Stack<KeyValuePair<Vector2, Vector2>> movesHistory = new Stack<KeyValuePair<Vector2, Vector2>>();
    public Text turnText; // Assign in Unity Inspector

    private Transform pieceParent;
    private Camera cam;

    public KeyValuePair<Vector2, Vector2> GetLastMove()
    {
        return lastMove;
    }

    public void SetLastMove(KeyValuePair<Vector2, Vector2> move)
    {
        lastMove = move;
    }

    private void Awake()
    {
        Debug.Log("TurnManager On");
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        cam = Camera.main;
        GameObject piecesObj = GameObject.Find("Pieces");
        if (piecesObj != null)
        {
            pieceParent = piecesObj.transform;
        }
        else
        {
            Debug.LogError("piece parent not found");
        }
        UpdateTurnText();
    }
    private void Update()
    {
        CheckForKings();
    }

    public bool IsWhiteTurn()
    {
        return isWhiteTurn;
    }

    public void SwitchTurn()
    {
        Debug.Log($"Last move: {lastMove}");
        movesHistory.Push(lastMove);      

        // **Switch Turn Normally**
        isWhiteTurn = !isWhiteTurn;
        UpdateTurnText();
        RotateCameraAndPieces();
    }




    private void RotateCameraAndPieces()
    {
        StartCoroutine(SmoothRotate(Quaternion.Euler(0, 0, 180), 0.5f));
    }

    private void UpdateTurnText()
    {
        if (turnText != null)
        {
            turnText.text = isWhiteTurn ? "White's Turn" : "Black's Turn";
        }
    }

    private IEnumerator SmoothRotate(Quaternion rotationAmount, float duration)
    {
        float elapsed = 0f;

        Quaternion startCameraRotation = cam.transform.rotation;
        Quaternion targetCameraRotation = startCameraRotation * rotationAmount;

        Dictionary<Transform, Quaternion> startPieceRotations = new Dictionary<Transform, Quaternion>();
        Dictionary<Transform, Quaternion> targetPieceRotations = new Dictionary<Transform, Quaternion>();

        foreach (Transform piece in pieceParent)
        {
            startPieceRotations[piece] = piece.rotation;
            targetPieceRotations[piece] = piece.rotation * rotationAmount;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);

            cam.transform.rotation = Quaternion.Lerp(startCameraRotation, targetCameraRotation, t);

            foreach (Transform piece in pieceParent)
            {
                piece.rotation = Quaternion.Lerp(startPieceRotations[piece], targetPieceRotations[piece], t);
            }

            yield return null;
        }

        cam.transform.rotation = targetCameraRotation;
        foreach (Transform piece in pieceParent)
        {
            piece.rotation = targetPieceRotations[piece];
        }
    }
    private GameObject FindKingObject(bool isWhite)
    {
        Transform piecesParent = GameObject.Find("Chessboard/Pieces").transform;

        foreach (Transform piece in piecesParent)
        {
            if (isWhite && piece.name.Contains("WhiteKing"))
            {
                return piece.gameObject;
            }
            else if (!isWhite && piece.name.Contains("BlackKing"))
            {
                return piece.gameObject;
            }
        }
        return null; // King not found
    }
    private bool CheckForKings()
    {
        GameObject whiteKing = FindKingObject(true);
        GameObject blackKing = FindKingObject(false);

        if (whiteKing == null)
        {
            Debug.Log("[Game Over] Black wins! White King is missing.");
            EndGame("Black Wins!");
            return false; // Stop the game immediately
        }
        else if (blackKing == null)
        {
            Debug.Log("[Game Over] White wins! Black King is missing.");
            EndGame("White Wins!");
            return false; // Stop the game immediately
        }

        return true; // Continue game if both kings are still present
    }

    private void EndGame(string message)
    {
        Debug.Log($"[Game Over] {message}");

       
        enabled = false;
        Invoke("RestartGame", 3f);  // Restart game
        
    }
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Reload current scene
    }
}
