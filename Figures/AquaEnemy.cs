using UnityEngine;
using System.Collections;

public class AquaEnemy : EnemyFigure {

    protected override void Start()
    {
        CellMoveType = BoardCellType.AQUA;

        base.Start();
        OnFigureMovedEvent += CheckIfHitTrack;
    }

    void CheckIfHitTrack(BoardCell cell)
    {
        if (Board.INSTANCE[cell.Row, cell.Column].IsTracked)
            SwipeController.mainHero.Die();
    }
}
