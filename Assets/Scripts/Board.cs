using UnityEngine;

public class Board : Singleton<Board>
{
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private Color _color1;
    [SerializeField] private Color _color2;
    void Start()
    {
        InstantiateBoard();
        InstantiateInitialPieces();
    }

    private void InstantiateBoard()
    {
        float scaleX = _tilePrefab.transform.localScale.x / 2;
        float scaleY = _tilePrefab.transform.localScale.y / 2;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (x == 0 && y == 0) MapBounds.min = new Vector2(x - scaleX, y - scaleY);
                if (x == 7 && y == 7) MapBounds.max = new Vector2(x + scaleX - 0.01f, y + scaleY - 0.01f);
                GameObject tile = Instantiate(_tilePrefab, transform);
                tile.transform.position = new Vector3(x, y, 0);
                tile.name = $"Tile {x} {y}";
                tile.GetComponent<SpriteRenderer>().color = (x + y) % 2 == 0 ? _color1 : _color2;
            }
        }
    }

    private void InstantiateInitialPieces()
    {
        for (int x = 0; x < 8; x++)
        {
            CreatePiece.Instance.Instantiate(new PawnController(true, new Vector2Int(x, 1)));
            CreatePiece.Instance.Instantiate(new PawnController(false, new Vector2Int(x, 6)));
        }
        CreatePiece.Instance.Instantiate(new RookController(true, new Vector2Int(0, 0)));
        CreatePiece.Instance.Instantiate(new RookController(true, new Vector2Int(7, 0)));
        CreatePiece.Instance.Instantiate(new RookController(false, new Vector2Int(0, 7)));
        CreatePiece.Instance.Instantiate(new RookController(false, new Vector2Int(7, 7)));
        CreatePiece.Instance.Instantiate(new KnightController(true, new Vector2Int(1, 0)));
        CreatePiece.Instance.Instantiate(new KnightController(true, new Vector2Int(6, 0)));
        CreatePiece.Instance.Instantiate(new KnightController(false, new Vector2Int(1, 7)));
        CreatePiece.Instance.Instantiate(new KnightController(false, new Vector2Int(6, 7)));
        CreatePiece.Instance.Instantiate(new BishopController(true, new Vector2Int(2, 0)));
        CreatePiece.Instance.Instantiate(new BishopController(true, new Vector2Int(5, 0)));
        CreatePiece.Instance.Instantiate(new BishopController(false, new Vector2Int(2, 7)));
        CreatePiece.Instance.Instantiate(new BishopController(false, new Vector2Int(5, 7)));
        CreatePiece.Instance.Instantiate(new QueenController(true, new Vector2Int(3, 0)));
        CreatePiece.Instance.Instantiate(new QueenController(false, new Vector2Int(3, 7)));
        CreatePiece.Instance.Instantiate(new KingController(true, new Vector2Int(4, 0)));
        CreatePiece.Instance.Instantiate(new KingController(false, new Vector2Int(4, 7)));
    }
}
