using System.Collections.Generic;
using UnityEngine;

public class PawnController : Piece
{
    public bool hasMovedTwiceLastTurn = false;
    
    public PawnController(bool isWhite, Vector2Int position) : base(isWhite, position) { }
    
    public override bool CanMove(Vector2Int targetPos, Piece piece, bool forced = false)
    {
        bool legalMove = isMoveLegal(targetPos, piece);
        return forced || legalMove;
    }
    public override bool CanTake(Vector2Int targetPos, Piece piece)
    {
        int distanceX = Mathf.Abs(targetPos.x - piece.position.x);
        int distanceY = targetPos.y - piece.position.y;
        if (!piece.isWhite) distanceY *= -1;
        if (distanceX == 1 && distanceY == 1) return true;
        return false;
    }
    public override bool isMoveLegal(Vector2Int targetPos, Piece piece)
    {
        int distanceX = targetPos.x - piece.position.x;
        int distanceY = targetPos.y - piece.position.y;
        int directionY = piece.isWhite ? 1 : -1;
        if (!piece.isWhite) distanceY *= -1;
        
        Vector2Int oneSquareForward = new Vector2Int(targetPos.x, targetPos.y - directionY);
        if (distanceY == 2 && GameController.GetPiece(oneSquareForward) != null) return false;
        if (distanceX == 0 && distanceY == 2 && !piece.hasMoved)
        {
            hasMovedTwiceLastTurn = true;
            return true;
        }
        else hasMovedTwiceLastTurn = false;
        if (distanceX == 0 && distanceY == 1) return true;
        return false;
    }
    
    public override List<Vector2Int> GetAttackingSquares(Piece piece)
    {
        List<Vector2Int> attackingSquares = new List<Vector2Int>();
        int directionY = piece.isWhite ? 1 : -1;
        for (int i = -1; i <= 1; i += 2)
        {
            Vector2Int square = new Vector2Int(piece.position.x + i, piece.position.y + directionY);
            bool isOutsideBoard  = square.x < 0 || square.x > 7 || square.y < 0 || square.y > 7;
            if (isOutsideBoard) continue;
            attackingSquares.Add(square);
        }
        return attackingSquares;
    }

    public bool CanEnPassant(Vector2Int targetPos, Piece piece)
    {
        int distanceX = targetPos.x - piece.position.x;
        int deltaDistanceX = Mathf.Abs(distanceX);
        int distanceY = targetPos.y - piece.position.y;
        int directionX = Mathf.Clamp(targetPos.x - piece.position.x, -1, 1);

        Vector2Int otherPawnPosition = new(piece.position.x + directionX, piece.position.y);
        PieceController otherPawn = GameController.GetPiece(otherPawnPosition);
        if (!piece.isWhite) distanceY *= -1;
        if (deltaDistanceX != 1 || distanceY != 1) return false;
        if (otherPawn == null || otherPawn.piece is not PawnController pawnController) return false;
        if (otherPawn.piece.isWhite == piece.isWhite) return false;
        if (pawnController.hasMovedTwiceLastTurn == false) return false;
        return true;
    }
    
    public override List<Movement> GetLegalMoves(PieceController piece)
    {
        List<Movement> legalMovements = new List<Movement>();
        int directionY = piece.piece.isWhite ? 1 : -1;
        for (int i = -1; i <= 1; i++)
        {
            Vector2Int square = new Vector2Int(piece.piece.position.x + i, piece.piece.position.y + directionY);
            Movement move = Movement.GetMovement(piece, square);
            if (move != null) legalMovements.Add(move);
        }
        
        Vector2Int pos = new Vector2Int(piece.piece.position.x, piece.piece.position.y + directionY * 2);
        Movement move2 = Movement.GetMovement(piece, pos);
        if (move2 != null) legalMovements.Add(move2);
        
        return legalMovements;
    }
}
