using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class KnightController : IPiece
{
    public KnightController(bool isWhite, Vector2Int position) : base(isWhite, position) { }

    public override bool CanMove(Vector2Int targetPos, IPiece piece, bool forced = false)
    {
        if (!forced) if (!isMoveLegal(targetPos, piece)) return false;
        return true;
    }
    
    public override bool isMoveLegal(Vector2Int targetPos, IPiece piece)
    {
        int distanceX = Mathf.Abs(targetPos.x - piece.position.x);
        int distanceY = Mathf.Abs(targetPos.y - piece.position.y);
        if (distanceX + distanceY == 3 && distanceX != 0 && distanceY != 0) return true;
        return false;
    }
    
    public override List<Vector2Int> GetAttackingSquares(IPiece piece)
    {
        Vector2Int pos = piece.position;
        List<Vector2Int> attackingSquares = new List<Vector2Int>()
        {
            new Vector2Int(pos.x + 1, pos.y + 2),
            new Vector2Int(pos.x + 2, pos.y + 1),
            new Vector2Int(pos.x + 2, pos.y - 1),
            new Vector2Int(pos.x + 1, pos.y - 2),
            new Vector2Int(pos.x - 1, pos.y - 2),
            new Vector2Int(pos.x - 2, pos.y - 1),
            new Vector2Int(pos.x - 2, pos.y + 1),
            new Vector2Int(pos.x - 1, pos.y + 2)
        };
        attackingSquares = RemoveWrongPos(attackingSquares);
        return attackingSquares;
    }
    
    private List<Vector2Int> RemoveWrongPos(List<Vector2Int> attackingSquares)
    {
        List<Vector2Int> removeList = new List<Vector2Int>();
        foreach (var pos in attackingSquares)
        {
            bool isInsideBoard = pos.x < 8 && pos.y < 8 && pos.x >= 0 && pos.y >= 0;
            if (isInsideBoard) continue;
            attackingSquares.Remove(pos);
        }
        return removeList;
    }
}
