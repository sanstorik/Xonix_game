using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Track {
    List<BoardCell> trackPoints;
    List<BoardCell> allNonLandPoints;

    List<HashSet<BoardCell>> closedAreas;
    Queue<BoardCell> adjacentCells;
    int closedAreaCount;

    Queue<BoardCell> queueBreadthSearch;
    BoardCell current;
    BoardCell queCurrent;

    public Track()
    {
        closedAreas = new List<HashSet<BoardCell>>();
        closedAreaCount = -1;
        adjacentCells = new Queue<BoardCell>();
        trackPoints = new List<BoardCell>();
        queueBreadthSearch = new Queue<BoardCell>();
        allNonLandPoints = Board.INSTANCE.GetCells((cell) => cell.Type != BoardCellType.LAND).ToList();
    }

    public void AddTrackPoint(BoardCell cell)
    {
        trackPoints.Add(cell);
    }

    public void ExecuteAlgo()
    {
        MarkTrackPointsAsVisited();
        for (int cellIndex = 0; cellIndex < allNonLandPoints.Count; cellIndex++)
            BreadthFirstSearch(allNonLandPoints[cellIndex]);

        ResetSelected();
        RepaintClosedAreas();
    }

    void MarkTrackPointsAsVisited()
    {
        foreach (var cell in trackPoints)
            cell.IsVisited = true;
    }

    void RepaintClosedAreas()
    {
        closedAreas.Remove(PickBiggestClosedArea());
        Queue<EnemyFigure> enemiesToDestroy = new Queue<EnemyFigure>();

        foreach (var area in closedAreas)
        {
            foreach (var cell in area)
                cell.Type = BoardCellType.LAND;

            foreach (var enemy in LevelManager.INSTANCE.GetEnemiesOnScene())
                if (area.Contains(enemy.Cell))
                    enemiesToDestroy.Enqueue(enemy);

            LevelManager.INSTANCE.IncreaseScore(area.Count);
        }

        while (enemiesToDestroy.Count > 0)
            LevelManager.INSTANCE.DestroyEnemy(enemiesToDestroy.Dequeue());

        foreach (var track in trackPoints)
            track.Type = BoardCellType.LAND;

        LevelManager.INSTANCE.IncreaseScore(trackPoints.Count);
        LevelManager.INSTANCE.CheckIfEnemiesDied();
    }

    HashSet<BoardCell> PickBiggestClosedArea()
    {
        HashSet<BoardCell> theBiggestArea = closedAreas[0];
        foreach(var area in closedAreas) 
            if (area.Count > theBiggestArea.Count)
                theBiggestArea = area;


        return theBiggestArea;
    }

    void BreadthFirstSearch(BoardCell root)
    {
        if (root.IsVisited)
            return;

        closedAreas.Add(new HashSet<BoardCell>());
        closedAreaCount++;

        queueBreadthSearch.Enqueue(root);
        closedAreas[closedAreaCount].Add(root);

        while (queueBreadthSearch.Count > 0)
        {
            current = queueBreadthSearch.Dequeue();
            GetAdjacentCells(current);
            while (adjacentCells.Count > 0)
            {
                queCurrent = adjacentCells.Dequeue();
                queCurrent.IsVisited = true;
                closedAreas[closedAreaCount].Add(queCurrent);
                queueBreadthSearch.Enqueue(queCurrent);
            }
        }
    }

    void GetAdjacentCells(BoardCell cell)
    {
        // from top right cell clockwise
        AddCellWithIncrement(cell, 1, 1);
        AddCellWithIncrement(cell, 0, 1);
        AddCellWithIncrement(cell, -1, 1);
        AddCellWithIncrement(cell, -1, 0);
        AddCellWithIncrement(cell, -1, -1);
        AddCellWithIncrement(cell, 0, -1);
        AddCellWithIncrement(cell, 1, -1);
        AddCellWithIncrement(cell, 1, 0);
    }

    void AddCellWithIncrement(BoardCell cell, int rowIncrement, int columnIncrement)
    {
        BoardCell incrementedCell = Board.INSTANCE[cell.Row + rowIncrement, cell.Column + columnIncrement];

        if (incrementedCell != null  && !incrementedCell.IsVisited && incrementedCell.Type != BoardCellType.LAND)
            adjacentCells.Enqueue(incrementedCell);
    }

    void ResetSelected()
    {
        for (int i = 0; i < allNonLandPoints.Count; i++)
            allNonLandPoints[i].IsVisited = false;
    }

    public void ResetTrack()
    {
        foreach (var cell in trackPoints)
            cell.UntrackCell();
    }
}
