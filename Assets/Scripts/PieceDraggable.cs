using UnityEngine;

public class PieceDraggable : MonoBehaviour
{
    private Vector3 offset;
    private Camera cam;
    private float tileSize = 1f;
    private Vector3 boardOffset = new Vector3(-3.5f, -3.5f, 0);
    private Vector3 onClickPosition;
    private Vector3 newPosition;
    private PieceLogic pieceLogic;
    private PieceSetup pieceSetup;

    void Start()
    {
        cam = Camera.main;
        pieceLogic = GetComponent<PieceLogic>();// Get the attached PieceLogic script -> Gets attached subclass script
        pieceSetup = GetComponent<PieceSetup>();
    }

    private void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPos(); //Difference between cursor position and center of sprite
        onClickPosition = transform.position;
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + offset; //Maintains the difference so sprite doesn't snap to cursor
    }

    private void OnMouseUp()
    {
        newPosition = transform.position;
        SnapToGrid();
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 0f;
        return cam.ScreenToWorldPoint(mousePoint);
    }

    private void SnapToGrid()
    {
        // Ensure the piece doesn't go out of bounds
        if (newPosition.x > 4 || newPosition.x < -4 || newPosition.y > 4 || newPosition.y < -4)
        {
            transform.position = onClickPosition;
            return;
        }

        // Validate the move using PieceLogic
        if (pieceLogic != null)
        {
            if (pieceLogic.IsValidMove(onClickPosition, newPosition))
            {
                float snappedX = Mathf.Round((newPosition.x - boardOffset.x) / tileSize) * tileSize + boardOffset.x;
                float snappedY = Mathf.Round((newPosition.y - boardOffset.y) / tileSize) * tileSize + boardOffset.y;
                transform.position = new Vector3(snappedX, snappedY, 0f);
            }
            else
            {
                transform.position = onClickPosition;
            }
        }
        else
        {
            transform.position = onClickPosition; // Reset if invalid
        }
    }
}
