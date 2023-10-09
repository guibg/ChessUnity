using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPiece
{
    public bool isWhite;
    public PieceType type => GetPieceType();
    public Vector2Int position;
    public bool hasMoved = false;

    public IPiece(bool isWhite, Vector2Int position)
    {
        this.isWhite = isWhite;
        this.position = position;
    }
    
    public abstract bool CanMove(Vector2Int targetPos, IPiece piece, bool forced = false);
    public virtual bool CanTake(Vector2Int targetPos, IPiece piece) { return CanMove(targetPos, piece); }
    public abstract bool isMoveLegal(Vector2Int targetPos, IPiece piece);
    public abstract List<Vector2Int> GetAttackingSquares(IPiece piece);
    public abstract List<Movement> GetLegalMoves(PieceController piece);
    private PieceType GetPieceType()
    {
        return this switch
        {
            PawnController => PieceType.Pawn,
            RookController => PieceType.Rook,
            KnightController => PieceType.Knight,
            BishopController => PieceType.Bishop,
            QueenController => PieceType.Queen,
            KingController => PieceType.King,
            _ => PieceType.Pawn
        };
    }
}

public enum PieceType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}
