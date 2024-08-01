using Assets.Resources.Weathers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public CSVLoader(string path, int postID = 0)
    {
        var rawtext = Resources.Load<TextAsset>(path).text;
        if (rawtext == null)
        {
            throw new FileNotFoundException(path);
        }
        var reader = new StringReader(rawtext);
        header = reader.ReadLine().Split(STRING_SPLIT);

        if(postID > 0)
        {
            remainingLines = FilteredRemainingLines(reader, postID.ToString());
        }
        else
        {
            remainingLines = AllRemainingLines(reader);
        }
    }

    private IEnumerable<string> AllRemainingLines(StringReader reader)
    {
        var remainingLines = MAX_LINES;
        while (reader.Peek() != -1 && remainingLines > 0)
        {
            remainingLines--;
            yield return reader.ReadLine();
        }
        yield break;
    }

    private IEnumerable<string> FilteredRemainingLines(StringReader reader, string postID)
    {
        string line;
        bool startAdding = false;

        while ((line = reader.ReadLine()) != null)
        {
            var columns = line.Split(STRING_SPLIT);
            var currentID = columns[0]; // Assumant que l'ID est dans la première colonne

            if (currentID == postID)
            {
                startAdding = true;

            }
            else if (startAdding && currentID != postID)
            {
                // Une fois que nous avons trouvé les lignes qui commencent par targetID, nous pouvons arrêter dès que l'ID change
                yield break;
            }

            if (startAdding)
            {
                yield return line;
            }
        }
    }
}