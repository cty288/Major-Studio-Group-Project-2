using System.Collections.Generic;
using System.Linq;

public class MyClass
{
    // A static Dictionary which holds all MyClass objects and their IDs.
    [ES3NonSerializable]
    public static Dictionary<string, MyClass> classes = new Dictionary<string, MyClass>();

    public int myInt = 123;
    public string myString = "myString";

    public MyClass(){}

    public MyClass(string uniqueID)
    {
        classes.Add(uniqueID, this);
    }

    // Gets the MyClass object for a given unique ID.
    public static MyClass Get(string uniqueID)
    {
        return classes[uniqueID];
    }

    // Gets the unique ID for a given class.
    public static string Get(MyClass myClass)
    {
        return classes.FirstOrDefault(x => x.Value == myClass).Key;
    }
}
