using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AIAgent : MonoBehaviour
{
    public float speed = 0.2f;
    public float rotationSpeed = 10f;

    Animator animator;
    Vector3 endPosition;
    int index = 0;
    bool moveFlag = false;

    List<Vector3> pathToGo = new List<Vector3>();

    private void Update()
    {
        if (moveFlag)
        {
            PerformMovement();
        }
    }

    private void OnDestroy()
    {
        OnDeath?.Invoke();
    }

    public event Action OnDeath;

    public void Initialize(List<Vector3> path)
    {
        pathToGo = path;
        index = 1;
        moveFlag = true;
        if (pathToGo.Count <= 1)
        {
            moveFlag = false;
            Destroy(gameObject);
            return;
        }
        endPosition = pathToGo[index];
        animator = GetComponent<Animator>();
        animator.SetTrigger("Walk");
    }

    private void PerformMovement()
    {
        if (pathToGo.Count > index)
        {
            float distanceToGo = MoveTheAgent();
            if (distanceToGo < 0.05f)
            {
                index++;
                if (index >= pathToGo.Count)
                {
                    moveFlag = false;
                    Destroy(gameObject);
                    return;
                }

                endPosition = pathToGo[index];
            }
        }
    }

    private float MoveTheAgent()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, endPosition, step);

        var lookDirection = endPosition - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection),
            Time.deltaTime * rotationSpeed);
        return Vector3.Distance(transform.position, endPosition);
    }
}