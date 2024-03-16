using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    public List<Transform> waypoints;
    public float moveSpeed = 3.0f;
    public float arrivalRadius = 0.5f;
    public float damageAmount = 10.0f;
    public float pushForce = 10.0f;

    private int currentWaypointIndex = 0;
    private bool movingForward = true;

    private void Update()
    {
        MoveToWaypoint();
    }

    private void MoveToWaypoint()
    {
        Vector3 direction = (waypoints[currentWaypointIndex].position - transform.position).normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) <= arrivalRadius)
        {
            if (movingForward)
            {
                currentWaypointIndex++;

                if (currentWaypointIndex >= waypoints.Count)
                {
                    currentWaypointIndex = 0;
                }
            }
            else
            {
                currentWaypointIndex--;

                if (currentWaypointIndex < 0)
                {
                    currentWaypointIndex = waypoints.Count - 1;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerHealth = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerHealth != null)
            {
                Vector3 pushDirection = (collision.transform.position - transform.position).normalized;
                collision.rigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                playerHealth.curHealth -= 10f;
            }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            waypoints.Reverse();
            movingForward = !movingForward;
        }
    }
}
