using UnityEngine;

public class Movement
{
    protected PieceController pieceCon;
    protected PieceController targetPiece;
    protected Vector2Int targetPos;

    protected Movement(PieceController pieceCon, Vector2Int targetPos, PieceController targetPiece = null)
    {
        this.pieceCon = pieceCon;
        this.targetPos = targetPos;
        this.targetPiece = targetPiece == null ? GameController.GetPiece(targetPos) : targetPiece;
    }
    
    public virtual void ExecuteMovement()
    {
        if (targetPiece != null) GameController.RemovePiece(targetPiece.piece.position);
        GameController.isWhiteTurn = !GameController.isWhiteTurn;
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
        IPiece piece = pieceCon.piece;
        PieceController targetPiece = GameController.GetPiece(targetPos);
        if (targetPiece != null && targetPiece.piece.isWhite == piece.isWhite) return null;
        if (targetPiece == null && piece.CanMove(targetPos, piece)) return new Movement(pieceCon, targetPos);
        if (targetPiece != null && piece.CanTake(targetPos, piece)) return new Movement(pieceCon, targetPos, targetPiece);
        return null;
    }
}