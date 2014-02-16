using UnityEngine;
using System.Collections;
using System.IO;
using System;

enum ExistCheck {
    Unchecked,
    Unique,
    TriedOnce
}

public class UserGUI : MonoBehaviour {

    bool male, female;
    static string ageStr ="0";
    static string nameStr = "";
    static string errorStr = "";
    static bool  showLabel = false;
    public string[] gender = new string[] {"Male", "Female"};
    public int genderInd = 0;


    int exists = (int)ExistCheck.Unchecked;
    string existingId = null;

    void OnGUI () {
      
     GUILayout.BeginArea (new Rect (Screen.width*0.3f, Screen.height*0.3f, Screen.width*0.5f, Screen.height*0.9f));
     GUIStyle style = new GUIStyle();
        style.fontSize = 18;
        style.normal.textColor = Color.white;             
        
        
        GUILayout.BeginHorizontal ();	
        GUILayout.Label ("User Name:  ", style);   
        nameStr = GUILayout.TextField(nameStr, 180);        
        GUILayout.EndHorizontal ();
        
       

        GUILayout.Space (10);

        GUILayout.BeginHorizontal ();	
        GUILayout.Label ("Gender:  ", style);  
          GUI.color = Color.white;  
        genderInd = GUILayout.SelectionGrid(genderInd, gender,2);



        if(genderInd == 0)
            UserInfo.isMale = true;
        else
            UserInfo.isMale = false;

        GUILayout.EndHorizontal ();

        

        GUILayout.Space (10);
        GUILayout.BeginHorizontal ();	
        GUILayout.Label ("Age:  ", style);   
        ageStr = GUILayout.TextField(ageStr, 2);        
        GUILayout.EndHorizontal ();
        int value;
        int.TryParse(ageStr, out value);
        UserInfo.age = value;

        GUILayout.Space (20);

       
        
        if (exists == (int)ExistCheck.Unique) { //to cause delay in coroutine output          
            this.StartCoroutine(PostUserInfo()); //post only if id does not exist                                            
          
            Application.LoadLevel("DemoLMA");
            
        }

        if (GUILayout.Button("Start")) {
            if (nameStr == "")
                errorStr = "Please enter a unique user name";
            else {
                if (exists == (int)ExistCheck.TriedOnce) {
                    if (existingId.Equals(nameStr)) { //entered a second time deliberately
                        exists = (int)ExistCheck.Unique;
                    }
                    else { //entered a different name
                        exists = (int)ExistCheck.Unchecked;
                        errorStr = null;
                        UserInfo.userId = nameStr;
                        this.StartCoroutine(IdExists());

                    }

                }
                else {
                    errorStr = null;
                    UserInfo.userId = nameStr;
                    this.StartCoroutine(IdExists());

                }

            }

        }

        style.fontSize = 20;
        
        style.normal.textColor = Color.white;  
        GUILayout.Label (errorStr, style);
         
 
        GUILayout.EndArea();

    }

    IEnumerator IdExists() {
        string resultURL = "https://fling.seas.upenn.edu/~fundad/cgi-bin/RCTAMAN/checkId.php";
           
        var form = new WWWForm();
        // Assuming the perl script manages high scores for different games
        form.AddField("userId", UserInfo.userId);  
        form.AddField("age", UserInfo.age.ToString()); ;
        if (UserInfo.isMale)
            form.AddField("gender", "male");
        else
            form.AddField("gender", "female");      

        // Create a download object
        var download = new WWW( resultURL,form);

        // Wait until the download is done
        yield return download;
        errorStr = download.text;

        if(download.error!= null) {    
            print( "Error: " + download.error );                         
        } else {
            if (download.text.Equals("true")) {

                exists = (int)ExistCheck.TriedOnce;
                existingId = UserInfo.userId;
                errorStr = "Username already exists. If this belongs to you please press Start. \n Otherwise please select another username.";
            }
            else {
                exists = (int)ExistCheck.Unique;
                errorStr = "";
            }                 
        }
    }

     IEnumerator PostUserInfo() {
         string resultURL = "https://fling.seas.upenn.edu/~fundad/cgi-bin/RCTAMAN/putUserInfo.php";

     // Create a form object for sending high score data to the server
        var form = new WWWForm();        
        form.AddField( "userId", UserInfo.userId);        
         form.AddField( "age", UserInfo.age.ToString());        
         if(UserInfo.isMale)
            form.AddField( "gender", "male");
         else
             form.AddField( "gender", "female");

         // Create a download object
        var download = new WWW( resultURL, form );



         
        // Wait until the download is done
        yield return download;


     }


        
}
