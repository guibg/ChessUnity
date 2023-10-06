using UnityEngine;

public class Movement
{
    protected PieceController pieceCon;
    protected Vector2Int targetPos;

    public Movement(PieceController pieceCon, Vector2Int targetPos)
    {
        this.pieceCon = pieceCon;
        this.targetPos = targetPos;
    }
    
    public virtual void ExecuteMovement(bool changeTurn = true)
    {
        var targetPiece = GameController.GetPiece(targetPos);
        if (targetPiece != null) targetPiece.DestroyPiece();
        if (changeTurn) GameController.isWhiteTurn = !GameController.isWhiteTurn;
        pieceCon.Move(targetPos);
        pieceCon.piece.hasMoved = true;
        if (pieceCon.piece.type == PieceType.Pawn && targetPos.y is 0 or 7) pieceCon.Promote();
        GameController.UpdateGameState();
    }
    
    public static Movement GetMovement(PieceController pieceCon, Vector2Int targetPos)
    {
        Movement move = CheckForCastle(pieceCon, targetPos);
        if(move != null) return move;
        move = CheckForEnPassant(pieceCon, targetPos);
        if(move != null) return move;
        move = CheckForNormalMove(pieceCon, targetPos);
        if(move != null) return move;
        return null;
    }

    private static Movement CheckForCastle(PieceController pieceCon, Vector2Int targetPos)
    {
        if (pieceCon.piece is not KingController kingController) return null;
        IPiece piece = pieceCon.piece;
        bool isLeft = targetPos.x < piece.position.x;
        int rookX = isLeft ? 0 : 7;
        Vector2Int rookPosition = new(rookX, targetPos.y);
        PieceController rook = GameController.GetPiece(rookPosition);
        if (kingController.CanCastle(targetPos, piece, rook.piece))
        {
            //return new CastleMovement(rook, isLeft);
        }
        return null;
    }

    private static Movement CheckForEnPassant(PieceController pieceCon, Vector2Int targetPos)
    {
        if (pieceCon.piece is not PawnController pawnController) return null;
        if (pawnController.CanEnPassant(targetPos, pieceCon.piece))
        {
            //return new EnPassantMovement(targetPos);
        }
        return null;
    }
    
    private static Movement CheckForNormalMove(PieceController pieceCon, Vector2Int targetPos)
    {
        //TODO: Check if the move is legal
        if (pieceCon.piece.CanMove(targetPos, pieceCon.piece))
        {
            return new Movement(pieceCon, targetPos);
        }
        return null;
    }
}