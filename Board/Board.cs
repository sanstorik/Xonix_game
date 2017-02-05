using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    static Board instance;

    [SerializeField]
    GameObject cellPrefab;

    // land - aqua
    public Sprite[] landTypes;

    BoardCell[,] board;

    public const int BoardCellWidth = 30;
    int boardCellHeight;

    Vector3 cellSpawnStartingPosition;
    Vector2 deviceSize;
    float boardRightX;

    float cellWidth;
    float cellHeight;

    private void Awake()
    {
        instance = this;

        deviceSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        float widthWithoutPanelInPercents = (1280f - 1125f) / 1280;
        boardRightX = Camera.main.ScreenToWorldPoint(Vector3.right * (Screen.width * widthWithoutPanelInPercents)).x;

        cellWidth = (Mathf.Abs(deviceSize.x) + Mathf.Abs(boardRightX)) / BoardCellWidth;

        boardCellHeight = Mathf.RoundToInt(deviceSize.y * 2 / cellWidth);

        cellHeight = Mathf.Abs(deviceSize.y * 2) / boardCellHeight;
        cellSpawnStartingPosition = new Vector3(-deviceSize.x + cellWidth / 2, deviceSize.y - cellHeight / 2);

        CreateBoard();
    }


    public void CreateBoard()
    {
        board = new BoardCell[boardCellHeight, BoardCellWidth];
        var positionToSpawn = cellSpawnStartingPosition;

        for (int row = 0; row < boardCellHeight; row++)
        {
            for (int column = 0; column < BoardCellWidth;  column++)
            {
                GameObject go = (GameObject)Instantiate(cellPrefab, positionToSpawn, Quaternion.identity);
                go.transform.parent = transform;
                RescaleObjectToCellSize(go);

                BoardCell boardCell = go.GetComponent<BoardCell>();
                boardCell.Row = row;
                boardCell.Column = column;
                board[row, column] = boardCell;

                positionToSpawn.x += cellWidth;
            }
            positionToSpawn.y -= cellHeight;
            positionToSpawn.x = cellSpawnStartingPosition.x;
        }

         ResetBoard();
    }

    public void ResetBoard()
    {
        for (int row = 0; row < boardCellHeight; row++)
        {
            for (int column = 0; column < BoardCellWidth; column++)
            {
                // set LAND type for 2 lines of borders on every side
                // as starting Level
                if ((row == 0 || row == 1) ||
                   row == boardCellHeight - 1 ||
                   row == boardCellHeight - 2 ||
                    (column == 0 || column == 1) ||
                    column == BoardCellWidth - 1 ||
                    column == BoardCellWidth - 2)
                {
                    board[row, column].Type = BoardCellType.LAND;
                }
                else
                    board[row, column].Type = BoardCellType.AQUA;
            }
        }
    }

    public void RescaleObjectToCellSize(GameObject go)
    {
        var sizes = go.GetComponent<Renderer>().bounds.size;
        float sizeY = sizes.y;
        float sizeX = sizes.x;

        Vector3 rescale = go.transform.localScale;

        rescale.y = cellHeight * rescale.y / sizeY;
        rescale.x = cellWidth * rescale.x / sizeX;

        go.transform.localScale = rescale;
    }

    public IEnumerable<BoardCell> GetCells(System.Predicate<BoardCell> condition)
    {
        for(int row =0; row < boardCellHeight; row++)
            for(int column = 0; column < BoardCellWidth; column++)
                if (condition(board[row, column]))
                    yield return board[row, column];
    }


    public BoardCell this[int row, int column]
    {
        get
        {
            if ((column >= 0 && row >= 0) &&
                    (column < BoardCellWidth && row < boardCellHeight))
                return board[row, column];
            else
                return null;
        }
    }

    public int BoardCellHeight
    {
        get { return boardCellHeight; }
    }

    public BoardCell this[BoardCell cell]
    {
        get{ return instance[cell.Row, cell.Column]; }
    }

    static public Board INSTANCE
    {
        get { return instance; }
    }

}
