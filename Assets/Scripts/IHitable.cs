using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represent object that can be dammaged
public interface IHitable //Unity inspector doesn't handle well interface...    
{
    bool Hit();
}
