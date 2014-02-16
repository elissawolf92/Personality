using UnityEngine;
using System.Collections;


public class Expression : MonoBehaviour
{
   public Mesh angry, afraid, sad, happy, neutral;

   void Start(){
   }

   public void SetExpression(string expression) {
       if(expression == "angry")		
           GetComponent<SkinnedMeshRenderer>().sharedMesh = angry;		
       else if(expression == "afraid")
           GetComponent<SkinnedMeshRenderer>().sharedMesh = afraid;
		else if(expression == "sad")
           GetComponent<SkinnedMeshRenderer>().sharedMesh = sad;
		else if(expression == "happy")
           GetComponent<SkinnedMeshRenderer>().sharedMesh = happy;
		else
           GetComponent<SkinnedMeshRenderer>().sharedMesh = neutral;
     
   }
	
	void Update() {
	
		string exp = transform.parent.GetComponent<AffectComponent>().GetEkmanEmotion();		
		SetExpression (exp);

	}
}

