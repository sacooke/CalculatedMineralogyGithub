using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Linq;


//perhaps also store fileInput.text locally?

public class FileChooser : MonoBehaviour {

	public GameObject titleObject;

	public GameObject button;

	public GameObject fileListPanel;

    public InputField fileInput;

    public InputField driveInput;

    public InputField directoryInput;

    public Text placeholder;

    public Text bannerFileName;

    public Text driveText;

    public Text drivePlaceholderText;

    public Text directoryPlaceholderText;

    public Button openSaveButton;

    public Button driveButton;

    public Button directoryButton;


#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    public static string SLASH="\\";
    public static bool isWindows = true;
#else
	public static string SLASH="/";
    public static bool isWindows = false;
#endif

    void Start () {
#if	UNITY_STANDALONE_OSX
        driveInput.gameObject.SetActive(false);
        driveButton.transform.GetChild(0).GetComponent<Text>().text = "External Drives";
#endif
		if(dir==""){
			SetStartingDirectory();
			showFiles ();	
		}
	}

    private void OnGUI()
    {
        if (directoryInput.isFocused && Input.GetKey(KeyCode.Return))
        {
            changeDirectory();
        }
        if (driveInput.isFocused && Input.GetKey(KeyCode.Return))
        {
            changeDrive();
        }
        if (fileInput.isFocused && Input.GetKey(KeyCode.Return))
        {
            Yes();
        }
    }
    

	void SetStartingDirectory(){
        //Sets the directory to the current directory
        //dir = Directory.GetCurrentDirectory();

        if (PlayerPrefs.HasKey("fileDirectory"))
        {
            //Sets the directory to the most recently accessed folder
            //string filePath = PlayerPrefs.GetString("fileDirectory");
            //print("loading playerPref \"fileDirectory\" "+filePath);
            dir = PlayerPrefs.GetString("fileDirectory");
        }
        else
        {
            //Sets the directory to the documents folder 
            dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
	}


	public enum OPENSAVE { OPEN, SAVE };

	OPENSAVE openSave = OPENSAVE.OPEN;

	public void setup(OPENSAVE os, string ext)
    {
        extensions.Clear();

		if(ext!=null && ext!="") {
			string[] exts = ext.Split ( '|' );
			for(int i=0;i<exts.Length;i++){
				extensions.Add("."+exts[i]);
			}
		}
		openSave = os;

		//set text on buttons
		if(openSave==OPENSAVE.SAVE)
			openSaveButton.transform.FindChild ("Text").GetComponent<Text>().text = "Save";
		if(openSave==OPENSAVE.OPEN)
			openSaveButton.transform.FindChild ("Text").GetComponent<Text>().text = "Open";


		placeholder.text = "";

		if(extensions.Count>0){
			string pattern="";
			for(int i=0;i<extensions.Count;i++){
				pattern+="*"+extensions[i];
				if(i<extensions.Count-1) pattern+=";";
			}
			placeholder.text = pattern;
			filter = WildCardToRegex(pattern);
		}

		Show (true);
	}

	public void Show(bool show)
    {
        fileInput.text = "";
		if(dir=="") SetStartingDirectory();
		showFiles ();
		gameObject.SetActive(true);
        if(dir!="")
        {
            drivePlaceholderText.text = dir.Substring(0, 3);
        }
	}

	string dir="";

	//list of extensions that we are looking for
	List<string> extensions = new List<string>();
	

	//convert a Wild Card pattern e.g. a??b.* to a RegEx expression:
	public static string WildCardToRegex(string pattern)
    {
        return "^" + Regex.Escape(pattern).
			Replace (";" , "$|^").
			Replace("\\*", ".*").
			Replace("\\?", ".") + "$";
	}
	
	string filter=""; 
	string lastfilter="";

	public void EndEdit()
    {
        driveText.text = "";

        //This whole section just ends the saving prematurely, so I don't see the point in keeping it around
        if (false)
        {
            checkWildCard();
            if (fileInput.text.Length > 0 && (fileInput.text[fileInput.text.Length - 1] == '/'
                                            || fileInput.text[fileInput.text.Length - 1] == '\\'))
            {
                //remove last character
                fileInput.text = fileInput.text.Substring(0, fileInput.text.Length - 1);
            }

            if (fileInput.text != "" && Directory.Exists(dir + SLASH + fileInput.text))
            {
                dir = dir + SLASH + fileInput.text;
                fileInput.text = "";
                showFiles();
                return;
            }
            /*Debug.Log("filter = " + filter);
            if (filter == "")
            {
                Yes();
            }
            if (filter != lastfilter) showFiles();
            lastfilter = filter;*/
        }
	}

	void checkWildCard()
    {
        if ( fileInput.text.Contains ("?") || fileInput.text.Contains("*") ){
			filter = WildCardToRegex (fileInput.text);
		}else filter="";
	}

	void showFiles(){
        
        //remove all buttons 
        foreach (Transform T in fileListPanel.transform){
			Destroy(T.gameObject);
		}

        string[] dirs;
        try
        {
            dirs = Directory.GetDirectories(dir);
        }
		catch(DirectoryNotFoundException e)
        {
            //print("Directory saved in PlayerPrefs does not exist - setting path to documents folder");
            dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dirs = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            //print("done");
        }
        setTitle(dir + SLASH);
        for (int i=0;i<dirs.Length;i++){
			GameObject newButton=(GameObject)Instantiate(button);
			newButton.transform.FindChild ("Text").GetComponent<Text>().text = 
				Path.GetFileName(dirs[i])+SLASH;
			newButton.transform.SetParent( fileListPanel.transform ); 
			newButton.transform.localScale = Vector3.one;
            newButton.GetComponent<Image>().color = new Color(1f, 0.9f, 0.7f, 0.6f);

			int j=i;
			Button.ButtonClickedEvent BCE=new Button.ButtonClickedEvent();
			BCE.AddListener(  ()=>{ 
				dir = dirs[j];
				showFiles ();
			}  );
			newButton.GetComponent<Button>().onClick = BCE;
	
			
		}

		//Debug.Log ("Matching "+filter);


		string[] files = Directory.GetFiles (dir);	
		for(int i=0;i<files.Length;i++){

			string fileName = Path.GetFileName(files[i]);
			//skip if doesn't match pattern:
			if(filter!="" && !Regex.IsMatch (fileName,filter,RegexOptions.IgnoreCase)) continue;

			GameObject newButton=(GameObject)Instantiate(button);
			newButton.transform.FindChild ("Text").GetComponent<Text>().text = fileName;
			newButton.transform.SetParent ( fileListPanel.transform );
			newButton.transform.localScale = Vector3.one;

			Button.ButtonClickedEvent BCE=new Button.ButtonClickedEvent();
			BCE.AddListener(  ()=>{ 
				fileInput.text = fileName; 
				Yes ();

			}  );
			newButton.GetComponent<Button>().onClick = BCE;

		}
        directoryInput.text = dir;
        directoryPlaceholderText.text = dir;

	}

    public void upDirectory()
    {
        driveText.text = "";
		if(Directory.GetParent(dir)!=null){
			dir = Directory.GetParent(dir).FullName;
			showFiles ();
		}
	}

    public void changeDrive()
    {
        if (isWindows)
        {
            if (!driveInput.text.Equals(""))
            {
                string[] drives = Directory.GetLogicalDrives();
                /*foreach (string drive in drives)
                    Debug.Log(drive);
                Debug.Log(driveInput.text);*/
                string sub = (driveInput.text.Substring(0, 1) + ":\\").ToUpper();
                //Debug.Log("sub: " + sub + ", driveinfo 0: " + drives[0]);
                driveInput.text = "";
                bool foundDrive = false;
                foreach (string drive in drives)
                    if (sub.Equals(drive))
                        foundDrive = true;
                if (foundDrive == false)
                {
                    //Debug.Log("Drive not found.");
                    bool isLetter = false;
                    char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
                    foreach (char letter in alpha)
                    {
                        if (sub.Contains(letter))
                            isLetter = true;
                    }
                    if (isLetter)
                        driveText.text = "Drive " + sub + " could not be found";
                    else
                        driveText.text = "Not a valid drive";

                    //displayDriveText(sub, " not found");
                }
                else
                {
                    //Debug.Log("Drive found.");
                    //Debug.Log("Loading directory...");
                    if (Directory.Exists(sub))
                    {
                        dir = sub;
                        showFiles();
                        //Debug.Log("Directory loaded (supposedly).");
                        driveText.text = "";
                        drivePlaceholderText.text = sub;
                    }
                    else
                    {
                        //Debug.Log("Directory could not be loaded.");
                        //displayDriveText(sub, " could not be loaded");
                        driveText.text = "Drive " + sub + " could not be loaded";
                    }
                }
            }
        }
        else
        {
            if (Directory.Exists("/Volumes"))
            {
                dir = "/Volumes";
                showFiles();
                //Debug.Log("Directory loaded (supposedly).");
            }
            else
            {
                //Debug.Log("Directory could not be loaded.");
                //displayDriveText(sub, " could not be loaded");
                driveText.text = "'Volumes' could not be loaded";
            }

        }
    }

    public void changeDirectory()
    {
        if (isWindows)
        {
            if (!directoryInput.text.Equals(""))
            {
                string sub = directoryInput.text;
                //Debug.Log("sub: " + sub + ", driveinfo 0: " + drives[0]);

                //Debug.Log("Drive found.");
                //Debug.Log("Loading directory...");
                if (Directory.Exists(sub))
                {
                    dir = sub;
                    showFiles();
                    //Debug.Log("Directory loaded (supposedly).");
                    //drivePlaceholderText.text = sub;
                }
                else
                {
                    //Debug.Log("Directory could not be loaded.");
                    //displayDriveText(sub, " could not be loaded");
                    driveText.text = "Directory could not be loaded";
                    directoryInput.text = directoryPlaceholderText.text;
                }
            }
        }
        else
        {
            if (Directory.Exists("/Volumes"))
            {
                dir = "/Volumes";
                showFiles();
                //Debug.Log("Directory loaded (supposedly).");
            }
            else
            {
                //Debug.Log("Directory could not be loaded.");
                //displayDriveText(sub, " could not be loaded");
                driveText.text = "'Volumes' could not be loaded";
            }

        }
    }

    public void setTitle(string text)
    {
        titleObject.GetComponent<Text>().text = text;
	}

	//for load, loop through extensions. Don't return if file doesn't exist.
	//test on Mac

    public void Yes()
    {
        driveText.text = "";
		string fullFilename = dir+SLASH+fileInput.text;
		//add the first extension in the list if none specified
		if(openSave == OPENSAVE.SAVE){
			if(!fileInput.text.Contains (".") && extensions.Count>0){
				fullFilename+=extensions[0];
			}
		}else{
			//if not extension provided try all the extensions
			if(!fileInput.text.Contains (".") && extensions.Count>0){
				for(int i=0;i<extensions.Count;i++){
					string testFilename = fullFilename+extensions[i];
					if(File.Exists( testFilename)){
						fullFilename = testFilename;
						break;
					}
				}
			}
			//if file still doesn't exist don't return
			if( !File.Exists( fullFilename) ) {
				Debug.Log ("Can't return. File does not exist: "+fullFilename);
				return;
			}
            int charPos = fullFilename.LastIndexOf('.');
            if (charPos > -1)
            {
                //Debug.Log("Changing banner text: " + fullFilename);
                if (fullFilename.Substring(charPos + 1) == "kmz" || fullFilename.Substring(charPos + 1) == "gvx")
                {
                    if (fullFilename.Length <= 70)
                    {
                        bannerFileName.text = fullFilename;
                    }
                    else
                    {
                        string[] directoryComponents = dir.Split('\\');
                        bannerFileName.text = directoryComponents[0] + SLASH + "..." + SLASH + directoryComponents[directoryComponents.Length - 1] + SLASH + fileInput.text;
                    }
                }
                else
                {
                    //Debug.Log("Did not load a kmz or other file");
                }
            }
            else
                Debug.Log(". not found - file invalid");
            
            //Export.fileName = fileInput.text;
		}

        PlayerPrefs.SetString("fileDirectory", dir);

		if(callbackYes!=null)
			callbackYes(fullFilename);


	}

    public void No()
    {
        driveText.text = "";
		if(callbackNo!=null)
			callbackNo();
	}
	
	public delegate void Del1();
	public delegate void Del2(string s);
	
	public Del2 callbackYes;
	public Del1 callbackNo;

}
