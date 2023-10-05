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
            if (GameController.pieces[checkPos.x, checkPos.y] != null) return false;
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
            bool isOutsideBoard = checkPos.x > 7 || checkPos.y > 7 || checkPos.x < 0 || checkPos.y < 0;
            if (isOutsideBoard) break;
            attackingSquares.Add(checkPos);
        } while (GameController.pieces[checkPos.x, checkPos.y] != null);
        return attackingSquares;
    }
}
