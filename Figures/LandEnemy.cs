using UnityEngine;
using System.Collections;

public class LandEnemy : EnemyFigure {

	protected override void Start()
    {
        CellMoveType = BoardCellType.LAND;
        base.Start();
	}


    protected override void ChangeMoveDirectionOnHit(BoardCell cell)
    {
        //check if enemy hits board corners
        if(
            (cell.Row== Board.INSTANCE.BoardCellHeight - 1 &&  // right-bottom corner
            cell.Column == Board.BoardCellWidth - 1) ||
            (cell.Row == Board.INSTANCE.BoardCellHeight -1 &&  // left-bottom corner
            cell.Column == 0) ||
            (cell.Row == 0 &&  cell.Column == 0) ||        // top left corner
            (cell.Row == 0 && cell.Column == Board.BoardCellWidth - 1)      // top right corner
            ) 
        {
            moveDirectionRow = GetOppositeMoveDirectionRow();
            moveDirectionColumn = GetOppositeMoveDirectionColumn();
            return;
        }

            //check if enemy hits top or bottom side
        else if (cell.Row + (int)moveDirectionRow >= Board.INSTANCE.BoardCellHeight ||
            cell.Row + (int)moveDirectionRow < 0)
        {
            moveDirectionRow = GetOppositeMoveDirectionRow();
            return;
        }

        //check if enemy hits right or left side
        else if (cell.Column + (int)moveDirectionColumn >= Board.BoardCellWidth ||
          cell.Column + (int)moveDirectionColumn < 0)
        {
            moveDirectionColumn = GetOppositeMoveDirectionColumn();
            return;
        }



        base.ChangeMoveDirectionOnHit(cell);
    }
}
