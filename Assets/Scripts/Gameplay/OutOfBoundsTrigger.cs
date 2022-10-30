using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.layer == (int)Layer.ShootingBubble)
        {
            var bubble = collider.GetComponentInParent<Bubble>();
            bubble.ProcessOutOfBounds();
        }
    }
}
