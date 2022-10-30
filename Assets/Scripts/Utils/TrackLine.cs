using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackLine : MonoBehaviour
{
    [SerializeField]
    private int _maxDotCount = 100;
    [SerializeField]
    private int _maxBounces = 5;
    [SerializeField]
    private float _maxRaycastDistance = 100f;
    [SerializeField]
    private float _dotSpacing = 0.25f;
    [SerializeField]
    private float _startPadding = 0.75f;
    [SerializeField]
    private float _projectileRadius = 0.25f;

    [SerializeField]
    private GameObject _lineDotPrefab;

    private Pool<Transform> _dotPool;

    private List<Transform> _activeDots;

    private bool m_isActive = false;

    private List<Vector2> _hitPoints;

    private RaycastHit2D[] _raycastResults = new RaycastHit2D[1];

    private ContactFilter2D contactFilter;

    private void DoRaycasting(List<Vector2> pointList, Vector2 position, Vector2 direction, int bouncesCount = 0)
    {
        if (Physics2D.CircleCast(position, _projectileRadius, direction, contactFilter, _raycastResults, _maxRaycastDistance) > 0)
        {
            pointList.Add(_raycastResults[0].centroid);

            bouncesCount++;

            if (bouncesCount >= _maxBounces)
            {
                return;
            }

            if (_raycastResults[0].collider.gameObject.layer == (int)Layer.Bubble)
            {
                return;
            }

            var newDirection = Vector2.Reflect(direction, _raycastResults[0].normal);
            var newPosition = _raycastResults[0].centroid + newDirection * _projectileRadius;

            DoRaycasting(pointList, newPosition, newDirection, bouncesCount);

            return;
        }

        if (bouncesCount < _maxBounces)
        {
            pointList.Add(position + direction * _maxRaycastDistance);
        }
    }
    
    private void Awake()
    {
        _dotPool = new Pool<Transform>
        (
            _maxDotCount,
            () =>
            {
                var x = Instantiate(_lineDotPrefab, transform);
                return x.transform;
            },
            x =>
            {
                Destroy(x.gameObject);
            },
            x =>
            {
                x.gameObject.SetActive(true);
            },
            x =>
            {
                x.gameObject.SetActive(false);
            }
        );
        _activeDots = new List<Transform>(_maxDotCount);
        _hitPoints = new List<Vector2>(_maxBounces);
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(1 << (int)Layer.Wall | 1 << (int)Layer.Bubble);
    }

    public void DrawBouncePath(Vector2 startPosition, Vector2 direction)
    {
        m_isActive = true;

        _hitPoints.Clear();

        DoRaycasting(_hitPoints, startPosition, direction);

        Vector2 dir;
        Vector2 dotPosition;
        int dotNum = 0;
        Transform curDot;
        Vector2 nextPoint = startPosition;
        float remainingDistance = -_startPadding;

        for (int pointNum = 0; pointNum < _hitPoints.Count; pointNum++)
        {
            dir = (_hitPoints[pointNum] - nextPoint).normalized;

            dotPosition = Vector3.MoveTowards(nextPoint, _hitPoints[pointNum], _dotSpacing - remainingDistance);
            nextPoint = _hitPoints[pointNum];
            
            while (true)
            {
                if (dotNum >= _maxDotCount)
                {
                    break;
                }

                if (dotNum < _activeDots.Count)
                {
                    curDot = _activeDots[dotNum];
                }
                else
                {
                    curDot = _dotPool.Pull();
                    _activeDots.Add(curDot);
                }

                curDot.position = dotPosition;

                dotNum++;

                remainingDistance = Vector2.Distance(dotPosition, nextPoint);

                if (remainingDistance < _dotSpacing)
                {
                    break;
                }

                dotPosition = Vector3.MoveTowards(dotPosition, nextPoint, _dotSpacing);
            }
        }

        for (int i = _activeDots.Count - 1; i >= dotNum; i--)
        {
            _dotPool.Push(_activeDots[i]);
            _activeDots.RemoveAt(i);
        }
    }

    public void Clear()
    {
        if (!m_isActive)
        {
            return;
        }

        m_isActive = false;

        for (int i = 0; i < _activeDots.Count; i++)
        {
            _dotPool.Push(_activeDots[i]);
        }

        _activeDots.Clear();
    }
}
