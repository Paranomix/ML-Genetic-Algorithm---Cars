using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public const float MAX_SPEED = 2f;
    public const float MIN_SPEED = -2f / 3;

    public const float ACCEL = 1f;
    public const int ANGULAR_SPEED = 60;

    public float speed = 0f;

    public bool rotate_left = false;
    public bool rotate_right = false;
    public bool forwards = false;
    public bool backards = false;

    public bool manual = false;
    public ManualControls controls;
    public Agent agent;

    public bool finished = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (finished)
            return;

        if (manual)
        {
            controls.UpdateControls();
            agent.CastRays();
        }
        else
            agent.UpdateControls();

        
        if (forwards)
        {
            if (speed < 0) // breaking
                speed += 2 * ACCEL * Time.deltaTime;
            else
                speed += ACCEL * Time.deltaTime;
        }
        else if (backards)
        {
            if (speed > 0) // breaking
                speed -= 2 * ACCEL * Time.deltaTime;
            else
                speed -= ACCEL * Time.deltaTime;
        }
        if (speed != 0)
        {
            if (rotate_left && speed > 0 || rotate_right && speed < 0)
            {
                this.transform.Rotate(new Vector3(0, Time.deltaTime * -ANGULAR_SPEED, 0));
            }
            else if (rotate_right && speed > 0 || rotate_left && speed < 0)
            {
                this.transform.Rotate(new Vector3(0, Time.deltaTime * ANGULAR_SPEED, 0));
            }

        }

        if (speed > MAX_SPEED)
            speed = MAX_SPEED;
        else if (speed < MIN_SPEED)
            speed = MIN_SPEED;

        Move(speed);
    }

    public void Move(float speed)
    {
            this.transform.position += new Vector3((float)Math.Cos(transform.rotation.eulerAngles.y / 360 * 2 * Math.PI) * speed * Time.deltaTime,
                0, (float)Math.Sin(transform.rotation.eulerAngles.y / 360 * 2 * Math.PI) * -speed * Time.deltaTime);
    }

    public void UndoRotationOnCollision()
    {
        if (rotate_left && speed > 0 || rotate_right && speed < 0)
        {
            this.transform.Rotate(new Vector3(0, Time.deltaTime * ANGULAR_SPEED, 0));
        }
        else if (rotate_right && speed > 0 || rotate_left && speed < 0)
        {
            this.transform.Rotate(new Vector3(0, Time.deltaTime * -ANGULAR_SPEED, 0));
        }
    }
}
