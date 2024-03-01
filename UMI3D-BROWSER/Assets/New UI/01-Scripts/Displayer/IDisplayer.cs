using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDisplayer
{ 
    public object GetValue(bool trim);
    public void SetTitle(string title);
    public void SetPlaceHolder(string placeHolder);
}
