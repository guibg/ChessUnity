using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BishopController : IPiece
{
    public BishopController(bool isWhite, Vector2Int position) : base(isWhite, position) { }

    public override bool CanMove(Vector2Int targetPos, IPiece piece, bool forced = false)
    {
        if (forced) return true;
        return isMoveLegal(targetPos, piece);
    }
    public override bool isMoveLegal(Vector2Int targetPos, IPiece piece)
    {
        int distanceX = Mathf.Abs(targetPos.x - piece.position.x);
        int distanceY = Mathf.Abs(targetPos.y - piece.position.y);
        int directionX = Mathf.Clamp(targetPos.x - piece.position.x, -1, 1);
        int directionY = Mathf.Clamp(targetPos.y - piece.position.y, -1, 1);
        Vector2Int checkPos = piece.position;
        for (int i = 1; i < distanceX; i++)
        {
            checkPos.x += directionX;
            checkPos.y += directionY;
            if (GameController.GetPiece(checkPos) != null) return false;
        }
        if (distanceX == distanceY) return true;
        return false;
    }
    
    public override List<Vector2Int> GetAttackingSquares(IPiece piece)
    {
        List<Vector2Int> attackingSquares = new List<Vector2Int>();
        attackingSquares.AddRange(GetDiagonalAttackingSquares(piece, 1, 1));
        attackingSquares.AddRange(GetDiagonalAttackingSquares(piece, 1, -1));
        attackingSquares.AddRange(GetDiagonalAttackingSquares(piece, -1, 1));
        attackingSquares.AddRange(GetDiagonalAttackingSquares(piece, -1, -1));
        return attackingSquares;
    }
    
    private List<Vector2Int> GetDiagonalAttackingSquares(IPiece piece, int directionX, int directionY)
    {
        List<Vector2Int> attackingSquares = new List<Vector2Int>();
        Vector2Int checkPos = piece.position;
        do
        {
            checkPos.x += directionX;
            checkPos.y += directionY;
            if (MapBounds.isPositionOutsideBoard(checkPos)) break;
            attackingSquares.Add(checkPos);
        } while (GameController.GetPiece(checkPos) == null);
        return attackingSquares;
    }
    
    public override List<Movement> GetLegalMoves(PieceController piece)
    {
        List<Movement> attackingSquares = new List<Movement>();
        attackingSquares.AddRange(GetLegalMovesInOneDiagonal(piece, 1, 1));
        attackingSquares.AddRange(GetLegalMovesInOneDiagonal(piece, 1, -1));
        attackingSquares.AddRange(GetLegalMovesInOneDiagonal(piece, -1, 1));
        attackingSquares.AddRange(GetLegalMovesInOneDiagonal(piece, -1, -1));
        return attackingSquares;
    }
    
    private List<Movement> GetLegalMovesInOneDiagonal(PieceController piece, int directionX, int directionY)
    {
        List<Movement> legalMovements = new List<Movement>();
        Vector2Int checkPos = piece.piece.position;
        do
        {
            checkPos.x += directionX;
            checkPos.y += directionY;
            if (MapBounds.isPositionOutsideBoard(checkPos)) break;
            Movement move = Movement.GetMovement(piece, checkPos);
            if (move != null) legalMovements.Add(move);
        } while (GameController.GetPiece(checkPos) == null);
        return legalMovements;
    }
}
