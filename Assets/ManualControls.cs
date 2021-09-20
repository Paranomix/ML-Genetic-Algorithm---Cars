using UnityEngine;

public class ManualControls : MonoBehaviour
{
    public Movement movement;

    public void UpdateControls()
    {
        // cleaning previous inputs
        movement.forwards = false;
        movement.backards = false;
        movement.rotate_left = false;
        movement.rotate_right = false;

        if (Input.GetKey(KeyCode.W))
            movement.forwards = true;
        else if (Input.GetKey(KeyCode.S))
            movement.backards = true;

        if (Input.GetKey(KeyCode.A))
            movement.rotate_left = true;
        else if (Input.GetKey(KeyCode.D))
            movement.rotate_right = true;
    }
}
