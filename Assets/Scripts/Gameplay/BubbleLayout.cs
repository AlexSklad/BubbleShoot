using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleLayout : MonoBehaviour
{
    private static BubbleLayout _instance;

    [SerializeField]
    private int _maxBubbleCount = 100;
    [SerializeField]
    private float _bubbleRadius = 0.25f;
    [SerializeField]
    private float _touchingBubblesDistance = 0.6f;

    [SerializeField]
    private GameObject _bubblePrefab;

    private Pool<Bubble> _bubblePool;
    private List<Bubble> _bubblesOnLayout;

    public static BubbleLayout Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BubbleLayout>(true);
            }

            return _instance;
        }
    }

    public int BubbleCount
    {
        get { return _bubblesOnLayout.Count; }
    }

    private void Awake()
    {
        _bubblePool = new Pool<Bubble>
        (
            _maxBubbleCount,
            () =>
            {
                var x = Instantiate(_bubblePrefab, transform);
                return x.GetComponent<Bubble>();
            },
            x =>
            {
                Destroy(x.gameObject);
            },
            x =>
            {
                x.Active = true;
            },
            x =>
            {
                x.Active = false;
            }
        );
        _bubblesOnLayout = new List<Bubble>(_maxBubbleCount);
    }

    public void PlaceBubbles(BubblePlacement[] placement)
    {
        for (int bubbleNum = 0; bubbleNum < placement.Length; bubbleNum++)
        {
            var bubble = PullBubble();
            bubble.Position = placement[bubbleNum].Position;
            bubble.IsKinematic = true;
            bubble.CollisionLayer = Layer.Bubble;
            bubble.Color = placement[bubbleNum].Color;
            _bubblesOnLayout.Add(bubble);
        }
    }
    public bool OverlapsWithAnyBubble(Vector2 position)
    {
        for (int i = 0; i < _bubblesOnLayout.Count; i++)
        {
            if (Vector3.Distance(position, _bubblesOnLayout[i].Position) < _bubbleRadius)
            {
                return true;
            }
        }

        return false;
    }

    public void StickBubbleToLayout(Bubble targetBubble, Bubble closestBubble)
    {
        targetBubble.IsKinematic = true;
        targetBubble.CollisionLayer = Layer.Bubble;
        targetBubble.Velocity = Vector2.zero;

        var initialPosition = targetBubble.Position;
        var referencePosition = closestBubble.Position;

        var cellSize = _bubbleRadius * 2f;
        var basePoint = new Vector2(0f, cellSize);

        var possiblePositions = new Vector2[6];

        for (int i = 0; i < possiblePositions.Length; i++)
        {
            possiblePositions[i] = referencePosition + (Vector2)(Quaternion.Euler(0f, 0f, i * 60f - 30f) * new Vector2(0f, cellSize));
        }

        int closestPosition = -1;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < possiblePositions.Length; i++)
        {
            if (Stage.Instance.IsOutOfBounds(possiblePositions[i], _bubbleRadius) || OverlapsWithAnyBubble(possiblePositions[i]))
            {
                continue;
            }

            float newDistance = Vector2.Distance(initialPosition, possiblePositions[i]);

            if (newDistance < closestDistance)
            {
                closestDistance = newDistance;
                closestPosition = i;
            }
        }

        if (closestPosition >= 0)
        {
            targetBubble.Position = possiblePositions[closestPosition];
        }

        _bubblesOnLayout.Add(targetBubble);
    }

    public List<Bubble> GetTouchingBubbles(Bubble targetBubble)
    {
        List<Bubble> result = new List<Bubble>();

        for (int i = 0; i < _bubblesOnLayout.Count; i++)
        {
            if (_bubblesOnLayout[i] != targetBubble && Vector3.Distance(targetBubble.Position, _bubblesOnLayout[i].Position) < _touchingBubblesDistance)
            {
                result.Add(_bubblesOnLayout[i]);
            }
        }

        return result;
    }

    public void DestroyBubblesAround(Bubble targetBubble, List<Bubble> exceptThese)
    {
        int destroyCount = CheckBubblesToDestroy(targetBubble, ref exceptThese);

        if (destroyCount > 1)
        {
            for (int i = 0; i < exceptThese.Count; i++)
            {
                exceptThese[i].Destroy();
            }
            //return destroyCount;
        }
        //else
        //{
        //    return 0;
        //}

    }

    private int  CheckBubblesToDestroy(Bubble targetBubble, ref List<Bubble> exceptThese)
    {
        int destroyCount = 0;

        var bubblesAround = BubbleLayout.Instance.GetTouchingBubbles(targetBubble);

        for (int i = 0; i < bubblesAround.Count; i++)
        {
            if (bubblesAround[i].Color == targetBubble.Color && !exceptThese.Contains(bubblesAround[i]))
            {
                exceptThese.Add(bubblesAround[i]);
                destroyCount += CheckBubblesToDestroy(bubblesAround[i], ref exceptThese);
                destroyCount++;
            }
        }

        return destroyCount;

    }

    public List<BubbleColor> GetColorsPresentOnLayout()
    {
        List<BubbleColor> result = new List<BubbleColor>();

        for (int i = 0; i < _bubblesOnLayout.Count; i++)
        {
            if (!result.Contains(_bubblesOnLayout[i].Color))
            {
                result.Add(_bubblesOnLayout[i].Color);
            }
        }

        return result;
    }
    public void Clear()
    {
        for (int i = _bubblesOnLayout.Count - 1; i >= 0; i--)
        {
            PushBubble(_bubblesOnLayout[i]);
        }
    }

    public Bubble PullBubble()
    {
        return _bubblePool.Pull();
    }
    public void PushBubble(Bubble bubble)
    {
        if (!bubble.Active)
        {
            return;
        }

        if (_bubblesOnLayout.Contains(bubble))
        {
            _bubblesOnLayout.Remove(bubble);
        }

        _bubblePool.Push(bubble);
    }

}
