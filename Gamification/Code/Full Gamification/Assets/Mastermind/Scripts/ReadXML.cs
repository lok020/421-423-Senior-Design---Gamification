using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;

public static class ReadXML {
    public static List<string> parseXmlFile(string path, string nodepath)
    {
        string value_display = "";
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        //Debug.Log(xmlDoc.Value);

        XmlNodeList myNodeList = xmlDoc.SelectNodes(nodepath);
        List<string> nodes = new List<string>();
        int counter = 0;            // used to count to 8 to make sure the board will be 9x9

        foreach (XmlNode node in myNodeList)
        {
            XmlNode board = node.FirstChild;            // getting the whole board
            XmlNode value = board.FirstChild;           // getting the value from the board
            XmlNode display = value.NextSibling;        // getting the display from the board

            //Debug.Log(node.InnerXml);
            //Debug.Log(node["value"].InnerXml);
            //if (counter != 9)
            //{
            //Str_value += value.InnerXml + "\n";
            //Str_display += display.InnerXml + "\n";

            value_display += node["value"].InnerXml + "/";
            value_display += node["display"].InnerXml+"*";
            //Debug.Log(value_display);
            //123456789/FTFTFTFTFTFT*123456789/FTFTFTFTFTFT.....

            //uiText.text = Str_value + "\n";

            //counter++;
            //}
            counter++;
            if(counter >= 9)
            {
                counter = 0;
                nodes.Add(value_display);
                value_display = "";
            }
        }
        return nodes;
    }
}
