using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Transform _arrow;
    [SerializeField]
    private TrackLine _bouncePreviewRenderer;
    [SerializeField]
    private float _maxShootingAngle = 70f;
    [SerializeField]
    private Vector3 _previewBubblePosition;

    private Vector2 _direction = Vector2.up;
    private bool _ChargingShot = false;

    private Bubble _currentBubble;
    private Bubble _nextBubble;

    private bool _waitingForHit;

    private bool _useRandomColors;
    private BubbleColor[] _colorSequence;
    private int _colorSequenceIndex;

    public void NotifyAboutHit()
    {
        _waitingForHit = false;
        GetNextBubble();
    }

    public void SetColorSequence(bool random, BubbleColor[] sequence)
    {
        _useRandomColors = random;
        _colorSequence = sequence;
        _colorSequenceIndex = 0;
    }

    public void Reset()
    {
        _direction = Vector2.up;
        _ChargingShot = false;

        if (_currentBubble != null)
        {
            BubbleLayout.Instance.PushBubble(_currentBubble);
        }

        if (_nextBubble != null)
        {
            BubbleLayout.Instance.PushBubble(_nextBubble);
        }

        _waitingForHit = false;

        _colorSequenceIndex = 0;

        GetNextBubble();
        GetNextBubble();
    }

    private void Update()
    {
        if (Game.Instance.CurrentState != Game.GameState.Playing)
        {
            return;
        }

        UpdateControls();
        UpdateVisuals();
    }

    private void UpdateControls()
    {
        if (!UIRaycaster.IsPointerOverUIObject())
        {
            if (Input.GetMouseButton(0))
            {
                _ChargingShot = true;

                _direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

                if (_waitingForHit || Vector2.Angle(Vector2.up, _direction) > _maxShootingAngle)
                {
                    _ChargingShot = false;
                }
            }
        }

        if (_ChargingShot && Input.GetMouseButtonUp(0))
        {
            _ChargingShot = false;

            Shoot();
        }
    }

    private void UpdateVisuals()
    {
        _arrow.up = _direction;

        if (_ChargingShot)
        {
            _bouncePreviewRenderer.DrawBouncePath(transform.position, _direction);
        }
        else
        {
            _bouncePreviewRenderer.Clear();
        }
    }

    private void Shoot()
    {
        _currentBubble.IsKinematic = false;
        _currentBubble.CollisionLayer = Layer.ShootingBubble;
        _currentBubble.ApplyForce(_direction);

        _currentBubble.OnBubbleHit = NotifyAboutHit;

        _waitingForHit = true;
    }

    private void GetNextBubble()
    {
        _currentBubble = _nextBubble;

        if (_currentBubble != null)
        {
            _currentBubble.Position = transform.position;
        }

        _nextBubble = BubbleLayout.Instance.PullBubble();
        _nextBubble.Position = transform.TransformPoint(_previewBubblePosition);
        _nextBubble.IsKinematic = true;
        _nextBubble.CollisionLayer = Layer.PreviewBubble;

        if (_useRandomColors)
        {
            var availableColors = BubbleLayout.Instance.GetColorsPresentOnLayout();
            _nextBubble.Color = availableColors.Count > 0 ? availableColors[Random.Range(0, availableColors.Count)] : BubbleColor.Red;
        }
        else
        {
            _nextBubble.Color = _colorSequence[_colorSequenceIndex++];

            if (_colorSequenceIndex >= _colorSequence.Length)
            {
                _colorSequenceIndex = 0;
            }
        }
    }
}
