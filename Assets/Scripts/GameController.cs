using System.Collections.Generic;
using UnityEngine;

public static class GameController
{
    private static PieceController[,] pieces = new PieceController[8, 8];
    private static List<PieceController> piecesList = new();
    public static bool isWhiteTurn = true;
    private static Vector2Int whiteKingPosition = new();
    private static Vector2Int blackKingPosition = new();
    
    private static HashSet<Vector2Int> whiteAttackingSquares = new();
    private static HashSet<Vector2Int> blackAttackingSquares = new();
    
    public static HashSet<Movement> whiteLegalMoves = new();
    public static HashSet<Movement> blackLegalMoves = new();
    
    public static PieceController GetPiece(Vector2Int position)
    {
        return pieces[position.x, position.y];
    }
    
    public static void SetPiece(PieceController piece, Vector2Int position)
    {
        pieces[position.x, position.y] = piece;
        if (piece != null && piece.piece.type == PieceType.King)
        {
            if (piece.piece.isWhite) whiteKingPosition = position;
            else blackKingPosition = position;
        }
    }
    
    public static void AddPiece(PieceController piece, Vector2Int position)
    {
        piecesList.Add(piece);
        SetPiece(piece, position);
    }
    
    public static void RemovePiece(Vector2Int position)
    {
        PieceController piece = pieces[position.x, position.y];
        piecesList.Remove(piece);
        pieces[position.x, position.y] = null;
        piece.DestroyPiece();
    }
    
    public static void UpdateGameState()
    {
        UpdateAttackingSquares();
        UpdateLegalMoves();
    }
    
    private static void UpdateAttackingSquares()
    {
        whiteAttackingSquares.Clear();
        blackAttackingSquares.Clear();
        List<Vector2Int> attackingSquares = new();
        foreach (PieceController pieceCon in piecesList)
        {
            bool isAlliedPiece = pieceCon.piece.isWhite == isWhiteTurn;
            if (isAlliedPiece) continue;
            
            List<Vector2Int> pieceAttackingSquares = pieceCon.piece.GetAttackingSquares(pieceCon.piece);
            
            attackingSquares.AddRange(pieceAttackingSquares);
        }
        if (isWhiteTurn) whiteAttackingSquares = new(attackingSquares);
        else blackAttackingSquares = new(attackingSquares);
    }
    
    private static void UpdateLegalMoves()
    {
        whiteLegalMoves.Clear();
        blackLegalMoves.Clear();
        List<Movement> legalMoves = new();
        foreach (PieceController pieceCon in piecesList)
        {
            bool isEnemyPiece = pieceCon.piece.isWhite != isWhiteTurn;
            if (isEnemyPiece) continue;
            
            List<Movement> pieceLegalMoves = pieceCon.piece.GetLegalMoves(pieceCon);
         
            legalMoves.AddRange(pieceLegalMoves);
        }
        legalMoves = RemoveMovesThatResultInCheck(legalMoves);
        
        if (isWhiteTurn) whiteLegalMoves = new(legalMoves);
        else blackLegalMoves = new(legalMoves);
        
        Debug.Log("White legal moves: " + whiteLegalMoves.Count);
        foreach (var move in whiteLegalMoves)
        {
            Debug.Log(move.pieceCon + " " + move.targetPos);
        }
        Debug.Log("Black legal moves: " + blackLegalMoves.Count);
        foreach (var move in blackLegalMoves)
        {
            Debug.Log(move.pieceCon + " " + move.targetPos);
        }
    }
    
    private static List<Movement> RemoveMovesThatResultInCheck(List<Movement> legalMovesWithChecks)
    {
        List<Movement> legalMoves = new(legalMovesWithChecks);
        foreach (Movement move in legalMovesWithChecks)
        {
            IPiece pieceBeforeMovement = move.pieceCon.piece;
            move.ExecuteMovement(isSimulated:true);
            Debug.Log(move.pieceCon.GetHashCode() + " 1");
            if (isWhiteTurn && isSquareAttacked(whiteKingPosition, true) ||
                !isWhiteTurn && isSquareAttacked(blackKingPosition, false))
            {
                legalMoves.Remove(move);
            }
            UndoMovement(move, pieceBeforeMovement);
        }
        return legalMoves;
    }
    
    private static void UndoMovement(Movement move, IPiece pieceBeforeMovement)
    {
        move.pieceCon.piece = pieceBeforeMovement;
        Debug.Log(move.pieceCon.GetHashCode() + " 2");
        move.pieceCon.Move(move.pieceCon.piece.position);
        if (move.targetPiece != null) AddPiece(move.targetPiece, move.targetPiece.piece.position);
        isWhiteTurn = !isWhiteTurn;
        UpdateAttackingSquares();
    }
    
    public static bool isSquareAttacked(Vector2Int square, bool isWhite)
    {
        return isWhite ? whiteAttackingSquares.Contains(square) : blackAttackingSquares.Contains(square);
    }
}