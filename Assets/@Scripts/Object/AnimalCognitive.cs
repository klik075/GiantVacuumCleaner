using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalCognitive : MonoBehaviour
{
    public AnimalController animator;
    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponentInParent<AnimalController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            Vector3 direction = (transform.position - other.gameObject.transform.position);
            animator.IsWalking(direction);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            animator.IsIdle();
        }
    }
}
