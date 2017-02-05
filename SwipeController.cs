using UnityEngine;
using System.Collections;

public class SwipeController : MonoBehaviour {

    static public MainHero mainHero;
    static public bool gameStarted = false;
    static public bool gamePaused = false;
    Vector2 mouseClickPosition;
    Vector2 mouseReleasePosition;

    float minimumOffset = 100f;

	void Update () {
        if (Input.GetMouseButtonDown(0))
            mouseClickPosition = Input.mousePosition;
        else if (Input.GetMouseButtonUp(0) && gameStarted && !gamePaused)
        {
            mouseReleasePosition = Input.mousePosition;

            CalculateSwipedMoveDirection();
        }
	}

    // THERES ONLY ONE DIRECTION
    // NIAL THE BEST
    void CalculateSwipedMoveDirection()
    {
        HeroMoveDirection direction = HeroMoveDirection.NONE;

        // IF SWIPED RIGHT OR LEFT
        if (Mathf.Abs(mouseClickPosition.y - mouseReleasePosition.y) < minimumOffset)
        {
            if (Mathf.Abs(mouseClickPosition.x) + minimumOffset / 2 < Mathf.Abs(mouseReleasePosition.x))
                direction = HeroMoveDirection.RIGHT;

            else if (Mathf.Abs(mouseClickPosition.x) - minimumOffset / 2 > Mathf.Abs(mouseReleasePosition.x))
                direction = HeroMoveDirection.LEFT;
        }

        // IF SWIPED TOP OR BOTTOM
        else if (Mathf.Abs(mouseClickPosition.x - mouseReleasePosition.x) < minimumOffset)
        {

            if (Mathf.Abs(mouseClickPosition.y) + minimumOffset / 3f < Mathf.Abs(mouseReleasePosition.y))
                direction = HeroMoveDirection.TOP;

            if (Mathf.Abs(mouseClickPosition.y) - minimumOffset / 3f > Mathf.Abs(mouseReleasePosition.y))
                direction = HeroMoveDirection.BOTTOM;
        }

        if(mainHero != null)
           mainHero.ChangeMoveDirection(direction);
    }
}
