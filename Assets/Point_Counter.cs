using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;


public class Point_Counter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        points_amount("/Test.txt");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void points_amount(string path_file)
    {
       string File_Path = Application.dataPath + path_file;
       int lines_amount = 0;
       if(File.Exists(File_Path))
       {
           string text_cont = File.ReadAllText(File_Path);
           string[] lines = File.ReadAllLines(File_Path);
           lines_amount = lines.Length;
           string amount_to_write = lines_amount.ToString();
           Debug.Log("Amount of points " + amount_to_write);
           string new_text = amount_to_write + text_cont;
           
           File.WriteAllText(path_file, new_text);
         
       }
       else
       {
           Debug.Log("No file exists ");
      
       }
    }
}
