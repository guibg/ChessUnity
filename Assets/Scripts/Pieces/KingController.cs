using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingController : IPiece
{
    public KingController(bool isWhite, Vector2Int position) : base(isWhite, position) { }

    public override bool CanMove(Vector2Int targetPos, IPiece piece, bool forced = false)
    {
        if (forced) return true;
        return isMoveLegal(targetPos, piece);
    }
    public override bool isMoveLegal(Vector2Int targetPos, IPiece piece)
    {
        int distanceX = Mathf.Abs(targetPos.x - piece.position.x);
        int distanceY = Mathf.Abs(targetPos.y - piece.position.y);
        if (distanceX <= 1 && distanceY <= 1) return true;
        return false;
    }

    public override List<Vector2Int> GetAttackingSquares(IPiece piece)
    {
        List<Vector2Int> attackingSquares = new List<Vector2Int>();
        for (int x = piece.position.x - 1; x <= piece.position.x + 1; x++)
        {
            for (int y = piece.position.y - 1; y <= piece.position.y + 1; y++)
            {
                if (x == piece.position.x && y == piece.position.y) continue;
                bool isOutsideBoard = x < 0 || x > 7 || y < 0 || y > 7;
                if (isOutsideBoard) continue;
                attackingSquares.Add(new Vector2Int(x, y));
            }
        }
        return attackingSquares;
    }

    public bool CanCastle(Vector2Int targetPos, IPiece king, IPiece rook)
    {
        //TODO: Add check for check
        int distanceX = Mathf.Abs(targetPos.x - king.position.x);
        int distanceToRook = Mathf.Abs(rook.position.x - king.position.x);
        int distanceY = Mathf.Abs(targetPos.y - king.position.y);
        int directionX = Mathf.Clamp(targetPos.x - king.position.x, -1, 1);
        if (king.hasMoved || rook.hasMoved || distanceY > 0 || distanceX < 2) return false;
        int checkPosX = king.position.x;
        for (int i = 1; i < distanceToRook; i++)
        {
            checkPosX += directionX;
            Vector2Int checkPos = new Vector2Int(checkPosX, king.position.y);
            if (GameController.GetPiece(checkPos) != null) return false;
        }
        return true;
    }
}
