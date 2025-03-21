using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5.0f;
    private Animator animator;
    private bool isDancing = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDancing)
        {
            return;
        }

        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            movement += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += Vector3.right;
        }

        if (movement != Vector3.zero)
        {
            transform.Translate(movement.normalized * speed * Time.deltaTime, Space.World);
            animator.SetBool("walk", true);
            transform.forward = movement.normalized;
        }
        else
        {
            animator.SetBool("walk", false);
            animator.SetTrigger("idle");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(DanceRoutine());
        }
    }

    private IEnumerator DanceRoutine()
    {
        isDancing = true;
        animator.SetBool("walk", false);
        animator.SetTrigger("dance");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        isDancing = false;
        animator.SetTrigger("idle");
    }
}

