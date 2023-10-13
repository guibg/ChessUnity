using System.Collections.Generic;
using System.Linq;
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

        bool hasNoLegalMoves = legalMoves.Count == 0;
        if (isWhiteTurn)
        {
            whiteLegalMoves = new(legalMoves);
            bool isInCheck = isSquareAttacked(whiteKingPosition, true);
            if(hasNoLegalMoves && isInCheck) Debug.Log("Checkmate");
            else if(hasNoLegalMoves) Debug.Log("Stalemate");
        }
        else
        {
            blackLegalMoves = new(legalMoves);
            bool isInCheck = isSquareAttacked(blackKingPosition, false);
            if(hasNoLegalMoves && isInCheck) Debug.Log("Checkmate");
            else if(hasNoLegalMoves) Debug.Log("Stalemate");
        }
    }
    
    private static List<Movement> RemoveMovesThatResultInCheck(List<Movement> legalMovesWithChecks)
    {
        List<Movement> legalMoves = new(legalMovesWithChecks);
        foreach (Movement move in legalMovesWithChecks)
        {
            Piece pieceBeforeMovement = move.pieceCon.piece.Copy();
            move.ExecuteMovement(isSimulated:true);
            UpdateAttackingSquares();
            if (isWhiteTurn && isSquareAttacked(whiteKingPosition, true) ||
                !isWhiteTurn && isSquareAttacked(blackKingPosition, false))
            {
                legalMoves.Remove(move);
            }
            UndoMovement(move, pieceBeforeMovement);
        }
        return legalMoves;
    }
    
    public static void UndoMovement(Movement move, Piece pieceBeforeMovement)
    {
        move.pieceCon.Move(pieceBeforeMovement.position);
        move.pieceCon.piece = pieceBeforeMovement;
        if (move.targetPiece != null)
        {
            Piece piece = move.targetPiece.piece;
            CreatePiece.Instance.Instantiate(piece);
        }
        if(move is CastleMovement castleMove)
        {
            Vector2Int rookOriginalPosition;
            if (castleMove.isLeft) rookOriginalPosition = new Vector2Int(0, castleMove.targetPos.y);
            else rookOriginalPosition = new Vector2Int(7, castleMove.targetPos.y);
            castleMove.rook.Move(rookOriginalPosition);
            castleMove.rook.piece.hasMoved = false;
        }
    }
    
    public static bool isSquareAttacked(Vector2Int square, bool isWhite)
    {
        return isWhite ? whiteAttackingSquares.Contains(square) : blackAttackingSquares.Contains(square);
    }
    
    public static bool isMoveLegal(Movement move)
    {
        if (isWhiteTurn) return whiteLegalMoves.Any(movee => movee == move);
        else return blackLegalMoves.Any(movee => movee == move);
    }
}