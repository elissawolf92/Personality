using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class UserGUILowLevel : MonoBehaviour {

    static string idStr = "0";
    static string nameStr = "";
    static string errorStr = "";
    static bool  showLabel = false;
     bool exists = true;
    

    void Start() {        
        
     
    }

    void OnGUI () {
      
     GUILayout.BeginArea (new Rect (Screen.width*0.3f, Screen.height*0.3f, Screen.width*0.5f, Screen.height*0.9f));
     GUIStyle style = new GUIStyle();
        style.fontSize = 18;
        style.normal.textColor = Color.white;             
        
        
        GUILayout.BeginHorizontal ();	
        GUILayout.Label ("User Name:  ", style);   
        nameStr = GUILayout.TextField(nameStr, 180);        
        GUILayout.EndHorizontal ();
        
       

        
        if(GUILayout.Button("Start")){
            if (nameStr == "")
                errorStr = "Please enter a unique user name";
            else {
                      errorStr = "";
                    UserInfo.userId = nameStr;
                    this.StartCoroutine(PostUserInfo());
                    
                    Application.LoadLevel("DemoDrives");

     
            }
        }

      
        style.fontSize = 15;
        
        style.normal.textColor = Color.red;  
        GUILayout.Label (errorStr);
         
 
        GUILayout.EndArea();

    }

    IEnumerator IdExists() {
        string resultURL = "https://fling.seas.upenn.edu/~fundad/cgi-bin/checkId.php";
           
        var form = new WWWForm();
        // Assuming the perl script manages high scores for different games
        form.AddField( "userId", nameStr);        

        // Create a download object
        var download = new WWW( resultURL,form);

        // Wait until the download is done
        yield return download;
        

        if(download.error!= null) {    
            print( "Error: " + download.error );                         
        } else {
             if(download.text.Equals("true")) {
                 exists = true;                
                 errorStr = "Username exists\nPlease select another one";
             }
             else{
                exists = false;
                 errorStr = "";
                    UserInfo.userId = nameStr;
                    this.StartCoroutine(PostUserInfo());
                    Application.LoadLevel("DemoDrives");
             }
                      
        }
    }

     IEnumerator PostUserInfo() {
         string resultURL = "https://fling.seas.upenn.edu/~fundad/cgi-bin/putUserInfo.php";

     // Create a form object for sending high score data to the server
        var form = new WWWForm();        
        form.AddField( "userId", UserInfo.userId);        
         

         // Create a download object
        var download = new WWW( resultURL, form );



         
        // Wait until the download is done
        yield return download;


     }


        
}
