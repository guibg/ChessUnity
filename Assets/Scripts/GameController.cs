using System.Collections.Generic;
using UnityEngine;

public static class GameController
{
    private static PieceController[,] pieces = new PieceController[8, 8];
    private static List<PieceController> piecesList = new();
    public static bool isWhiteTurn = true;
    
    private static HashSet<Vector2Int> whiteAttackingSquares = new();
    private static HashSet<Vector2Int> blackAttackingSquares = new();
    
    public static int whiteLegalMoves = 0;
    public static int blackLegalMoves = 0;

    public static PieceController GetPiece(Vector2Int position)
    {
        return pieces[position.x, position.y];
    }
    
    public static void SetPiece(PieceController piece, Vector2Int position)
    {
        pieces[position.x, position.y] = piece;
        piecesList.Add(piece);
    }
    
    public static void RemovePiece(Vector2Int position)
    {
        var piece = pieces[position.x, position.y];
        piecesList.Remove(piece);
        pieces[position.x, position.y] = null;
        piece.DestroyPiece();
    }
    
    public static void UpdateGameState()
    {
        UpdateAttackingSquares();
    }
    
    public static void UpdateAttackingSquares()
    {
        whiteAttackingSquares.Clear();
        blackAttackingSquares.Clear();
        foreach (PieceController pieceCon in pieces)
        {
            if (pieceCon == null || pieceCon.piece.isWhite == isWhiteTurn) continue;
            
            List<Vector2Int> attackingSquares = pieceCon.piece.GetAttackingSquares(pieceCon.piece);
            
            if (pieceCon.piece.isWhite) whiteAttackingSquares.UnionWith(attackingSquares);
            else blackAttackingSquares.UnionWith(attackingSquares);
        }
    }
    
    public static bool isSquareAttacked(Vector2Int square, bool isWhite)
    {
        return isWhite ? whiteAttackingSquares.Contains(square) : blackAttackingSquares.Contains(square);
    }
}