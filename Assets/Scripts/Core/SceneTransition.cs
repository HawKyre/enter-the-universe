using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public float animationLength;
    [SerializeField] private Animator anim;

    public void GoToScene(int scene)
    {
        StartCoroutine(AnimateTransition(scene)); 
    }

    private IEnumerator AnimateTransition(int scene)
    {
        anim.SetTrigger("ChangeScene");

        yield return new WaitForSeconds(animationLength == 0 ? 1 : animationLength);
        SceneManager.LoadScene(scene);
    }

}