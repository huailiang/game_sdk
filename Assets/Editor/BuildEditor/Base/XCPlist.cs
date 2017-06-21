using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public partial class XCPlist : System.IDisposable
{

    private string filePath;
    List<string> contents = new List<string>();
    public XCPlist(string fPath)
    {

        // new version 
        this.plistPath = fPath;
        // -----------------------
        filePath = Path.Combine(fPath, "Info.plist");
        //filePath = plistPath;
        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogError(filePath + "路径下文件不存在");
            return;
        }

        FileInfo projectFileInfo = new FileInfo(filePath);
        StreamReader sr = projectFileInfo.OpenText();
        while (sr.Peek() >= 0)
        {
            contents.Add(sr.ReadLine());
        }
        sr.Close();
    }
    public void AddKey(string key)
    {
        if (contents.Count < 2)
            return;
        contents.Insert(contents.Count - 2, key);

    }

    public void ReplaceKey(string key, string replace)
    {
        for (int i = 0; i < contents.Count; i++)
        {
            if (contents[i].IndexOf(key) != -1)
            {
                contents[i] = contents[i].Replace(key, replace);
            }
        }
    }

    public void Save()
    {
        StreamWriter saveFile = File.CreateText(filePath);
        foreach (string line in contents)
            saveFile.WriteLine(line);
        saveFile.Close();
    }

    public void Dispose()
    {

    }

    string plistPath;
    bool plistModified;

    // URLTypes constant --- plist
    const string BundleUrlTypes = "CFBundleURLTypes";
    const string BundleTypeRole = "CFBundleTypeRole";
    const string BundleUrlName = "CFBundleURLName";
    const string BundleUrlSchemes = "CFBundleURLSchemes";

    // URLTypes constant --- projmods
    const string PlistUrlType = "urltype";
    const string PlistRole = "role";
    const string PlistEditor = "Editor";
    const string PlistName = "name";
    const string PlistSchemes = "schemes";
    /*
    public XCPlist(string plistPath)
    {
        this.plistPath = plistPath;
    }
    */
    public void Process(Hashtable plist)
    {
        if (!File.Exists(plistPath))
        {
            return;
        }
        Dictionary<string, object> dict = (Dictionary<string, object>)PlistCS.Plist.readPlist(plistPath);
        foreach (DictionaryEntry entry in plist)
        {
            Debug.LogError(entry.Key + "/" + entry.Value + "/" + dict);
            this.AddPlistItems((string)entry.Key, entry.Value, dict);
        }
        if (plistModified)
        {
            PlistCS.Plist.writeXml(dict, plistPath);
        }
    }

    public void AddPlistItems(string key, object value, Dictionary<string, object> dict)
    {
        Debug.Log("AddPlistItems: key=" + key);

        if (key.CompareTo(PlistUrlType) == 0)
        {
            processUrlTypes((ArrayList)value, dict);
        }
        else
        {
            Debug.LogWarning("unknown plist key : " + key);
        }
    }

    private void processUrlTypes(ArrayList urltypes, Dictionary<string, object> dict)
    {
        List<object> bundleUrlTypes;
        if (dict.ContainsKey(BundleUrlTypes))
        {
            bundleUrlTypes = (List<object>)dict[BundleUrlTypes];
        }
        else
        {
            bundleUrlTypes = new List<object>();
        }

        foreach (Hashtable table in urltypes)
        {
            string role = (string)table[PlistRole];
            if (string.IsNullOrEmpty(role))
            {
                role = PlistEditor;
            }
            string name = (string)table[PlistName];
            ArrayList shcemes = (ArrayList)table[PlistSchemes];

            // new schemes
            List<object> urlTypeSchemes = new List<object>();
            foreach (string s in shcemes)
            {
                urlTypeSchemes.Add(s);
            }

            Dictionary<string, object> urlTypeDict = this.findUrlTypeByName(bundleUrlTypes, name);
            if (urlTypeDict == null)
            {
                urlTypeDict = new Dictionary<string, object>();
                urlTypeDict[BundleTypeRole] = role;
                urlTypeDict[BundleUrlName] = name;
                urlTypeDict[BundleUrlSchemes] = urlTypeSchemes;
                bundleUrlTypes.Add(urlTypeDict);
            }
            else
            {
                urlTypeDict[BundleTypeRole] = role;
                urlTypeDict[BundleUrlSchemes] = urlTypeSchemes;
            }
            plistModified = true;
        }
        dict[BundleUrlTypes] = bundleUrlTypes;
    }

    private Dictionary<string, object> findUrlTypeByName(List<object> bundleUrlTypes, string name)
    {
        if ((bundleUrlTypes == null) || (bundleUrlTypes.Count == 0))
            return null;

        foreach (Dictionary<string, object> dict in bundleUrlTypes)
        {
            string _n = (string)dict[BundleUrlName];
            if (string.Compare(_n, name) == 0)
            {
                return dict;
            }
        }
        return null;
    }


}

