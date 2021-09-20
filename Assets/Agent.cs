using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Agent : MonoBehaviour
{
    public const int REALLY_BIG_NUMBER = 1000000;
    public Movement movement;
    public float[] inputs = new float[Constants.number_of_inputs];
    public int[] outputs = new int[2];
    public NeuralNetwork brain = new NeuralNetwork();

    public int checkpoints = 0;

    public float endTime;

    public Level level;

    private void Start()
    {
        level.startPosition = transform.position;
        level.startRotation = transform.rotation.eulerAngles;
    }

    public void UpdateControls()
    {
        inputs[0] = movement.speed;
        CastRays();

        outputs = brain.Decision(inputs);

        // speed
        if (outputs[0] == -1)
        {
            movement.forwards = false;
            movement.backards = true;
        }
        if (outputs[0] == 0)
        {
            movement.forwards = false;
            movement.backards = false;
        }
        if (outputs[0] == 1)
        {
            movement.forwards = true;
            movement.backards = false;
        }

        // steer
        if (outputs[1] == -1)
        {
            movement.rotate_left = true;
            movement.rotate_right = false;
        }
        if (outputs[1] == 0)
        {
            movement.rotate_left = false;
            movement.rotate_right = false;
        }
        if (outputs[1] == 1)
        {
            movement.rotate_left = false;
            movement.rotate_right = true;
        }
    }

    public void CastRays()
    {
        int layerMask = 1 << 8;

        // positions (points)
        Vector3 center = transform.position + new Vector3((float)Math.Cos(transform.rotation.eulerAngles.y / 360 * 2 * Math.PI) * 0.1f,
                0, (float)Math.Sin(transform.rotation.eulerAngles.y / 360 * 2 * Math.PI) * -0.1f);
        Vector3 left = transform.position + new Vector3((float)Math.Cos(transform.rotation.eulerAngles.y / 360 * 2 * Math.PI) * 0.1f,
                0, (float)Math.Sin(transform.rotation.eulerAngles.y / 360 * 2 * Math.PI) * -0.1f)
            + new Vector3((float)Math.Cos((transform.rotation.eulerAngles.y - 90) / 360 * 2 * Math.PI) * 0.05f,
                0, (float)Math.Sin((transform.rotation.eulerAngles.y - 90) / 360 * 2 * Math.PI) * -0.05f);
        Vector3 right = transform.position + new Vector3((float)Math.Cos(transform.rotation.eulerAngles.y / 360 * 2 * Math.PI) * 0.1f,
                0, (float)Math.Sin(transform.rotation.eulerAngles.y / 360 * 2 * Math.PI) * -0.1f)
            + new Vector3((float)Math.Cos((transform.rotation.eulerAngles.y + 90) / 360 * 2 * Math.PI) * 0.05f,
                0, (float)Math.Sin((transform.rotation.eulerAngles.y + 90) / 360 * 2 * Math.PI) * -0.05f);
        Vector3 back = transform.position - new Vector3((float)Math.Cos(transform.rotation.eulerAngles.y / 360 * 2 * Math.PI) * 0.1f,
                0, (float)Math.Sin(transform.rotation.eulerAngles.y / 360 * 2 * Math.PI) * -0.1f);

        // directions (vectors)
        Vector3 forward = transform.position + new Vector3((float)Math.Cos(transform.rotation.eulerAngles.y / 360 * 2 * Math.PI) * 3,
                0, (float)Math.Sin(transform.rotation.eulerAngles.y / 360 * 2 * Math.PI) * -3) - center;

        float x = (float) Math.Cos(-15f / 360 * 2 * Math.PI) * forward.x + (float) Math.Sin(-15f / 360 * 2 * Math.PI) * forward.z;
        float z = (float)-Math.Sin(-15f / 360 * 2 * Math.PI) * forward.x + (float) Math.Cos(-15f / 360 * 2 * Math.PI) * forward.z;
        Vector3 left_dir_15 = new Vector3(x, 0, z);
        x = (float) Math.Cos(15f / 360 * 2 * Math.PI) * forward.x + (float) Math.Sin(15f / 360 * 2 * Math.PI) * forward.z;
        z = (float)-Math.Sin(15f / 360 * 2 * Math.PI) * forward.x + (float) Math.Cos(15f / 360 * 2 * Math.PI) * forward.z;
        Vector3 right_dir_15 = new Vector3(x, 0, z);
        x = (float) Math.Cos(-5f / 360 * 2 * Math.PI) * forward.x + (float) Math.Sin(-5f / 360 * 2 * Math.PI) * forward.z;
        z = (float)-Math.Sin(-5f / 360 * 2 * Math.PI) * forward.x + (float) Math.Cos(-5f / 360 * 2 * Math.PI) * forward.z;
        Vector3 left_dir_5 = new Vector3(x, 0, z);
        x = (float) Math.Cos(5f / 360 * 2 * Math.PI) * forward.x + (float) Math.Sin(5f / 360 * 2 * Math.PI) * forward.z;
        z = (float)-Math.Sin(5f / 360 * 2 * Math.PI) * forward.x + (float) Math.Cos(5f / 360 * 2 * Math.PI) * forward.z;
        Vector3 right_dir_5 = new Vector3(x, 0, z);
        x = (float)Math.Cos(-40f / 360 * 2 * Math.PI) * forward.x + (float)Math.Sin(-40f / 360 * 2 * Math.PI) * forward.z;
        z = (float)-Math.Sin(-40f / 360 * 2 * Math.PI) * forward.x + (float)Math.Cos(-40f / 360 * 2 * Math.PI) * forward.z;
        Vector3 left_dir_40 = new Vector3(x, 0, z);
        x = (float)Math.Cos(40f / 360 * 2 * Math.PI) * forward.x + (float)Math.Sin(40f / 360 * 2 * Math.PI) * forward.z;
        z = (float)-Math.Sin(40f / 360 * 2 * Math.PI) * forward.x + (float)Math.Cos(40f / 360 * 2 * Math.PI) * forward.z;
        Vector3 right_dir_40 = new Vector3(x, 0, z);
        x = (float)Math.Cos(-60f / 360 * 2 * Math.PI) * forward.x + (float)Math.Sin(-60f / 360 * 2 * Math.PI) * forward.z;
        z = (float)-Math.Sin(-60f / 360 * 2 * Math.PI) * forward.x + (float)Math.Cos(-60f / 360 * 2 * Math.PI) * forward.z;
        Vector3 left_dir_60 = new Vector3(x, 0, z);
        x = (float)Math.Cos(60f / 360 * 2 * Math.PI) * forward.x + (float)Math.Sin(60f / 360 * 2 * Math.PI) * forward.z;
        z = (float)-Math.Sin(60f / 360 * 2 * Math.PI) * forward.x + (float)Math.Cos(60f / 360 * 2 * Math.PI) * forward.z;
        Vector3 right_dir_60 = new Vector3(x, 0, z);

        RaycastHit hit;
        // forward ray
        if (Physics.Raycast(center, forward, out hit, Mathf.Infinity, layerMask))
        {
            inputs[1] = hit.distance;
        }
        else
            inputs[1] = REALLY_BIG_NUMBER;
        Debug.DrawRay(center, forward, Color.green);
        // forward-left ray
        if (Physics.Raycast(left, forward, out hit, Mathf.Infinity, layerMask))
        {
            inputs[2] = hit.distance;
        }
        else
            inputs[2] = REALLY_BIG_NUMBER;
        Debug.DrawRay(left, forward, Color.green);
        // forward-right ray
        if (Physics.Raycast(right, forward, out hit, Mathf.Infinity, layerMask))
        {
            inputs[3] = hit.distance;
        }
        else
            inputs[3] = REALLY_BIG_NUMBER;
        Debug.DrawRay(right, forward, Color.green);
        // left-left ray 15
        if (Physics.Raycast(left, left_dir_15, out hit, Mathf.Infinity, layerMask))
        {
            inputs[4] = hit.distance;
        }
        else
            inputs[4] = REALLY_BIG_NUMBER;
        Debug.DrawRay(left, left_dir_15, Color.green);
        // right-right ray 15
        if (Physics.Raycast(right, right_dir_15, out hit, Mathf.Infinity, layerMask))
        {
            inputs[5] = hit.distance;
        }
        else
            inputs[5] = REALLY_BIG_NUMBER;
        Debug.DrawRay(right, right_dir_15, Color.green);
        // left-left ray 5
        if (Physics.Raycast(left, left_dir_5, out hit, Mathf.Infinity, layerMask))
        {
            inputs[6] = hit.distance;
        }
        else
            inputs[6] = REALLY_BIG_NUMBER;
        Debug.DrawRay(left, left_dir_5, Color.green);
        // right-right ray 5
        if (Physics.Raycast(right, right_dir_5, out hit, Mathf.Infinity, layerMask))
        {
            inputs[7] = hit.distance;
        }
        else
            inputs[7] = REALLY_BIG_NUMBER;
        Debug.DrawRay(right, right_dir_5, Color.green);
        // left-left ray 40
        if (Physics.Raycast(left, left_dir_40, out hit, Mathf.Infinity, layerMask))
        {
            inputs[8] = hit.distance;
        }
        else
            inputs[8] = REALLY_BIG_NUMBER;
        Debug.DrawRay(left, left_dir_40, Color.green);
        // right-right ray 40
        if (Physics.Raycast(right, right_dir_40, out hit, Mathf.Infinity, layerMask))
        {
            inputs[9] = hit.distance;
        }
        else
            inputs[9] = REALLY_BIG_NUMBER;
        Debug.DrawRay(right, right_dir_40, Color.green);
        // left-left ray 60
        if (Physics.Raycast(left, left_dir_60, out hit, Mathf.Infinity, layerMask))
        {
            inputs[10] = hit.distance;
        }
        else
            inputs[10] = REALLY_BIG_NUMBER;
        Debug.DrawRay(left, left_dir_60, Color.green);
        // right-right ray 60
        if (Physics.Raycast(right, right_dir_60, out hit, Mathf.Infinity, layerMask))
        {
            inputs[11] = hit.distance;
        }
        else
            inputs[11] = REALLY_BIG_NUMBER;
        Debug.DrawRay(right, right_dir_60, Color.green);
        // backwards ray
        if (Physics.Raycast(back, -forward, out hit, Mathf.Infinity, layerMask))
        {
            inputs[12] = hit.distance;
        }
        else
            inputs[12] = REALLY_BIG_NUMBER;
        Debug.DrawRay(back, -forward, Color.green);
    }

    public float CalculateDistanceToTarget()
    {
        float distance_to_target = 0;
        for (int i = 0; i < level.checkpoints.Count; i++)
        {
            if (checkpoints == i)
            {
                distance_to_target += (level.checkpoints[i].position - transform.position).magnitude;
            }
            if (checkpoints < i)
            {
                distance_to_target += (level.checkpoints[i].position - level.checkpoints[i-1].position).magnitude;
            }
        }

        return distance_to_target;
    }
}
