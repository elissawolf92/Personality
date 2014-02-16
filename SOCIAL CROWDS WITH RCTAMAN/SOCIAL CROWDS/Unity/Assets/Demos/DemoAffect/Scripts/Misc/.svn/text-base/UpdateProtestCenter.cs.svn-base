using UnityEngine;
using System.Collections;

public class UpdateProtestCenter : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        Vector3 location = Vector3.zero;
        int cnt = 0;
        AgentComponent[] agentComponents = FindObjectsOfType(typeof(AgentComponent)) as AgentComponent[];
        foreach (AgentComponent a in agentComponents) {
            if (a.role == (int)RoleName.Protester) {
                location += a.transform.position;
                cnt++;
            }
        }

        if(cnt > 0) {
            location /= cnt;
            transform.Translate(location - transform.position);
        }
    }
}