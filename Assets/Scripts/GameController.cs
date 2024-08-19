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
    
    private static HashSet<Movement> whiteLegalMoves = new();
    private static HashSet<Movement> blackLegalMoves = new();
    
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
        List<Movement> legalMoves = GetCurrentLegalMoves();
        CheckForCheckmate(legalMoves);
    }

    private static List<Movement> GetCurrentLegalMoves()
    {
        List<Movement> legalMoves = new();
        foreach (PieceController pieceCon in piecesList)
        {
            bool isEnemyPiece = pieceCon.piece.isWhite != isWhiteTurn;
            if (isEnemyPiece) continue;
            
            List<Movement> pieceLegalMoves = pieceCon.piece.GetLegalMoves(pieceCon);
         
            legalMoves.AddRange(pieceLegalMoves);
        }
        legalMoves = RemoveMovesThatResultInCheck(legalMoves);
        return legalMoves;
    }
    
    private static List<Movement> GetBoardLegalMoves(PieceController[,] board, bool isWhiteTurn)
    {
        List<Movement> legalMoves = new();
        foreach (PieceController pieceCon in board)
        {
            if (pieceCon == null) continue;
            bool isEnemyPiece = pieceCon.piece.isWhite != isWhiteTurn;
            if (isEnemyPiece) continue;
            
            List<Movement> pieceLegalMoves = pieceCon.piece.GetLegalMoves(pieceCon);
         
            legalMoves.AddRange(pieceLegalMoves);
        }
        return legalMoves;
    }

    private static void CheckForCheckmate(List<Movement> legalMoves)
    {
        bool hasNoLegalMoves = legalMoves.Count == 0;
        bool isInCheck;
        if (isWhiteTurn)
        {
            whiteLegalMoves = new HashSet<Movement>(legalMoves);
            isInCheck = isSquareAttacked(whiteKingPosition, true);
        }
        else
        {
            blackLegalMoves = new HashSet<Movement>(legalMoves);
            isInCheck = isSquareAttacked(blackKingPosition, false);
        }
        if(isInCheck && hasNoLegalMoves) Debug.Log("Checkmate");
        else if(hasNoLegalMoves) Debug.Log("Stalemate");
    }
    
    private static List<Movement> RemoveMovesThatResultInCheck(List<Movement> legalMovesWithChecks)
    {
        List<Movement> legalMoves = new(legalMovesWithChecks);
        foreach (Movement move in legalMovesWithChecks)
        {
            Piece pieceBeforeMovement = move.pieceCon.piece.Copy();
            move.ExecuteMovement(isSimulated:true);
            UpdateAttackingSquares();
            if (isKingInCheck()) legalMoves.Remove(move);
            UndoMovement(move, pieceBeforeMovement);
        }
        return legalMoves;
    }
    
    private static void UndoMovement(Movement move, Piece pieceBeforeMovement)
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

    private static bool isKingInCheck()
    {
        bool isWhiteKingInCheck = isWhiteTurn && isSquareAttacked(whiteKingPosition, true);
        bool isBlackKingInCheck = !isWhiteTurn && isSquareAttacked(blackKingPosition, false);
        return isWhiteKingInCheck ||  isBlackKingInCheck;
    }

public static void CalculateFuturePossibleMoves(int depth, bool isWhiteTurn = true)
{
    if (depth <= 0) return;

    List<Movement> legalMoves = GetCurrentLegalMoves();
    foreach (Movement move in legalMoves)
    {
        Piece pieceBeforeMovement = move.pieceCon.piece.Copy();
        move.ExecuteMovement(isSimulated: true);
        UpdateAttackingSquares();
        
        if (!isKingInCheck())
        {
            if (depth > 1)
            {
                CalculateFuturePossibleMoves(depth - 1, !isWhiteTurn);
            }
            else
            {
                PieceController[,] board = GetBoardAfterMove(move);
                List<Movement> futureMoves = GetBoardLegalMoves(board, !isWhiteTurn);
                Debug.Log("Future moves: " + futureMoves.Count);
                Debug.Log("Depth reached");
            }
        }
        
        UndoMovement(move, pieceBeforeMovement);
    }
}
    
    private static PieceController[,] GetBoardAfterMove(Movement move)
    {
        PieceController[,] board = new PieceController[8,8];
        foreach (PieceController pieceCon in piecesList)
        {
            board[pieceCon.piece.position.x, pieceCon.piece.position.y] = pieceCon;
        }
        board[move.pieceCon.piece.position.x, move.pieceCon.piece.position.y] = null;
        board[move.targetPos.x, move.targetPos.y] = move.pieceCon;
        if (move.targetPiece != null) board[move.targetPiece.piece.position.x, move.targetPiece.piece.position.y] = null;
        if (move is CastleMovement castleMove)
        {
            board[castleMove.rook.piece.position.x, castleMove.rook.piece.position.y] = null;
            // board[castleMove.rook.rook.position.x, castleMove.rook.rook.position.y] = castleMove.rook;
        }
        return board;
    }
    
}