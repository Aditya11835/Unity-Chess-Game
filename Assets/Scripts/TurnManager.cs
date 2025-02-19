using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        if(piecesObj != null)
        {
            pieceParent = piecesObj.transform;
        }
        else
        {
            Debug.LogError("piece parent not found");
        }
        UpdateTurnText();
    }

    public bool IsWhiteTurn()
    {
        return isWhiteTurn;
    }

    public void SwitchTurn()
    {
        Debug.Log($"Last move: {lastMove}"); // Remove later
        movesHistory.Push(lastMove);
        isWhiteTurn = !isWhiteTurn;
        UpdateTurnText();
        RotateCameraAndPieces();
    }

    private void RotateCameraAndPieces()
    {
        //  Start the smooth rotation coroutine
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

        //  Rotate each piece individually
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
            float t = Mathf.SmoothStep(0, 1, elapsed / duration); // Smooth transition

            //  Apply smooth rotation to the camera
            cam.transform.rotation = Quaternion.Lerp(startCameraRotation, targetCameraRotation, t);

            //  Apply smooth rotation to each piece
            foreach (Transform piece in pieceParent)
            {
                piece.rotation = Quaternion.Lerp(startPieceRotations[piece], targetPieceRotations[piece], t);
            }

            yield return null; // Wait for next frame
        }

        //  Ensure final rotation is exact
        cam.transform.rotation = targetCameraRotation;
        foreach (Transform piece in pieceParent)
        {
            piece.rotation = targetPieceRotations[piece];
        }
    }

}
