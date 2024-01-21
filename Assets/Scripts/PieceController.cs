using System;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    [NonSerialized] public Piece piece;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public bool CallDebugMethod;
    private bool isDragging;

    private void OnValidate()
    {
        if (!CallDebugMethod) return;
        CallDebugMethod = false;
        GameController.CalculateFuturePossibleMoves(4);
    }

    public void Init(Piece piece)
    {
        this.piece = piece;
    }

    private void OnMouseDown()
    {
        isDragging = true;
        spriteRenderer.sortingLayerName = "DraggingPiece";
    }
    
    private void OnMouseDrag()
    {
        if (!isDragging) return;
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        objPosition = MapBounds.GetPositionInsideMapBounds(objPosition);
        transform.position = objPosition;
    }

    
    private void OnMouseUp()
    {
        isDragging = false;
        Vector2Int targetPosition = new Vector2Int((int)(transform.position.x + 0.5f), (int)(transform.position.y + 0.5f));
        if(TryMove(targetPosition)) return;
        CancelMove();
    }

    private bool TryMove(Vector2Int targetPosition)
    {
        bool isPlayerTurn = GameController.isWhiteTurn == piece.isWhite;
        bool isSamePosition = targetPosition == piece.position;
        if (!isPlayerTurn || isSamePosition) return false;
        Movement move = Movement.GetMovement(this, targetPosition);
        if (move != null && GameController.isMoveLegal(move))
        {
            move.ExecuteMovement();
            return true;
        }
        return false;
    }

    public void Move(Vector2Int targetPosition)
    {
        GameController.SetPiece(this, targetPosition);
        GameController.SetPiece(null, piece.position);
        piece.position = targetPosition;
        transform.position = new Vector3(targetPosition.x, targetPosition.y, 0);
    }

    public void DestroyPiece()
    {
        Destroy(gameObject);
    }

    private void CancelMove()
    {
        isDragging = false;
        transform.position = new Vector2(piece.position.x, piece.position.y);
    }

    public void Promote()
    {
        piece = new QueenController(piece.isWhite, piece.position);
        spriteRenderer.sortingLayerName = "Piece";
        spriteRenderer.sprite = CreatePiece.Instance.GetSprite(piece);
    }
}
