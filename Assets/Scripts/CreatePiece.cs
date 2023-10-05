using UnityEngine;

public class CreatePiece : Singleton<CreatePiece>
{
    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private Sprite whitePawnSprite;
    [SerializeField] private Sprite blackPawnSprite;
    [SerializeField] private Sprite whiteRookSprite;
    [SerializeField] private Sprite blackRookSprite;
    [SerializeField] private Sprite whiteKnightSprite;
    [SerializeField] private Sprite blackKnightSprite;
    [SerializeField] private Sprite whiteBishopSprite;
    [SerializeField] private Sprite blackBishopSprite;
    [SerializeField] private Sprite whiteQueenSprite;
    [SerializeField] private Sprite blackQueenSprite;
    [SerializeField] private Sprite whiteKingSprite;
    [SerializeField] private Sprite blackKingSprite;

    public GameObject Instantiate(IPiece piece)
    {
        Vector3 piecePos = new Vector3(piece.position.x, piece.position.y, 0);
        GameObject pieceObject = GameObject.Instantiate(piecePrefab, piecePos, Quaternion.identity, transform);
        pieceObject.name = $"Piece {piece.type} {piece.position.x} {piece.position.y}";
        pieceObject.GetComponent<PieceController>().Init(piece);
        pieceObject.GetComponent<SpriteRenderer>().sprite = GetSprite(piece.type, piece.isWhite);
        GameController.pieces[piece.position.x, piece.position.y] = pieceObject.GetComponent<PieceController>();
        return pieceObject;
    }

    public Sprite GetSprite(IPiece piece)
    {
        return piece.type switch
        {
            PieceType.Pawn => piece.isWhite ? whitePawnSprite : blackPawnSprite,
            PieceType.Rook => piece.isWhite ? whiteRookSprite : blackRookSprite,
            PieceType.Knight => piece.isWhite ? whiteKnightSprite : blackKnightSprite,
            PieceType.Bishop => piece.isWhite ? whiteBishopSprite : blackBishopSprite,
            PieceType.Queen => piece.isWhite ? whiteQueenSprite : blackQueenSprite,
            PieceType.King => piece.isWhite ? whiteKingSprite : blackKingSprite,
            _ => null
        };
    }

    public Sprite GetSprite(PieceType type, bool isWhite)
    {
        return type switch
        {
            PieceType.Pawn => isWhite ? whitePawnSprite : blackPawnSprite,
            PieceType.Rook => isWhite ? whiteRookSprite : blackRookSprite,
            PieceType.Knight => isWhite ? whiteKnightSprite : blackKnightSprite,
            PieceType.Bishop => isWhite ? whiteBishopSprite : blackBishopSprite,
            PieceType.Queen => isWhite ? whiteQueenSprite : blackQueenSprite,
            PieceType.King => isWhite ? whiteKingSprite : blackKingSprite,
            _ => null
        };
    }
}