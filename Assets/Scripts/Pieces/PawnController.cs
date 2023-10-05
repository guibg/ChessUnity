using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnController : IPiece
{
    public bool hasMovedTwiceLastTurn = false;
    
    public PawnController(bool isWhite, Vector2Int position) : base(isWhite, position) { }
    
    public override bool CanMove(Vector2Int targetPos, IPiece piece, bool forced = false)
    {
        if (!forced) if (!isMoveLegal(targetPos, piece)) return false;
        return true;
    }
    public override bool CanTake(Vector2Int targetPos, IPiece piece)
    {
        int distanceX = Mathf.Abs(targetPos.x - piece.position.x);
        int distanceY = targetPos.y - piece.position.y;
        if (!piece.isWhite) distanceY *= -1;
        if (distanceX == 1 && distanceY == 1) return true;
        return false;
    }
    public override bool isMoveLegal(Vector2Int targetPos, IPiece piece)
    {
        int distanceX = targetPos.x - piece.position.x;
        int distanceY = targetPos.y - piece.position.y;
        int directionY = piece.isWhite ? 1 : -1;
        if (!piece.isWhite) distanceY *= -1;

        if (distanceY == 2 && GameController.pieces[targetPos.x, targetPos.y - directionY] != null) return false;
        if (distanceY == 2 && !piece.hasMoved)
        {
            hasMovedTwiceLastTurn = true;
            return true;
        }
        else hasMovedTwiceLastTurn = false;
        if (distanceX == 0 && distanceY == 1) return true;
        return false;
    }
    
    public override List<Vector2Int> GetAttackingSquares(IPiece piece)
    {
        List<Vector2Int> attackingSquares = new List<Vector2Int>();
        int directionY = piece.isWhite ? 1 : -1;
        int directionX = 1;
        for (int i = -1; i <= 1; i += 2)
        {
            Vector2Int square = new Vector2Int(piece.position.x + i, piece.position.y + directionY);
            if (square.x < 0 || square.x > 7 || square.y < 0 || square.y > 7) continue;
            attackingSquares.Add(square);
        }
        return attackingSquares;
    }

    public bool CanEnPassant(Vector2Int targetPos, IPiece piece)
    {
        int distanceX = targetPos.x - piece.position.x;
        int deltaDistanceX = Mathf.Abs(distanceX);
        int distanceY = targetPos.y - piece.position.y;
        int directionX = Mathf.Clamp(targetPos.x - piece.position.x, -1, 1);
        int directionY = piece.isWhite ? 1 : -1;

        PieceController otherPawn = GameController.pieces[piece.position.x + directionX, piece.position.y];
        if (!piece.isWhite) distanceY *= -1;
        if (deltaDistanceX != 1 || distanceY != 1) return false;
        if (otherPawn == null || otherPawn.piece.type != PieceType.Pawn) return false;
        if (otherPawn.piece.isWhite == piece.isWhite) return false;
        if ((otherPawn.piece as PawnController).hasMovedTwiceLastTurn == false) return false;
        return true;
    }
}
