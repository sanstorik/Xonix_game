using UnityEngine;
using System.Linq;
using System.Collections;


public abstract class EnemyFigure : Figure
{
    BoardCellType cellMoveType;
    BoardCellType oppositeType;

    protected override void Start()
    {
        base.Start();
        SpawnAtRandomPosition();

        oppositeType = cellMoveType ==
              BoardCellType.AQUA ? BoardCellType.LAND : BoardCellType.AQUA;
        OnFigureMovedEvent += ChangeMoveDirectionOnHit;
        OnFigureMovedEvent += CheckIfHitMainHero;
    }

    void SpawnAtRandomPosition()
    {
        var availableCellsToSpawn = Board.INSTANCE.GetCells(cell => cell.Type == CellMoveType &&
            cell.Row != 1 && cell.Row != 0).ToArray();

        int index = Random.Range(0, availableCellsToSpawn.Length);
        var goToCell = availableCellsToSpawn[index];

        MoveToCell(goToCell);
        SetRandomMoveDirections();
    }

    //complex (and stupid) calculations to spawn on proper random position
    // and with proper moveDirection to exclude errors
    void SetRandomMoveDirections()
    {
        float randomNumber = Random.Range(0f, 2f);

        if (currentCell.Row == 0 || currentCell.Row == Board.INSTANCE.BoardCellHeight - 2)
            moveDirectionRow = MoveDirectionsRow.BOTTOM;
        else if (currentCell.Row == Board.INSTANCE.BoardCellHeight - 1 || currentCell.Row == 1)
            moveDirectionRow = MoveDirectionsRow.TOP;
        else
        {
            if (randomNumber <= 1)
                moveDirectionRow = MoveDirectionsRow.TOP;
            else
                moveDirectionRow = MoveDirectionsRow.BOTTOM;
        }

        if (currentCell.Column == 0 || currentCell.Column == Board.BoardCellWidth - 2)
            moveDirectionColumn = MoveDirectionsColumn.LEFT;
        else if (currentCell.Column == Board.BoardCellWidth - 1 || currentCell.Column == 1)
            moveDirectionColumn = MoveDirectionsColumn.RIGHT;
        else
        {
            if (randomNumber <= 1)
                moveDirectionColumn = MoveDirectionsColumn.RIGHT;
            else
                moveDirectionColumn = MoveDirectionsColumn.LEFT;
        }
    }

    protected virtual void ChangeMoveDirectionOnHit(BoardCell cell)
    {
        // there is border end for figure on columnIncrease
        if (GetNextMoveCell(cell).Type == oppositeType)
        {
            if (HittingCorners(cell))
            {
                moveDirectionColumn = GetOppositeMoveDirectionColumn();
                moveDirectionRow = GetOppositeMoveDirectionRow();
            }
            else if (HittingTopOrBottomBorders(cell))
                moveDirectionRow = GetOppositeMoveDirectionRow();
            //if hitting right/left borders
            else
                moveDirectionColumn = GetOppositeMoveDirectionColumn();
        }
    }

    void CheckIfHitMainHero(BoardCell cell)
    {
        if(Vector3.Distance(transform.position, SwipeController.mainHero.transform.position) <= 0.7f
            && currentCell.Type == SwipeController.mainHero.Cell.Type)
        {
            SwipeController.mainHero.Die();
        }
    }

    bool HittingTopOrBottomBorders(BoardCell cell)
    {
        return Board.INSTANCE[cell.Row + (int)moveDirectionRow, cell.Column].Type == oppositeType;
    }

    bool HittingCorners(BoardCell cell)
    {
        return Board.INSTANCE[cell.Row + (int)moveDirectionRow, cell.Column].Type == oppositeType &&
            Board.INSTANCE[cell.Row, cell.Column + (int)moveDirectionColumn].Type == oppositeType;
    }

    public BoardCellType CellMoveType
    {
        get { return cellMoveType; }
        set { cellMoveType = value; }
    }
}
