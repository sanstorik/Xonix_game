using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;

public enum MoveDirectionsRow
{
    TOP = -1, BOTTOM = 1, NONE = 0
}

public enum MoveDirectionsColumn
{
    RIGHT = -1, LEFT = 1, NONE = 0
}

public abstract class Figure : MonoBehaviour {

    float timer = 0;
    const float MOVE_DELAY = 0.16f;

    //top-bottom movement
    protected MoveDirectionsRow moveDirectionRow;
    //right-left movement
    protected MoveDirectionsColumn moveDirectionColumn;

    protected BoardCell currentCell;

    protected event Action<BoardCell> OnFigureMovedEvent;

    protected virtual void Start()
    {
        Board.INSTANCE.RescaleObjectToCellSize(gameObject);
    }

    protected virtual void Update () {
        timer += Time.deltaTime;

        if (timer >= MOVE_DELAY)
        {
            Move();
            timer = 0;

            OnFigureMovedEvent(currentCell);
        }
    }

    protected virtual void Move()
    {
        currentCell = Board.INSTANCE[currentCell.Row + (int)moveDirectionRow, 
            currentCell.Column + (int)moveDirectionColumn];

        transform.DOMove(currentCell.transform.position, MOVE_DELAY);

    }

    protected MoveDirectionsColumn GetOppositeMoveDirectionColumn()
    {
        return  moveDirectionColumn ==
                 MoveDirectionsColumn.RIGHT ? MoveDirectionsColumn.LEFT : MoveDirectionsColumn.RIGHT;
    }

    protected MoveDirectionsRow GetOppositeMoveDirectionRow()
    {
        return  moveDirectionRow ==
                 MoveDirectionsRow.BOTTOM ? MoveDirectionsRow.TOP : MoveDirectionsRow.BOTTOM;
    }

    protected BoardCell GetNextMoveCell(BoardCell current)
    {
        return Board.INSTANCE[current.Row + (int)moveDirectionRow, current.Column + (int)moveDirectionColumn];
    }

    protected void MoveToCell(BoardCell cell)
    {
        currentCell = cell;
        transform.position = currentCell.transform.position;
    }

    public BoardCell Cell
    {
        get { return currentCell; }
    }
}
