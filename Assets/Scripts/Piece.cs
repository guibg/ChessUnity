using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece
{
    public bool isWhite;
    public PieceType type => GetPieceType();
    public Vector2Int position;
    public bool hasMoved = false;

    public Piece(bool isWhite, Vector2Int position)
    {
        this.isWhite = isWhite;
        this.position = position;
    }

    public abstract bool CanMove(Vector2Int targetPos, Piece piece, bool forced = false);
    public virtual bool CanTake(Vector2Int targetPos, Piece piece) { return CanMove(targetPos, piece); }
    public abstract bool isMoveLegal(Vector2Int targetPos, Piece piece);
    public abstract List<Vector2Int> GetAttackingSquares(Piece piece);
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
            _ => throw new InvalidOperationException("Unknown piece type")
        };
    }

    public Piece Copy()
    {
        Piece piece = (Piece)this.MemberwiseClone();
        piece.hasMoved = this.hasMoved;
        return piece;
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
