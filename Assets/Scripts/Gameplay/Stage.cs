using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    private static Stage _instance;

    [SerializeField]
    private float _wallThickness = 1f;
    [SerializeField]
    private BoxCollider2D _leftWall;
    [SerializeField]
    private BoxCollider2D _rightWall;
    [SerializeField]
    private BoxCollider2D _topWall;
    [SerializeField]
    private BoxCollider2D _downWall;
    [SerializeField]
    private Transform _backgroundTransform;
    [SerializeField]
    private SpriteRenderer _backgroundImage;

    public static Stage Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Stage>(true);
            }

            return _instance;
        }
    }

    public void SetBackground(string backgroundName)
    {
        _backgroundImage.sprite = Resources.Load<Sprite>("Backgrounds/" + backgroundName);
    }

    public bool IsOutOfBounds(Vector2 position, float radius)
    {
        var edge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f));

        return position.x < -edge.x + radius ||
            position.x > edge.x - radius ||
            position.y < -Camera.main.orthographicSize + radius ||
            position.y > Camera.main.orthographicSize - radius;
    }

    private void Start()
    {
        SetupStage();
    }

    private void SetupStage()
    {
        // Correct the camera zoom
        Camera.main.orthographicSize = Mathf.Max(5f, 5f * 0.5625f / ((float)Screen.width / (float)Screen.height));
        _backgroundTransform.localScale = Vector3.one * (Camera.main.orthographicSize * 2f);
        SetupVerWall(_leftWall, 0f);
        SetupVerWall(_rightWall, 1f);
        SetupHorWall(_downWall, 0f);
        SetupHorWall(_topWall, 1f);
    }

    private void SetupVerWall(BoxCollider2D wall, float position)
    {
        var edge = Camera.main.ScreenToWorldPoint(new Vector3(position * Screen.width, 0f, 0f));
        wall.offset = new Vector2(edge.x + Mathf.Sign(position - 0.5f) * _wallThickness * 0.5f, 0f);
        wall.size = new Vector2(_wallThickness, Camera.main.orthographicSize * 2f);
    }

    private void SetupHorWall(BoxCollider2D wall, float position)
    {
        var edge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f));
        wall.offset = new Vector2(0f, (position - 0.5f) * Camera.main.orthographicSize * 2f + Mathf.Sign(position - 0.5f) * _wallThickness * 0.5f);
        wall.size = new Vector2(edge.x * 2f, _wallThickness);
    }
}
