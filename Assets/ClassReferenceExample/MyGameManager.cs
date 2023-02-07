using UnityEngine;
using System.Collections.Generic;
using System;

public class MyGameManager : MonoBehaviour
{
    void Start()
    {
        // If there's no save data, initialise our scene by creating a 'MyClass' instance, 
        // and assigning it to every 'myClass' variable of every 'MyScript' Component in the scene.
        if(!ES3.FileExists())
        {
            // Create a new MyClass object with a unique ID.
            var myClass = new MyClass(Guid.NewGuid().ToString());
            // Assign this MyClass to every 'myClass' variable of every 'MyScript' Component in the scene.
            foreach (var myScript in FindObjectsOfType<MyScript>())
                myScript.myClass = myClass;
        }
        // Otherwise, try to load the data.
        else
        {
            // Before loading anything else, load the Dictionary of MyClass objects.
            MyClass.classes = ES3.Load<Dictionary<string, MyClass>>("myClasses");
            // Now we can load our MyScripts.
            var myScripts = ES3.Load<MyScript[]>("myScripts");

            // Now we can check that they both reference the same MyClass.
            Debug.Assert(myScripts[0].myClass == myScripts[1].myClass);
        }

    }

    // Save everything when we quit.
    void OnApplicationQuit()
    {
        // Firstly, save the Dictionary of MyClass objects.
        ES3.Save("myClasses", MyClass.classes);
        // And now save our MyScript Components.
        // Note: you could save the GameObjects containing them instead and this would still work.
        ES3.Save("myScripts", FindObjectsOfType<MyScript>());
    }
}
