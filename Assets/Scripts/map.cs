using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

public class map 
{
    [XmlAttribute("version")]
    public string version;
    [XmlAttribute("orientation")]
    public string orientation;
    [XmlAttribute("width")]
    public string width;
    [XmlAttribute("height")]
    public string height;
    [XmlAttribute("tilewidth")]
    public string tilewidth;
    [XmlAttribute("tileheight")]
    public string tileheight;


}
