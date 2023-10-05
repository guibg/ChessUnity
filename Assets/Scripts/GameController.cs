using System.Collections.Generic;
using UnityEngine;

public static class GameController
{
    public static PieceController[,] pieces = new PieceController[8, 8];
    public static bool isWhiteTurn = true;
    
    private static HashSet<Vector2Int> whiteAttackingSquares = new();
    private static HashSet<Vector2Int> blackAttackingSquares = new();
    
    public static void UpdateAttackingSquares()
    {
        whiteAttackingSquares.Clear();
        blackAttackingSquares.Clear();
        foreach (PieceController pieceCon in pieces)
        {
            if (pieceCon == null) continue;
            
            List<Vector2Int> attackingSquares = pieceCon.piece.GetAttackingSquares(pieceCon.piece);
            
            if (pieceCon.piece.isWhite) whiteAttackingSquares.UnionWith(attackingSquares);
            else blackAttackingSquares.UnionWith(attackingSquares);
        }
    }
    
    public static bool isSquareAttacked(Vector2Int square, bool isWhite)
    {
        if (isWhite) return whiteAttackingSquares.Contains(square);
        else return blackAttackingSquares.Contains(square);
    }
}