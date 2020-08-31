using System.Collections.Generic;
using System.IO;

namespace Covid19Translate.Utilities
{
    public static class Synonyms
    {
        public static readonly Dictionary<string, string> ToProteinId = new Dictionary<string, string>();

        public static void Load(string inputPath)
        {
            using FileStream stream = FileUtilities.GetReadStream(inputPath);
            using var        reader = new StreamReader(stream);

            while (true)
            {
                string line = reader.ReadLine();
                if (string.IsNullOrEmpty(line)) break;

                string[] cols = line.Split('\t');
                if (cols.Length != 2)
                    throw new InvalidDataException(
                        $"ERROR: Expected two columns in the synonyms file, but found {cols.Length}");

                string synonym   = cols[0].ToLower();
                string proteinId = cols[1].ToUpper();

                if (ToProteinId.ContainsKey(synonym))
                    throw new InvalidDataException(
                        $"ERROR: More than one occurrence of synonym ({synonym}) was found.");
                ToProteinId[synonym] = proteinId;
            }
        }
    }
}