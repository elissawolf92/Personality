using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class CrowdEditor : EditorWindow {
		
	int agentCnt = 0;	//# of agents to be added
	static int totalAgentCnt = 0;
	int agentRole = 0; //audience
	string[] roleNames = {"Audience", "Provocateur", "Singer", "Protester", "Police", "Shopper", "Attacker", "Victim"};
	int[] roleInds = {0,1,2,3,4,5,6,7,8};
	
	[MenuItem ("ADAPT/Crowd")]
	 static void Init () {
        // Get existing open window or if none, make a new one:
        CrowdEditor window = (CrowdEditor)EditorWindow.GetWindow(typeof (CrowdEditor));		
		
	}
	
	void OnGUI () {
		
		
		GUILayout.Label ("Create New Group", EditorStyles.largeLabel);	
		
		//EditorGUILayout.BeginHorizontal();
		agentCnt = EditorGUILayout.IntField("Agent Count", agentCnt, GUILayout.ExpandWidth(true));
	
		agentRole = EditorGUILayout.IntPopup("Role: ", agentRole, roleNames,roleInds);
		if(GUILayout.Button("Add Group", GUILayout.ExpandWidth(false))) {

            int groupId = ComputeNewGroupId();
            GameObject go = new GameObject("CrowdGroup" + (groupId));
			GameObject crowd = GameObject.Find("Crowd");
			if(crowd == null)
			{
				crowd = new GameObject("Crowd");
                crowd.AddComponent<CrowdManager>();
			}
			
			go.transform.parent = GameObject.Find("Crowd").transform;
		//	if(selected == true) //Add to the selected location on screen
		//		go.transform.position = selectedPoint;
		//	else //add to the parent gameobject's location
			go.transform.position = go.transform.parent.transform.position;
			go.AddComponent<GroupBuilder>();			
			go.GetComponent<GroupBuilder>().Init(agentRole, agentCnt,  groupId, totalAgentCnt);
			totalAgentCnt += agentCnt;
			groupId++;
			
						
		}
		
		//EditorGUILayout.EndHorizontal ();		
	}

    int ComputeNewGroupId() {
        int id;
        int maxId = 0;

        foreach (GameObject g in UnityEngine.Object.FindObjectsOfType(typeof(GameObject))) {
            if (g.name.Contains("CrowdGroup") && g.transform.parent.name.Equals("Crowd")) {                
                id = Int32.Parse(g.name.Substring(10));                
                if (id > maxId)
                    maxId = id;
            }
        }
        return maxId + 1;       
    }
	
	
}
