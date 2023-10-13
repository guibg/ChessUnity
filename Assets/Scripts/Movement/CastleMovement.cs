using UnityEngine;

public class CastleMovement : Movement
{
    public PieceController rook;
    public bool isLeft;
    
    public CastleMovement(PieceController king, PieceController rook, bool isLeft) 
        : base(king, GetKingTargetPosition(king, isLeft))
    {
        this.rook = rook;
        this.isLeft = isLeft;
    }
    
    private Vector2Int GetRookTargetPosition()
    {
        Vector2Int rookPos = rook.piece.position;
        return new Vector2Int(isLeft ? rookPos.x + 3 : rookPos.x - 2, rookPos.y);
    }
    
    private static Vector2Int GetKingTargetPosition(PieceController king, bool isLeft)
    {
        return new Vector2Int(isLeft ? king.piece.position.x - 2 : king.piece.position.x + 2, king.piece.position.y);
    }
    
    public override void ExecuteMovement(bool isSimulated = false)
    {
        rook.Move(GetRookTargetPosition());
        base.ExecuteMovement(isSimulated: isSimulated);
    }
}