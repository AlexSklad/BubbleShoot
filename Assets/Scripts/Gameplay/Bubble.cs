using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bubble : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _sprite;
    [SerializeField]
    private Rigidbody2D _rigibody;
    [SerializeField]
    private Collider2D _collider;
    [SerializeField]
    private float _targetSpeed;
    [SerializeField]
    private BubbleColor _ownColor;
    [SerializeField]
    private Color[] _visualColors;

    private Action _onHit;

    public BubbleColor Color
    {
        get { return _ownColor; }
        set
        {
            _ownColor = value;
            _sprite.color = _visualColors[(int)value];
        }
    }

    public bool IsKinematic
    {
        get { return _rigibody.isKinematic; }
        set { _rigibody.isKinematic = value; }
    }

    public Vector2 Position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public Vector2 Velocity
    {
        get { return _rigibody.velocity; }
        set { _rigibody.velocity = value; }
    }

    public Layer CollisionLayer
    {
        get { return (Layer)_collider.gameObject.layer; }
        set { _collider.gameObject.layer = (int)value; }
    }

    public Action OnBubbleHit
    {
        get { return _onHit; }
        set { _onHit = value; }
    }

    public bool Active
    {
        get { return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }

    public void ApplyForce(Vector2 direction)
    {
        _rigibody.AddForce(direction);
    }

    public void Destroy()
    {
        if (!Active) { return; }

        BubbleLayout.Instance.PushBubble(this);
        Game.Instance.OnBubbleDestroy();
    }

    public void ProcessOutOfBounds()
    {
        if (CollisionLayer == Layer.ShootingBubble)
        {
            BubbleLayout.Instance.PushBubble(this);

            OnBubbleHit?.Invoke();
        }
    }

    public void ProcessLoss()
    {
        if (CollisionLayer == Layer.Bubble)
        {
            Game.Instance.Loss();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (CollisionLayer == Layer.ShootingBubble && collision.collider.gameObject.layer == (int)Layer.Bubble)
        {
            OnBubbleHit?.Invoke();

            var otherBubble = collision.collider.GetComponentInParent<Bubble>();

            BubbleLayout.Instance.StickBubbleToLayout(this, otherBubble);

            List<Bubble> dontDestroyThese = new List<Bubble>();
            dontDestroyThese.Add(this);

            /*var destroyCount = */
            BubbleLayout.Instance.DestroyBubblesAround(this, dontDestroyThese);
        }
    }

    private void FixedUpdate()
    {
        Velocity = Velocity.normalized * _targetSpeed;
    }
}