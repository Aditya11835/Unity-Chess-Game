using UnityEngine;

public class PieceDraggable : MonoBehaviour
{
    private Vector3 offset;
    private Camera cam;
    private float tileSize = 1f;
    private Vector3 boardOffset = new Vector3(-3.5f, -3.5f, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Vector3 onClickPosition;
    void Start()
    {
        cam = Camera.main;
    }
    private void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPos();
        onClickPosition = transform.position;
    }
    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + offset;
    }
    private void OnMouseUp()
    {
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
        if (transform.position.x > 4 || transform.position.x < -4 || transform.position.y > 4 || transform.position.y < -4)
        {
            transform.position = onClickPosition;
        }

        float snappedX = Mathf.Round((transform.position.x - boardOffset.x) / tileSize) * tileSize + boardOffset.x;
        float snappedY = Mathf.Round((transform.position.y - boardOffset.y) / tileSize) * tileSize + boardOffset.y;

        transform.position = new Vector3(snappedX, snappedY, 0f);
    }
}
