using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleport : MonoBehaviour
{
    public GameObject targetObj;
    public GameObject toObj;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            animator.SetBool("gate",true);
            targetObj = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            animator.SetBool("gate",false);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            StartCoroutine(TeleportRoutine());
        }
    }
    IEnumerator TeleportRoutine()
    {
        yield return null;
        targetObj.GetComponent<PlayerScript>().isControl = false;
        yield return new WaitForSeconds(0.5f);
        targetObj.transform.position = toObj.transform.position;
        yield return new WaitForSeconds(1f);
        targetObj.GetComponent<PlayerScript>().isControl = true;

    }
}
