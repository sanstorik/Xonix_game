using UnityEngine;
using System.Collections;

public enum HeroMoveDirection
{
    LEFT, RIGHT, TOP, BOTTOM, NONE
}

public class MainHero : Figure
{
    HeroMoveDirection heroMoveDirection;
    bool isMakingTrack = false;
    bool isDead = false;
    SpriteRenderer spriteRenderer;
    Track track;

    [SerializeField]
    Sprite ship;
    [SerializeField]
    Sprite ussualSprite;

    protected override void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StopHero();

        base.Start();
        OnFigureMovedEvent += CheckIfTrackStarted;
        OnFigureMovedEvent += CheckIfTrackEnded;
        OnFigureMovedEvent += DoTrackOnMove;

        MoveToCell(Board.INSTANCE[0, Board.BoardCellWidth / 2]);

        OnFigureMovedEvent += (cell) =>
        {
            if (isMakingTrack && !isDead)
            {
                if (GetNextMoveCell(cell).IsTracked)
                    Die();
            }
        };
    }

    protected override void Move()
    {
        if (heroMoveDirection == HeroMoveDirection.NONE)
            return;

        //if player hit the borders of game stop him
        //and dont allow to move
        if ((currentCell.Row + (int)moveDirectionRow >= Board.INSTANCE.BoardCellHeight) ||
            (currentCell.Row + (int)moveDirectionRow < 0) ||
            (currentCell.Column + (int)moveDirectionColumn < 0) ||
            (currentCell.Column + (int)moveDirectionColumn >= Board.BoardCellWidth))
        {
            StopHero();
            return;
        }

        base.Move();
    }

    public void ChangeMoveDirection(HeroMoveDirection direction)
    {
        if (heroMoveDirection == direction || direction == HeroMoveDirection.NONE)
            return;

        if (GetOppositeMoveDirection(heroMoveDirection) == direction)
        {
            Die();
            return;
        }

        StopHero();
        heroMoveDirection = direction;

        ConvertMoveDirection();
    }

    public void Die()
    {
        isDead = true;
        StartCoroutine(DieAnimation());
    }


    void StopHero()
    {
        moveDirectionColumn = MoveDirectionsColumn.NONE;
        moveDirectionRow = MoveDirectionsRow.NONE;
        heroMoveDirection = HeroMoveDirection.NONE;
    }

    void CheckIfTrackStarted(BoardCell cell)
    {
        if (!isMakingTrack && cell.Type == BoardCellType.AQUA && !isDead)
        {
            isMakingTrack = true;
            spriteRenderer.sprite = ship;
            Board.INSTANCE.RescaleObjectToCellSize(gameObject);

            track = new Track();
        }
    }

    void CheckIfTrackEnded(BoardCell cell)
    {
        if (isMakingTrack && cell.Type == BoardCellType.LAND && !isDead)
        {
            isMakingTrack = false;
            transform.eulerAngles = new Vector3(0, 0, 0);
            spriteRenderer.sprite = ussualSprite;

            Board.INSTANCE.RescaleObjectToCellSize(gameObject);

            track.ExecuteAlgo();
        }
    }

    void DoTrackOnMove(BoardCell cell)
    {
        if (isMakingTrack && !isDead)
        {
            cell.MarkCellAsTracked();
            track.AddTrackPoint(cell);

            ChangeRotationOfSpriteOnMove();
        }
    }

    HeroMoveDirection GetOppositeMoveDirection(HeroMoveDirection direction)
    {
        HeroMoveDirection temp = HeroMoveDirection.NONE;

        if (direction == HeroMoveDirection.RIGHT)
            temp = HeroMoveDirection.LEFT;
        else if (direction == HeroMoveDirection.LEFT)
            temp = HeroMoveDirection.RIGHT;
        else if (direction == HeroMoveDirection.BOTTOM)
            temp = HeroMoveDirection.TOP;
        else if (direction == HeroMoveDirection.TOP)
            temp = HeroMoveDirection.BOTTOM;

        return temp;
    }

    void ConvertMoveDirection()
    {
        if (heroMoveDirection == HeroMoveDirection.RIGHT)
            moveDirectionColumn = MoveDirectionsColumn.LEFT;
        else if (heroMoveDirection == HeroMoveDirection.LEFT)
            moveDirectionColumn = MoveDirectionsColumn.RIGHT;
        else if (heroMoveDirection == HeroMoveDirection.BOTTOM)
            moveDirectionRow = MoveDirectionsRow.BOTTOM;
        else if (heroMoveDirection == HeroMoveDirection.TOP)
            moveDirectionRow = MoveDirectionsRow.TOP;
        else if (heroMoveDirection == HeroMoveDirection.NONE)
            moveDirectionRow = MoveDirectionsRow.NONE;
    }

    void ChangeRotationOfSpriteOnMove()
    {
        switch (heroMoveDirection)
        {
            case HeroMoveDirection.RIGHT:
                var scale = transform.localScale;
                scale.x *= -1;

                transform.localScale = scale;
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case HeroMoveDirection.LEFT:
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case HeroMoveDirection.TOP:
                transform.eulerAngles = new Vector3(0, 0, 90);
                break;
            case HeroMoveDirection.BOTTOM:
                transform.eulerAngles = new Vector3(0, 0, 90);
                break;
        }
    }


    IEnumerator DieAnimation()
    {
        Time.timeScale = 0;
        yield return StartCoroutine(TextContoller.INSTANCE.WaitForRealSeconds(1.5f));
        Time.timeScale = 1;


        if (track != null)
            track.ResetTrack();

        LevelManager.INSTANCE.OnHeroDie();
    }

    public bool IsDead
    {
        get { return isDead; }
        set { isDead = true; }
    }
}