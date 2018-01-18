using UnityEngine;
using System.Collections;

public class Condition 
{
    string name;
    object value;

    public Condition(string _name, object _value)
    {
        name = _name;
        value = _value;
    }

   public string Name
    {
        get
        {
            return name;
        }
    }

  

    public object Value
    {
        get
        {
            return value;
        }
    }
}
