using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LossTrigger : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == (int)Layer.Bubble)
        {
            var bubble = collision.GetComponentInParent<Bubble>();
            bubble.ProcessLoss();
        }
    }
}
