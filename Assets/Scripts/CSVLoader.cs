using Assets.Resources.Weathers;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// ex1. CSVReader.getData(csvFile)
// ex2. CSVReader.getData(csvFile, ",")

public class CSVLoader
{
    private const int MAX_LINES = int.MaxValue;
    public static char[] STRING_SPLIT = { ';' };
    private const string DECIMAL_SPLIT = ".";

    public string[] header;
    public IEnumerable<string> remainingLines;

    public CSVLoader(string path)
    {
        var rawtext = Resources.Load<TextAsset>(path).text;
        if (rawtext == null)
        {
            throw new FileNotFoundException(path);
        }
        var reader = new StringReader(rawtext);
        header = reader.ReadLine().Split(STRING_SPLIT);
        remainingLines = RemainingLines(reader);
    }

    private IEnumerable<string> RemainingLines(StringReader reader)
    {
        var remainingLines = MAX_LINES;
        while (reader.Peek() != -1 && remainingLines > 0)
        {
            remainingLines--;
            yield return reader.ReadLine();
        }
        yield break;
    }
}