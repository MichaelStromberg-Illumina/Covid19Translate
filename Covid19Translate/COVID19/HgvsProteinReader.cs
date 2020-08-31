using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Covid19Translate.Nirvana;
using Covid19Translate.Utilities;

namespace Covid19Translate.COVID19
{
    public static class HgvsProteinReader
    {
        public static HashSet<HgvsProteinEntry> GetEntries(string inputPath)
        {
            var entries = new HashSet<HgvsProteinEntry>();

            var regex = new Regex(@"([^_]+)_([a-zA-Z]+)(\d+)([a-zA-Z]+)", RegexOptions.Compiled);

            using FileStream stream = FileUtilities.GetReadStream(inputPath);
            using var        reader = new StreamReader(stream);

            while (true)
            {
                string line = reader.ReadLine();
                if (string.IsNullOrEmpty(line)) break;

                Match match = regex.Match(line);
                if (!match.Success)
                    throw new InvalidDataException($"ERROR: Unable to apply the HGVS regular expression to: [{line}]");

                string name         = match.Groups[1].Captures[0].Value;
                string refAminoAcid = match.Groups[2].Captures[0].Value;
                int    position     = int.Parse(match.Groups[3].Captures[0].Value);
                string altAminoAcid = match.Groups[4].Captures[0].Value;

                if (altAminoAcid.ToLower() == "stop") altAminoAcid = "*";
                
                ValidateAminoAcid("reference", refAminoAcid, line);
                ValidateAminoAcid("alternate", altAminoAcid, line);
                
                entries.Add(new HgvsProteinEntry(name, refAminoAcid, position, altAminoAcid));
            }

            return entries;
        }

        private static void ValidateAminoAcid(string description, string aminoAcid, string line)
        {
            if (!AminoAcids.ValidAminoAcids.Contains(aminoAcid))
                throw new InvalidDataException(
                    $"ERROR: Found an invalid {description} amino acid when parsing the HGVS protein file: {aminoAcid} in [{line}]");
        }
    }
}