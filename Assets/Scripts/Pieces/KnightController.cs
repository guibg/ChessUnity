using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class KnightController : Piece
{
    public KnightController(bool isWhite, Vector2Int position) : base(isWhite, position) { }

    public override bool CanMove(Vector2Int targetPos, Piece piece, bool forced = false)
    {
        if (!forced) if (!isMoveLegal(targetPos, piece)) return false;
        return true;
    }
    
    public override bool isMoveLegal(Vector2Int targetPos, Piece piece)
    {
        int distanceX = Mathf.Abs(targetPos.x - piece.position.x);
        int distanceY = Mathf.Abs(targetPos.y - piece.position.y);
        if (distanceX + distanceY == 3 && distanceX != 0 && distanceY != 0) return true;
        return false;
    }
    
    public override List<Vector2Int> GetAttackingSquares(Piece piece)
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
        List<Vector2Int> removeList = new List<Vector2Int>(attackingSquares);
        foreach (var pos in attackingSquares)
        {
            if (MapBounds.isPositionOutsideBoard(pos)) removeList.Remove(pos);
        }
        return removeList;
    }
    
    public override List<Movement> GetLegalMoves(PieceController piece)
    {
        List<Movement> legalMoves = new List<Movement>();
        List<Vector2Int> attackingSquares = GetAttackingSquares(piece.piece);
        foreach (var sq in attackingSquares)
        {
            Movement move = Movement.GetMovement(piece, sq);
            if (move != null) legalMoves.Add(move);
        }
        return legalMoves;
    }
}
