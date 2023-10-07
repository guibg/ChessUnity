using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RookController : IPiece
{
    public RookController(bool isWhite, Vector2Int position) : base(isWhite, position) { }

    public override bool CanMove(Vector2Int targetPos, IPiece piece, bool forced = false)
    {
        if (!forced) if (!isMoveLegal(targetPos, piece)) return false;
        return true;
    }
    public override bool isMoveLegal(Vector2Int targetPos, IPiece piece)
    {
        int distanceX = Mathf.Abs(targetPos.x - piece.position.x);
        int distanceY = Mathf.Abs(targetPos.y - piece.position.y);
        int directionX = Mathf.Clamp(targetPos.x - piece.position.x, -1, 1);
        int directionY = Mathf.Clamp(targetPos.y - piece.position.y, -1, 1);
        int distance = distanceX + distanceY;
        Vector2Int checkPos = piece.position;
        for (int i = 1; i < distance; i++)
        {
            checkPos.x += directionX;
            checkPos.y += directionY;
            if (GameController.GetPiece(checkPos) != null) return false;
        }
        if (distanceX == 0 || distanceY == 0) return true;
        return false;
    }
    
    public override List<Vector2Int> GetAttackingSquares(IPiece piece)
    {
        List<Vector2Int> attackingSquares = new List<Vector2Int>();
        attackingSquares.AddRange(GetStraightAttackingSquares(piece, 1, 0));
        attackingSquares.AddRange(GetStraightAttackingSquares(piece, -1, 0));
        attackingSquares.AddRange(GetStraightAttackingSquares(piece, 0, 1));
        attackingSquares.AddRange(GetStraightAttackingSquares(piece, 0, -1));
        return attackingSquares;
    }
    
    private List<Vector2Int> GetStraightAttackingSquares(IPiece piece, int directionX, int directionY)
    {
        List<Vector2Int> attackingSquares = new List<Vector2Int>();
        Vector2Int checkPos = piece.position;
        do
        {
            checkPos.x += directionX;
            checkPos.y += directionY;
            bool isOutsideBoard = checkPos.x > 7 || checkPos.y > 7 || checkPos.x < 0 || checkPos.y < 0;
            if (isOutsideBoard) break;
            // Debug.Log(checkPos + " " + (GameController.GetPiece(checkPos) != null));
            attackingSquares.Add(checkPos);
        } while (GameController.GetPiece(checkPos) != null);
        return attackingSquares;
    }
}
