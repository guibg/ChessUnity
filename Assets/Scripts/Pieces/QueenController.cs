using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenController : IPiece
{
    public QueenController(bool isWhite, Vector2Int position) : base(isWhite, position) { }

    public override bool CanMove(Vector2Int targetPos, IPiece piece, bool forced = false)
    {
        if (!forced) if (!isMoveLegal(targetPos, piece)) return false;
        return true;
    }
    public override bool isMoveLegal(Vector2Int targetPos, IPiece piece)
    {
        int distanceX = Mathf.Abs(targetPos.x - piece.position.x);
        int distanceY = Mathf.Abs(targetPos.y - piece.position.y);
        int directionX = Mathf.Clamp(targetPos.x - piece.position.x, -1, 1);
        int directionY = Mathf.Clamp(targetPos.y - piece.position.y, -1, 1);
        int distance = distanceX == distanceY ? distanceX : distanceX + distanceY;
        
        if (distanceX != 0 && distanceY != 0 && distanceX != distanceY) return false;
        
        Vector2Int checkPos = piece.position;
        for (int i = 1; i < distance; i++)
        {
            checkPos.x += directionX;
            checkPos.y += directionY;
            if (GameController.GetPiece(checkPos) != null) return false;
        }
        return true;
    }
    
    public override List<Vector2Int> GetAttackingSquares(IPiece piece)
    {
        List<Vector2Int> attackingSquares = new List<Vector2Int>();
        attackingSquares.AddRange(GetAttackingSquaresInOneDirection(piece, 1, 0));
        attackingSquares.AddRange(GetAttackingSquaresInOneDirection(piece, -1, 0));
        attackingSquares.AddRange(GetAttackingSquaresInOneDirection(piece, 0, 1));
        attackingSquares.AddRange(GetAttackingSquaresInOneDirection(piece, 0, -1));
        attackingSquares.AddRange(GetAttackingSquaresInOneDirection(piece, 1, 1));
        attackingSquares.AddRange(GetAttackingSquaresInOneDirection(piece, 1, -1));
        attackingSquares.AddRange(GetAttackingSquaresInOneDirection(piece, -1, 1));
        attackingSquares.AddRange(GetAttackingSquaresInOneDirection(piece, -1, -1));
        return attackingSquares;
    }
    
    private List<Vector2Int> GetAttackingSquaresInOneDirection(IPiece piece, int directionX, int directionY)
    {
        List<Vector2Int> attackingSquares = new List<Vector2Int>();
        Vector2Int checkPos = piece.position;
        do
        {
            checkPos.x += directionX;
            checkPos.y += directionY;
            if (MapBounds.isPositionOutsideBoard(checkPos)) break;
            attackingSquares.Add(checkPos);
        } while (GameController.GetPiece(checkPos) == null);
        return attackingSquares;
    }

    public override List<Movement> GetLegalMoves(PieceController piece)
    {
        List<Movement> legalMoves = new List<Movement>();
        legalMoves.AddRange(GetLegalMovesInOneDirection(piece, 1, 0));
        legalMoves.AddRange(GetLegalMovesInOneDirection(piece, -1, 0));
        legalMoves.AddRange(GetLegalMovesInOneDirection(piece, 0, 1));
        legalMoves.AddRange(GetLegalMovesInOneDirection(piece, 0, -1));
        legalMoves.AddRange(GetLegalMovesInOneDirection(piece, 1, 1));
        legalMoves.AddRange(GetLegalMovesInOneDirection(piece, 1, -1));
        legalMoves.AddRange(GetLegalMovesInOneDirection(piece, -1, 1));
        legalMoves.AddRange(GetLegalMovesInOneDirection(piece, -1, -1));
        return legalMoves;
    }
    
    private List<Movement> GetLegalMovesInOneDirection(PieceController piece, int directionX, int directionY)
    {
        List<Movement> legalMoves = new List<Movement>();
        Vector2Int checkPos = piece.piece.position;
        do
        {
            checkPos.x += directionX;
            checkPos.y += directionY;
            if (MapBounds.isPositionOutsideBoard(checkPos)) break;
            Movement move = Movement.GetMovement(piece, checkPos);
            if (move != null) legalMoves.Add(move);
        } while (true);
        return legalMoves;
    }
    
}
