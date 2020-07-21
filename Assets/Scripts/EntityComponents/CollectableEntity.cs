using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableEntity : MonoBehaviour
{
    private CircleCollider2D collectTrigger;
    private Rigidbody2D rb2d;
    private ItemStack itemStack;

    private void Awake()
    {
        rb2d = this.gameObject.AddComponent<Rigidbody2D>();
        rb2d.gravityScale = 0;
        rb2d.drag = 2.5f;
        collectTrigger = this.gameObject.AddComponent<CircleCollider2D>();
        collectTrigger.radius = 2;
        collectTrigger.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(AnimateCollection(other.transform));
        }
    }

    public void Move(Vector2 force)
    {
        if (!rb2d)
        {
            rb2d = GetComponent<Rigidbody2D>();
        }
        rb2d.AddForce(force, ForceMode2D.Impulse);
    }

    public void Merge(CollectableEntity c)
    {
        throw new NotImplementedException();
    }

    private IEnumerator AnimateCollection(Transform playerT)
    {
        Transform t = this.transform;

        while (Vector2.Distance(playerT.position, t.position) > 0.5f)
        {
            rb2d.AddForce((playerT.position - t.position) * 20);
            yield return null;
        }

        // Add it after you collect it
        GameState.GetInstance()._PlayerInventory.AddItem(itemStack);

        GameObject.Destroy(this.gameObject);
    }

    public void SetCollectible(ItemStack i)
    {
        itemStack = i;
    }
}
