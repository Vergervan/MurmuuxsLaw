using System.Collections.Generic;
using System.IO;

public class Reader
{
    private string _filename;
    public string Filename
    {
        get => _filename;
        set
        {
            _filename = value;
        }
    }
    private Dictionary<string, UnitSpeech> lines;

    public bool ReadFile() => ReadFile(Filename);
    public bool ReadFile(string fname)
    {
        if (!File.Exists(fname))
        {
            return false;
        }
        lines.Clear();
        byte[] buffer = { };
        using (FileStream ifs = new FileStream(fname, FileMode.Open, FileAccess.Read))
        {
            int numBytesToRead = (int)(ifs.Length), numBytesRead = 0;
            buffer = new byte[numBytesToRead];
            while (numBytesToRead > 0)
            {
                int n = ifs.Read(buffer, numBytesRead, numBytesToRead);
                numBytesRead += n;
                numBytesToRead -= n;
            }
        }
        string[] fileContents = System.Text.Encoding.UTF8.GetString(buffer).Split('\n');
        foreach (string line in fileContents)
        {
            if (line[0] == '#' || string.IsNullOrWhiteSpace(line)) continue;
            string stringName = string.Empty, stringValue = string.Empty;
            int indexOfEq = line.IndexOf("=");
            if (indexOfEq == -1) continue;
            stringName = line.Substring(0, indexOfEq).Trim();
            stringValue = line.Substring(indexOfEq + 1).TrimStart(' ');
            lines.Add(stringName, new UnitSpeech(stringValue.Split(new string[] { "<br>" }, System.StringSplitOptions.RemoveEmptyEntries)));
        }
        return true;
    }
    public Reader()
    {
        lines = new Dictionary<string, UnitSpeech>();
    }
    public Reader(string fname)
    {
        lines = new Dictionary<string, UnitSpeech>();
        Filename = fname;
        ReadFile(Filename);
    }

    public UnitSpeech GetUnitSpeech(string name)
    {
        if (lines.ContainsKey(name))
            return lines[name].Clone();
        return null;
    }
}