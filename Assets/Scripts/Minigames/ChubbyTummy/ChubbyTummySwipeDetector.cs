using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;

public class ChubbyTummySwipeDetector : NetworkBehaviour
{
    private const int mMessageWidth = 200;
    private const int mMessageHeight = 64;

    private readonly Vector2 mXAxis = new Vector2(1, 0);
    private readonly Vector2 mYAxis = new Vector2(0, 1);

    public GameObject fireButton;
    public GameObject jumpButton;

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

    private bool swipingRight;
    private bool swipingLeft;
    private bool swipeUp;
    private bool swipeDown;
    private bool fire;

    private static Vector3 fireColliderCenter;
    private static float fireColliderRadius;

    private static Vector3 jumpColliderCenter;
    private static float jumpColliderRadius;


    private void Start()
    {
        swipingRight = false;
        swipingLeft = false;
        swipeUp = false;
        swipeDown = false;
        fire = false;
        

        // ottengo oggetto fire button
        fireButton = GameObject.Find("Canvas").transform.Find("FireButton").gameObject;
        jumpButton = GameObject.Find("Canvas").transform.Find("JumpButton").gameObject;

        fireColliderRadius = fireButton.GetComponent<CircleCollider2D>().bounds.extents.x; //x e y sono uguali
        fireColliderCenter = fireButton.GetComponent<CircleCollider2D>().bounds.center;

        jumpColliderRadius = jumpButton.GetComponent<CircleCollider2D>().bounds.extents.x; //x e y sono uguali
        jumpColliderCenter = jumpButton.GetComponent<CircleCollider2D>().bounds.center;
    }
    
    

    // Update is called once per frame
    void Update()
    {
        GestureCompleted();
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

            if (Input.GetMouseButton(0))
            {
                Vector2 currentFingerPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                float deltaX = currentFingerPosition.x - mStartPosition.x;
                float deltaY = currentFingerPosition.y - mStartPosition.y;
                // sono sotto i 45 gradi
                if (Math.Abs(deltaY) < Math.Abs(deltaX))
                {
                    if (deltaX < 0)
                    {
                        swipingRight = false;
                        swipingLeft = true;

                    }
                    else
                    {
                        swipingLeft = false;
                        swipingRight = true;
                    }
                }
            }
            
            // Mouse button up, possible chance for a swipe
            if (Input.GetMouseButtonUp(0))
            {
                // blocco lo swiping right/left
                GestureCompleted();
                float deltaTime = Time.time - mSwipeStartTime;

                Vector2 endPosition = new Vector2(Input.mousePosition.x,
                                                   Input.mousePosition.y);
                Vector2 swipeVector = endPosition - mStartPosition;

                float velocity = swipeVector.magnitude / deltaTime;
                if (isClickInside(mStartPosition, fireColliderCenter, fireColliderRadius) && isClickInside(endPosition, fireColliderCenter, fireColliderRadius))
                {
                    SetFire(true);
                }
                else
                if (isClickInside(mStartPosition, jumpColliderCenter, jumpColliderRadius) && isClickInside(endPosition, jumpColliderCenter, jumpColliderRadius))
                {
                    swipeUp = true;
                }
            }
        }
    }



 

    public bool GetSwipeUp()
    {
        return swipeUp;
    }

    public  bool isSwipingRight()
    {
        return swipingRight;
    }
    public  bool isSwipingLeft()
    {
        return swipingLeft;
    }

    public  void SetFire(bool value)
    {
        fire = value;
    }

    public  bool GetFire()
    {
        return fire;
    }

    public void GestureCompleted()
    {
        swipeUp = false;
        swipingLeft = false;
        swipingRight = false;
    }


    private static bool isClickInside(Vector3 click, Vector3 center, float radius)
    {
        float x = click.x - center.x;
        float y = click.y - center.y;
        float square = x * x + y * y;
        if (radius * radius >= square)
        {
            return true;
        }
        return false;
    }
    

}