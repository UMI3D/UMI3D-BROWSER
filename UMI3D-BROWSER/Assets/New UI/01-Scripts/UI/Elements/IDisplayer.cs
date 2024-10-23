using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An interface to connect any displayer to the dynamic server container
/// </summary>
/// If you want to make a new displayer you need to implement this interface. 
/// This is intended to be used with the UMI3D form system
public interface IDisplayer
{ 
    public object GetValue(bool trim);
    public void SetTitle(string title);
    public void SetPlaceHolder(List<string> placeHolder);
    public void SetColor(Color color);
    public void SetResource(object resource);
}
