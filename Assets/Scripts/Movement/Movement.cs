﻿using UnityEngine;

public class Movement
{
    public PieceController pieceCon;
    public PieceController targetPiece;
    public Vector2Int targetPos;

    protected Movement(PieceController pieceCon, Vector2Int targetPos, PieceController targetPiece = null)
    {
        this.pieceCon = pieceCon;
        this.targetPos = targetPos;
        this.targetPiece = targetPiece == null ? GameController.GetPiece(targetPos) : targetPiece;
    }
    
    public virtual void ExecuteMovement(bool isSimulated = false)
    {
        if (targetPiece != null) GameController.RemovePiece(targetPiece.piece.position);
        pieceCon.Move(targetPos);
        pieceCon.piece.hasMoved = true;
        if (pieceCon.piece.type == PieceType.Pawn && targetPos.y is 0 or 7) pieceCon.Promote();
        if (isSimulated) return;
        GameController.isWhiteTurn = !GameController.isWhiteTurn;
        GameController.UpdateGameState();
    }
    
    public static Movement GetMovement(PieceController pieceCon, Vector2Int targetPos)
    {
        if(MapBounds.isPositionOutsideBoard(targetPos)) return null;
        Movement move = CheckForCastle(pieceCon, targetPos);
        if(move is not null) return move;
        move = CheckForEnPassant(pieceCon, targetPos);
        if(move is not null) return move;
        move = CheckForNormalMove(pieceCon, targetPos);
        if(move is not null) return move;
        return null;
    }

    private static Movement CheckForCastle(PieceController pieceCon, Vector2Int targetPos)
    {
        if (pieceCon.piece is not KingController kingController) return null;
        Piece piece = pieceCon.piece;
        bool isLeft = targetPos.x < piece.position.x;
        int rookX = isLeft ? 0 : 7;
        Vector2Int rookPosition = new(rookX, targetPos.y);
        if(MapBounds.isPositionOutsideBoard(rookPosition)) return null;
        PieceController rook = GameController.GetPiece(rookPosition);
        if (rook != null && kingController.CanCastle(targetPos, piece, rook.piece))
        {
            return new CastleMovement(pieceCon, rook, isLeft);
        }
        return null;
    }

    private static Movement CheckForEnPassant(PieceController pieceCon, Vector2Int targetPos)
    {
        if (pieceCon.piece is not PawnController pawnController) return null;
        if (pawnController.CanEnPassant(targetPos, pieceCon.piece))
        {
            Vector2Int takenPiecePosition = new Vector2Int(targetPos.x, pieceCon.piece.position.y);
            PieceController takenPiece = GameController.GetPiece(takenPiecePosition);
            return new Movement(pieceCon, targetPos, takenPiece);

        }
        return null;
    }
    
    private static Movement CheckForNormalMove(PieceController pieceCon, Vector2Int targetPos)
    {
        Piece piece = pieceCon.piece;
        PieceController targetPiece = GameController.GetPiece(targetPos);
        if (targetPiece != null && targetPiece.piece.isWhite == piece.isWhite) return null;
        if (targetPiece == null && piece.CanMove(targetPos, piece)) return new Movement(pieceCon, targetPos);
        if (targetPiece != null && piece.CanTake(targetPos, piece)) return new Movement(pieceCon, targetPos, targetPiece);
        return null;
    }
    
    public static bool operator ==(Movement move1, Movement move2)
    {
        if (move1 is null && move2 is null) return true;
        if (move1 is null || move2 is null) return false;
        if (move1.pieceCon != move2.pieceCon) return false;
        if (move1.targetPos != move2.targetPos) return false;
        return true;
    }

    public static bool operator !=(Movement move1, Movement move2)
    {
        if (move1 is null && move2 is null) return false;
        if (move1 is null || move2 is null) return true;
        if (move1.pieceCon != move2.pieceCon) return true;
        if (move1.targetPos != move2.targetPos) return true;
        return false;
    }
}