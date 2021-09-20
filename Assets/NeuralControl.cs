using System.Collections.Generic;
using UnityEngine;

public class NeuralControl : MonoBehaviour
{
    public const float MARGIN = 0.00001f;
    public const float RANDOM_PERCENTAGE = 0.1f;

    public List<Agent> agents;
    public bool manual = false;
    public bool autosaveNeuralNetwork = true;
    public bool loadNeuralNetwork = false;
    public int gen = 0; // new, every 10 secs
    public float time = 0;
    public int level = 0;
    public GameObject[] levels;

    public int time_for_this_level = 10;

    private bool setNextLevel = false;

    private void Start()
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag("Player");

        foreach (var agent in array)
        {
            agents.Add(agent.GetComponent<Agent>());
        }

        foreach (Agent agent in agents)
        {
            agent.movement.manual = manual;
            agent.endTime = time_for_this_level;
        }

        if (!manual && !loadNeuralNetwork)
        {
            GenerateRandomNeuralNetwork();
        }
        else if (loadNeuralNetwork)
        {
            LoadBrains();
            int number_of_agents_saved = agents.Count / 3;
            List<int> agents_saved = new List<int>();
            for (int i = 0; i < number_of_agents_saved; i++)
            {
                agents_saved.Add(i);
            }
            GenerateNewAgentsNetwork(agents_saved);
        }
    }

    private void FixedUpdate()
    {
        if (Time.time - time >= time_for_this_level) // FIXME
        {
            // new gen
            gen++;

            // eliminate shit ones
            int number_of_remaining_agents = agents.Count / 3;
            List<int> agents_to_save = new List<int>();

            float max_distance = agents[0].CalculateDistanceToTarget(); // worst of the best
            float max_time = agents[0].endTime; // worst of the best
            agents_to_save.Add(0);
            number_of_remaining_agents--;


            for (int i = 1; i < agents.Count; i++)
            {
                if (number_of_remaining_agents > 0)
                {
                    number_of_remaining_agents--;
                    agents_to_save.Add(i);
                }
                else
                {
                    agents_to_save.Sort(CompareAgentsResults);
                    
                    max_distance = agents[agents_to_save[agents_to_save.Count - 1]].CalculateDistanceToTarget();
                    max_time = agents[agents_to_save[agents_to_save.Count - 1]].endTime;

                    if (Mathf.Abs(max_distance - agents[i].CalculateDistanceToTarget()) < MARGIN)
                    {
                        if (max_time > agents[i].endTime)
                        {
                            agents_to_save.RemoveAt(agents_to_save.Count - 1);

                            agents_to_save.Add(i);
                        }
                    }
                    else if (max_distance > agents[i].CalculateDistanceToTarget())
                    {
                        agents_to_save.RemoveAt(agents_to_save.Count - 1);

                        agents_to_save.Add(i);
                    }
                }
            }

            agents_to_save.Sort(CompareAgentsResults);

            max_distance = agents[agents_to_save[agents_to_save.Count - 1]].CalculateDistanceToTarget();
            max_time = agents[agents_to_save[agents_to_save.Count - 1]].endTime;

            Debug.Log("Gen " + gen + " worst best distance(and time): " + max_distance + "(" + max_time + ").");
            string s = "Agents best scores: ";
            for (int i = 0; i < agents_to_save.Count; i++)
            {
                s += agents[agents_to_save[i]].CalculateDistanceToTarget() + " (" + agents_to_save[i] + ") ";
            }
            Debug.Log(s);

            // auto-save on Player Prefs
            if (autosaveNeuralNetwork)
                AutoSave(agents_to_save);

            // crossfit and adds some randomness to created new ones
            GenerateNewAgentsNetwork(agents_to_save);


            if (setNextLevel)
                SetNextLevel();

            Reset_Level();
        }
    }

    private int CompareAgentsResults(int x, int y)
    {

        if (Mathf.Abs(agents[x].CalculateDistanceToTarget() - agents[y].CalculateDistanceToTarget()) < MARGIN)
        {
            if (agents[x].endTime > agents[y].endTime)
            {
                return 1; // x is greater
            }
            else
                return -1; // y is greater
        }

        if (agents[x].CalculateDistanceToTarget() > agents[y].CalculateDistanceToTarget())
        {
            return 1; // x is greater
        }
        else
            return -1; // y is greater
    }

    public void GenerateRandomNeuralNetwork()
    {
        foreach (Agent agent in agents)
        {
            // generate weights_1
            agent.brain.weights_1 = new float[Constants.number_of_inputs, Constants.size_of_layers];
            for (int i = 0; i < Constants.number_of_inputs; i++)
            {
                for (int j = 0; j < Constants.size_of_layers; j++)
                {
                    agent.brain.weights_1[i, j] = 10 * Random.value;
                }
            }

            // generate weights_2
            agent.brain.weights_2 = new float[Constants.size_of_layers, Constants.number_of_outputs];
            for (int i = 0; i < Constants.size_of_layers; i++)
            {
                for (int j = 0; j < Constants.number_of_outputs; j++)
                {
                    agent.brain.weights_2[i, j] = 10 * Random.value;
                }
            }

            // generate bias_1
            agent.brain.bias_1 = new float[Constants.size_of_layers];
            for (int i = 0; i < Constants.size_of_layers; i++)
            {
                agent.brain.bias_1[i] = 50 * Random.value;
            }

            // generate bias_2
            agent.brain.bias_2 = new float[Constants.size_of_layers];
            for (int i = 0; i < Constants.number_of_outputs; i++)
            {
                agent.brain.bias_2[i] = 50 * Random.value;
            }
        }
    }

    public void GenerateNewAgentsNetwork(List<int> saved)
    {
        int gen_modifier = gen / 100 + 1;

        int k = 0;
        foreach (Agent agent in agents)
        {
            bool replaceable = true;
            for (int i = 0; i < saved.Count; i++)
            {
                if (k == saved[i])
                {
                    replaceable = false;
                }
            }
            k++;
            if (!replaceable)
            {
                continue;
            }

            // generate weights_1
            agent.brain.weights_1 = new float[Constants.number_of_inputs, Constants.size_of_layers];
            for (int i = 0; i < Constants.number_of_inputs; i++)
            {
                for (int j = 0; j < Constants.size_of_layers; j++)
                {
                    float original = agents[saved[Random.Range(0, saved.Count)]].brain.weights_1[i, j];
                    if (Random.value > RANDOM_PERCENTAGE)
                        agent.brain.weights_1[i, j] = original + 20 * (Random.value - 0.5f) / gen_modifier;
                    else
                        agent.brain.weights_1[i, j] = original + 20 * (Random.value - 0.5f);
                }
            }

            // generate weights_2
            agent.brain.weights_2 = new float[Constants.size_of_layers, Constants.number_of_outputs];
            for (int i = 0; i < Constants.size_of_layers; i++)
            {
                for (int j = 0; j < Constants.number_of_outputs; j++)
                {
                    float original = agents[saved[Random.Range(0, saved.Count)]].brain.weights_2[i, j];
                    if (Random.value > RANDOM_PERCENTAGE)
                        agent.brain.weights_2[i, j] = original + 20 * (Random.value - 0.5f) / gen_modifier;
                    else
                        agent.brain.weights_2[i, j] = original + 20 * (Random.value - 0.5f);
                }
            }

            // generate bias_1
            agent.brain.bias_1 = new float[Constants.size_of_layers];
            for (int i = 0; i < Constants.size_of_layers; i++)
            {
                float original = agents[saved[Random.Range(0, saved.Count)]].brain.bias_1[i];
                if (Random.value > RANDOM_PERCENTAGE)
                    agent.brain.bias_1[i] = original + 100 * (Random.value - 0.5f) / gen_modifier;
                else
                    agent.brain.bias_1[i] = original + 100 * (Random.value - 0.5f);
            }

            // generate bias_2
            agent.brain.bias_2 = new float[Constants.size_of_layers];
            for (int i = 0; i < Constants.number_of_outputs; i++)
            {
                float original = agents[saved[Random.Range(0, saved.Count)]].brain.bias_2[i];
                if (Random.value > RANDOM_PERCENTAGE)
                    agent.brain.bias_2[i] = original + 100 * (Random.value - 0.5f) / gen_modifier;
                else
                    agent.brain.bias_2[i] = original + 100 * (Random.value - 0.5f);
            }
        }
    }

    /// <summary>
    /// start new gen; reset finished flag, reset position, reset endTime on agents
    /// </summary>
    private void Reset_Level()
    {
        time = Time.time;

        int k = 0;
        foreach (Agent agent in agents)
        {
            agent.endTime = time + time_for_this_level;
            agent.movement.finished = false;
            agent.transform.position = agent.level.startPosition;
            agent.transform.rotation = Quaternion.Euler(agent.level.startRotation);
            agent.checkpoints = 0;
            agent.movement.speed = 0f;

            k++;
        }
    }

    private void SetNextLevel()
    {
        setNextLevel = false;

        levels[level].gameObject.SetActive(false);

        level++;

        levels[level].gameObject.SetActive(true);

        GameObject[] array = GameObject.FindGameObjectsWithTag("Player");

        int k = 0;
        foreach (var agent in array)
        {
            NeuralNetwork brain = agent.GetComponent<Agent>().brain;
            brain.weights_1 = agents[k].brain.weights_1;
            brain.weights_2 = agents[k].brain.weights_2;
            brain.bias_1 = agents[k].brain.bias_1;
            brain.bias_2 = agents[k].brain.bias_2;

            k++;
        }

        agents = new List<Agent>();

        foreach (var agent in array)
        {
            agents.Add(agent.GetComponent<Agent>());
        }

        foreach (Agent agent in agents)
        {
            agent.movement.manual = manual;

            agent.level.startPosition = agent.transform.position;
            agent.level.startRotation = agent.transform.rotation.eulerAngles;
        }

        time_for_this_level = 18; // FIXME Should be something kept on the level, not hardcoded
    }

    // FIXME IMPORT, EXPORT
    void AutoSave(List<int> agents_ids)
    {
        for (int agent_id = 0; agent_id < agents_ids.Count; agent_id++)
        {
            // layer 1
            for (int layer_node_id = 0; layer_node_id < Constants.size_of_layers; layer_node_id++)
            {
                PlayerPrefs.SetFloat("Agent_" + agent_id + "_layer_1_" + layer_node_id, agents[agents_ids[agent_id]].brain.layer_1[layer_node_id]);
            }
            // weights 1
            for (int weight_line = 0; weight_line < Constants.number_of_inputs; weight_line++)
            {
                for (int weight_column = 0; weight_column < Constants.size_of_layers; weight_column++)
                {
                    PlayerPrefs.SetFloat("Agent_" + agent_id + "_weights_1_" + weight_line + "_" + weight_column, agents[agents_ids[agent_id]].brain.weights_1[weight_line, weight_column]);
                }
            }
            // weights 2
            for (int weight_line = 0; weight_line < Constants.size_of_layers; weight_line++)
            {
                for (int weight_column = 0; weight_column < Constants.number_of_outputs; weight_column++)
                {
                    PlayerPrefs.SetFloat("Agent_" + agent_id + "_weights_2_" + weight_line + "_" + weight_column, agents[agents_ids[agent_id]].brain.weights_2[weight_line, weight_column]);
                }
            }
            // bias 1
            for (int bias_id = 0; bias_id < Constants.size_of_layers; bias_id++)
            {
                PlayerPrefs.SetFloat("Agent_" + agent_id + "_bias_1_" + bias_id, agents[agents_ids[agent_id]].brain.bias_1[bias_id]);
            }
            // bias 2
            for (int bias_id = 0; bias_id < Constants.number_of_outputs; bias_id++)
            {
                PlayerPrefs.SetFloat("Agent_" + agent_id + "_bias_2_" + bias_id, agents[agents_ids[agent_id]].brain.bias_2[bias_id]);
            }
        }
    }

    void LoadBrains()
    {

        for (int agent_id = 0; agent_id < agents.Count; agent_id++)
        {
            bool exist = Mathf.Abs(PlayerPrefs.GetFloat(HashKeyPlayerPrefs("Agent_" + agent_id + "_layer_1_" + 0), -1000) + 1000) > MARGIN;
            if (!exist)
                break;

            // layer 1
            for (int layer_node_id = 0; layer_node_id < Constants.size_of_layers; layer_node_id++)
            {
                agents[agent_id].brain.layer_1[layer_node_id] = PlayerPrefs.GetFloat(HashKeyPlayerPrefs("Agent_" + agent_id + "_layer_1_" + layer_node_id));
            }
            // weights 1
            agents[agent_id].brain.weights_1 = new float[Constants.number_of_inputs, Constants.size_of_layers];
            for (int weight_line = 0; weight_line   < Constants.number_of_inputs; weight_line++)
            {
                for (int weight_column = 0; weight_column < Constants.size_of_layers; weight_column++)
                {
                    agents[agent_id].brain.weights_1[weight_line, weight_column] = PlayerPrefs.GetFloat(HashKeyPlayerPrefs("Agent_" + agent_id + "_weights_1_" + weight_line + "_" + weight_column));
                }
            }
            // weights 2
            agents[agent_id].brain.weights_2 = new float[Constants.size_of_layers, Constants.number_of_outputs];
            for (int weight_line = 0; weight_line < Constants.size_of_layers; weight_line++)
            {
                for (int weight_column = 0; weight_column < Constants.number_of_outputs; weight_column++)
                {
                    agents[agent_id].brain.weights_2[weight_line, weight_column] = PlayerPrefs.GetFloat(HashKeyPlayerPrefs("Agent_" + agent_id + "_weights_2_" + weight_line + "_" + weight_column));
                }
            }
            // bias 1
            agents[agent_id].brain.bias_1 = new float[Constants.size_of_layers];
            for (int bias_id = 0; bias_id < Constants.size_of_layers; bias_id++)
            {
                agents[agent_id].brain.bias_1[bias_id] = PlayerPrefs.GetFloat(HashKeyPlayerPrefs("Agent_" + agent_id + "_bias_1_" + bias_id));
            }
            // bias 2
            agents[agent_id].brain.bias_2 = new float[Constants.size_of_layers];
            for (int bias_id = 0; bias_id < Constants.number_of_outputs; bias_id++)
            {
                agents[agent_id].brain.bias_2[bias_id] = PlayerPrefs.GetFloat(HashKeyPlayerPrefs("Agent_" + agent_id + "_bias_2_" + bias_id));
            }
        }

    }

    // FIXME end simulation
    // FIXME maybe keep some completely random values or brains

    public void NextLevel()
    {
        setNextLevel = true;
    }

    private string HashKeyPlayerPrefs(string key)
    {
        uint hash = 5381;
        foreach (char c in key)
            hash = hash * 33 ^ c;

        return key + "_h" + hash;
    }
}
