// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ItemObject : MonoBehaviour
// {
//     private ItemStack itemStack;
//     private Rigidbody2D rb2d;

//     public ItemStack ItemStack { get => itemStack; set => itemStack = value; }
//     public Rigidbody2D Rb2d { get => rb2d; set => rb2d = value; }

//     private void Awake() {
//         rb2d = this.GetComponent<Rigidbody2D>();
//     }

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.tag == "Player")
//         {
//             GameState.GetInstance()._PlayerState.playerInventory.AddItem(ItemStack);
//             StartCoroutine(AnimateCollection(other.transform));
//         }
//     }

//     public void Move(Vector2 force)
//     {
//         Rb2d.AddForce(force, ForceMode2D.Impulse);
//     }

//     private IEnumerator AnimateCollection(Transform playerT)
//     {
//         Transform t = this.transform;

//         while (Vector2.Distance(playerT.position, t.position) > 0.5f)
//         {
//             Rb2d.AddForce((playerT.position - t.position) * 20);
//             yield return null;
//         }

//         GameObject.Destroy(this.gameObject);
//     }
// }
