using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    GameObject EntityObject
    {
        get;
        set;
    }

    int ID
    {
        get;
        set;
    }
}
