using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PieceController : MonoBehaviour
{
    [NonSerialized] public IPiece piece;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public bool CallDebugMethod;
    
    private Vector2 initialPosition;
    private bool isDragging = false;

    private void OnValidate()
    {
        if (!CallDebugMethod) return;
        CallDebugMethod = false;
        var attackingSquares = piece.GetAttackingSquares(piece);
        foreach (var sq in attackingSquares)
        {
            Debug.Log(sq);
        }
    }

    public void Init(IPiece piece)
    {
        this.piece = piece;
    }

    private void OnMouseDown()
    {
        isDragging = true;
        spriteRenderer.sortingLayerName = "DraggingPiece";
        initialPosition = transform.position;
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
        spriteRenderer.sortingLayerName = "Piece";
        Vector2Int targetPosition = new Vector2Int((int)(transform.position.x + 0.5f), (int)(transform.position.y + 0.5f));
        if(CheckForPossibleMoves(targetPosition)) return;
        CancelMove();
    }

    private bool CheckForPossibleMoves(Vector2Int targetPosition)
    {
        bool isPlayerTurn = GameController.isWhiteTurn == piece.isWhite;
        if (!isPlayerTurn) return false;
        if (CheckForCastle(targetPosition)) return true;
        if (CheckForEnPassant(targetPosition)) return true;
        if (isLegalMove(targetPosition))
        {
            Move(targetPosition);
            return true;
        }
        return false;
    }

    private bool CheckForEnPassant(Vector2Int targetPosition)
    {
        if (piece is not PawnController pawnController) return false;
        if (pawnController.CanEnPassant(targetPosition, piece))
        {
            EnPassant(targetPosition);
            return true;
        }
        return false;
    }

    private void EnPassant(Vector2Int targetPosition)
    {
        Vector2Int takenPiecePosition = new Vector2Int(targetPosition.x, piece.position.y);
        GameController.GetPiece(takenPiecePosition).DestroyPiece();
        Move(targetPosition);
    }

    private bool CheckForCastle(Vector2Int targetPosition)
    {
        if (piece is not KingController kingController) return false;
        bool isLeft = targetPosition.x < piece.position.x;
        int rookX = isLeft ? 0 : 7;
        Vector2Int rookPosition = new(rookX, targetPosition.y);
        PieceController rook = GameController.GetPiece(rookPosition);
        if (kingController.CanCastle(targetPosition, piece, rook.piece))
        {
            Castle(rook, isLeft);
            return true;
        }
        return false;
    }

    private void Castle(PieceController rook, bool isLeft)
    {
        Vector2Int kingTargetPosition = new Vector2Int(isLeft ? piece.position.x - 2 : piece.position.x + 2, piece.position.y);
        Move(kingTargetPosition, false);
        rook.Move(kingTargetPosition + new Vector2Int(isLeft ? 1 : -1, 0));
    }

    private bool isLegalMove(Vector2Int targetPosition)
    {
        //TODO: Add check for check
        //TODO: Add check for checkmate
        //TODO: Add check for stalemate
        if (targetPosition == initialPosition) return false;

        PieceController targetPiece = GameController.GetPiece(targetPosition);
        if (targetPiece != null && targetPiece.piece.isWhite == piece.isWhite) return false;
        if (targetPiece == null && piece.CanMove(targetPosition, piece)) return true;
        if (targetPiece != null && piece.CanTake(targetPosition, piece)) return true;
        return false;
    }

    private void Move(Vector2Int targetPosition, bool changeTurn = true)
    {
        var targetPiece = GameController.GetPiece(targetPosition);
        if (targetPiece != null) targetPiece.DestroyPiece();
        if (changeTurn) GameController.isWhiteTurn = !GameController.isWhiteTurn;
        GameController.pieces[targetPosition.x, targetPosition.y] = this;
        GameController.pieces[piece.position.x, piece.position.y] = null;
        piece.position = targetPosition;
        transform.position = new Vector3(targetPosition.x, targetPosition.y, 0);
        initialPosition = targetPosition;
        piece.hasMoved = true;
        if (piece.type == PieceType.Pawn && targetPosition.y is 0 or 7) Promote();
        GameController.UpdateGameState();
    }

    private void DestroyPiece()
    {
        GameController.pieces[piece.position.x, piece.position.y] = null;
        Destroy(gameObject);
    }

    private void CancelMove()
    {
        transform.position = initialPosition;
    }

    private void Promote()
    {
        piece = new QueenController(piece.isWhite, piece.position);
        spriteRenderer.sprite = CreatePiece.Instance.GetSprite(piece);
    }
}
