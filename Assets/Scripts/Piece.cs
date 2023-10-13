using System.Collections;
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
            _ => PieceType.Pawn
        };
    }
    
    public Piece Copy1()
    {
        return this switch
        {
            PawnController pawn => new PawnController(pawn.isWhite, pawn.position),
            RookController rook => new RookController(rook.isWhite, rook.position),
            KnightController knight => new KnightController(knight.isWhite, knight.position),
            BishopController bishop => new BishopController(bishop.isWhite, bishop.position),
            QueenController queen => new QueenController(queen.isWhite, queen.position),
            KingController king => new KingController(king.isWhite, king.position),
            _ => new PawnController(isWhite, position)
        };
    }
    
    public Piece Copy()
    {
        Piece piece;
        if (this is PawnController) piece = new PawnController(isWhite, position);
        else if (this is RookController) piece = new RookController(isWhite, position);
        else if (this is KnightController) piece = new KnightController(isWhite, position);
        else if (this is BishopController) piece = new BishopController(isWhite, position);
        else if (this is QueenController) piece = new QueenController(isWhite, position);
        else if (this is KingController) piece = new KingController(isWhite, position);
        else piece = new PawnController(isWhite, position);
        piece.hasMoved = hasMoved;
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
