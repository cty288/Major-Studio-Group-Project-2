using UnityEngine;

public class MyScript : MonoBehaviour
{
    // Instead of storing the class itself, store it's unique ID.
    public string myClassUniqueID;

    //Gets the MyClass object with the ID specified by the myClassUniqueID variable.
    //Sets the myClassUniqueID to the unique ID of the specific MyClass object.
    public MyClass myClass
    {
        get{ return MyClass.Get(myClassUniqueID); }
        set { myClassUniqueID = MyClass.Get(value); }
    }
}
