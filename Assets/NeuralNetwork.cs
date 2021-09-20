using System;
using UnityEngine;

static class Constants
{
    public const int number_of_inputs = 13;
    public const int size_of_layers = 10;
    public const int number_of_outputs = 6;
}

public class NeuralNetwork
{
    public float[] layer_1 = new float[Constants.size_of_layers] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
    public float[,] weights_1; // 13 x 10
    public float[] bias_1; // 10
    public float[,] weights_2; // 10 x 6
    public float[] bias_2; // 6

    public int[] Decision(float[] inputs)
    {
        for (int i = 0; i < layer_1.Length; i++)
        {
            layer_1[i] = 0;
            for (int j = 0; j < inputs.Length; j++)
            {
                layer_1[i] += inputs[j] * weights_1[j, i];
            }

            // Sigmoid (and adding the bias)
            float k = (float) Math.Exp(-layer_1[i] + bias_1[i]);
            layer_1[i] = 1 / (1.0f + k);
        }

        float[] output = new float[6] {0, 0, 0, 0, 0, 0};

        for (int i = 0; i < output.Length; i++)
        {
            for (int j = 0; j < layer_1.Length; j++)
            {
                output[i] += layer_1[j] * weights_2[j, i];
            }

            // Sigmoid (and adding the bias)
            float k = (float)Math.Exp(-output[i] + bias_2[i]);
            output[i] = 1 / (1.0f + k);
        }

        int speed;
        int steer;

        // deciding speed
        if (output[0] >= output[1] && output[0] >= output[2])
            speed = -1;
        else if (output[1] >= output[0] && output[1] >= output[2])
            speed = 0;
        else
            speed = 1;

        // deciding steer
        if (output[3] >= output[4] && output[3] >= output[5])
        {
            steer = -1;
        }
        else if (output[4] >= output[3] && output[4] >= output[5])
        {
            steer = 0;
        }
        else
            steer = 1;

        return new int[2] { speed, steer};
    }
}
