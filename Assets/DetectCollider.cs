using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Movement))]
public class DetectCollider : MonoBehaviour
{
    private Movement movement;

    private void Start()
    {
        movement = this.gameObject.GetComponent<Movement>();
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Wall")
        {
            movement.Move(-2);
            movement.UndoRotationOnCollision();
            movement.speed = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
           GetComponent<Agent>().checkpoints = other.GetComponent<Checkpoint>().Id + 1;
        }
        if (other.gameObject.tag == "FinishLine")
        {
            GetComponent<Agent>().checkpoints = other.GetComponent<Checkpoint>().Id + 1;
            GetComponent<Agent>().endTime = Time.time;
            movement.finished = true;
        }
    }
}
