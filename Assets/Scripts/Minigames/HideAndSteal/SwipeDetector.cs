using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SwipeDetector : NetworkBehaviour
{

    private const int mMessageWidth = 200;
    private const int mMessageHeight = 64;

    private readonly Vector2 mXAxis = new Vector2(1, 0);
    private readonly Vector2 mYAxis = new Vector2(0, 1);

    // The angle range for detecting swipe
    [Range(10f, 45f)] public float angleRange;

    // To recognize as swipe user should at lease swipe for this many pixels
    public float mMinSwipeDist = 50.0f;

    // To recognize as a swipe the velocity of the swipe
    // should be at least minVelocity
    // Reduce or increase to control the swipe speed
    public float minVelocity = 2000.0f;

    private Vector2 mStartPosition;
    private float mSwipeStartTime;

    private static bool swipeRight = false;
    private static bool swipeLeft = false;
    private static bool swipeUp = false;
    private static bool swipeDown = false;

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            // Mouse button down, possible chance for a swipe
            if (Input.GetMouseButtonDown(0))
            {
                // Record start time and position
                mStartPosition = new Vector2(Input.mousePosition.x,
                                             Input.mousePosition.y);
                mSwipeStartTime = Time.time;
            }

            // Mouse button up, possible chance for a swipe
            if (Input.GetMouseButtonUp(0))
            {
                float deltaTime = Time.time - mSwipeStartTime;

                Vector2 endPosition = new Vector2(Input.mousePosition.x,
                                                   Input.mousePosition.y);
                Vector2 swipeVector = endPosition - mStartPosition;

                float velocity = swipeVector.magnitude / deltaTime;

                if (velocity > minVelocity &&
                    swipeVector.magnitude > mMinSwipeDist)
                {
                    // if the swipe has enough velocity and enough distance

                    swipeVector.Normalize();

                    float angleOfSwipe = Vector2.Dot(swipeVector, mXAxis);
                    angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;

                    // Detect left and right swipe
                    if (angleOfSwipe < angleRange)
                    {
                        OnSwipeRight();
                    }
                    else if ((180.0f - angleOfSwipe) < angleRange)
                    {
                        OnSwipeLeft();
                    }
                    else
                    {
                        // Detect top and bottom swipe
                        angleOfSwipe = Vector2.Dot(swipeVector, mYAxis);
                        angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;
                        if (angleOfSwipe < angleRange)
                        {
                            OnSwipeTop();

                        }
                        else if ((180.0f - angleOfSwipe) < angleRange)
                        {
                            OnSwipeBottom();

                        }
                        else
                        {
                            Debug.Log("swipe male");
                        }
                    }
                }
            }
        }
    }


    private void OnSwipeLeft()
    {
        swipeLeft = true;
        swipeRight = false;
        swipeDown = false;
        swipeUp = false;
    }

    private void OnSwipeRight()
    {
        swipeLeft = false;
        swipeRight = true;
        swipeDown = false;
        swipeUp = false;
    }

    private void OnSwipeTop()
    {
        swipeLeft = false;
        swipeRight = false;
        swipeDown = false;
        swipeUp = true;
    }

    private void OnSwipeBottom()
    {
        swipeLeft = false;
        swipeRight = false;
        swipeDown = true;
        swipeUp = false;
    }

    public static bool GetSwipeUp()
    {
        return swipeUp;
    }

    public static bool GetSwipeDown()
    {
        return swipeDown;
    }

    public static bool GetSwipeRight()
    {
        return swipeRight;
    }

    public static bool GetSwipeLeft()
    {
        return swipeLeft;
    }

    public static void GestureCompleted()
    {
        swipeLeft = false;
        swipeRight = false;
        swipeDown = false;
        swipeUp = false;
    }


}