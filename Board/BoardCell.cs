using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BoardCellType
{
    LAND, AQUA
}

public class BoardCell : MonoBehaviour {
    int row;
    int column;
    BoardCellType type = BoardCellType.AQUA;
    SpriteRenderer spriteRend;
    bool isTracked = false;
    bool isVisited = false;
    bool isHeroOnCell = false;

    GameObject track;

    private void Start()
    {
        track = transform.GetChild(0).gameObject;
        spriteRend = GetComponent<SpriteRenderer>();
    }

    public int Row
    {
        get { return row; }
        set { row = value; }
    }

    public int Column
    {
        get { return column; }
        set { column = value; }
    }

    public bool IsTracked
    {
        get { return isTracked; }
    }

    public bool IsVisited
    {
        get { return isVisited; }
        set { isVisited = value; }
    }

    public bool IsHeroOnCell
    {
        get { return isHeroOnCell; }
        set { isHeroOnCell = value; }
    }

    public BoardCellType Type
    {
        set
        {
            BoardCellType oldtype = type;
            type = value;

            if(oldtype != type)
               OnBoardTypeChange((int)type);
        }
        get { return type; }
    }

    void OnBoardTypeChange(int type)
    {
        if (spriteRend == null)
            spriteRend = GetComponent<SpriteRenderer>();

        spriteRend.sprite = Board.INSTANCE.landTypes[type];
        Board.INSTANCE.RescaleObjectToCellSize(gameObject);

        if (isTracked)
            UntrackCell();
    }

    public void MarkCellAsTracked()
    {
        track.SetActive(true);
        isTracked = true;
    }

    public void UntrackCell()
    {
        track.SetActive(false);
        isTracked = false;
    }

    public override bool Equals(object o)
    {
        BoardCell cell = (BoardCell)o;
        return (row == cell.row) && (column == cell.column);
    }

    public override int GetHashCode()
    {
        return (row ^ column) * 500;
    }

    static public bool operator==(BoardCell x, BoardCell y)
    {
        if (ReferenceEquals(x, null))
            return ReferenceEquals(y, null);
        else if(ReferenceEquals(y, null))
            return ReferenceEquals(x, null);

        return x.Equals(y);
    }

    static public bool operator!=(BoardCell x, BoardCell y)
    {
        return !( x == y );
    }
}
