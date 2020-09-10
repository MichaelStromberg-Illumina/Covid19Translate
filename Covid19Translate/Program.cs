using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Covid19Translate.COVID19;
using Covid19Translate.GenBank;
using Covid19Translate.Nirvana;
using Covid19Translate.Utilities;

namespace Covid19Translate
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                string programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                Console.WriteLine($"{programName} <input GenBank path> <input synonyms path> <input protein HGVS list> <output VCF directory>");
                Environment.Exit(1);
            }

            string inputGenBankPath     = args[0];
            string inputSynonymsPath    = args[1];
            string inputProteinHgvsPath = args[2];
            string outputVcfDirectory   = args[3];

            Console.Write("- parsing GenBank file... ");
            (Dictionary<string, Transcript> idToTranscript, string bases) = Load(inputGenBankPath);
            Console.WriteLine($"{idToTranscript.Count} coding sequences loaded.");

            Console.Write("- parsing synonyms file... ");
            Synonyms.Load(inputSynonymsPath);
            Console.WriteLine($"{Synonyms.ToProteinId.Count} synonyms loaded.");
            
            Console.Write("- parsing pseudo-HGVS p. nomenclature file... ");
            Dictionary<string, HgvsProteinEntry[]> hgvsEntriesBySample = HgvsProteinReader.GetEntries(inputProteinHgvsPath);
            Console.WriteLine($"{hgvsEntriesBySample.Count} samples loaded.");

            HashSet<HgvsProteinEntry> hgvsEntries = GetAggregateEntries(hgvsEntriesBySample);
            
            DisplayHgvsEntries(hgvsEntries);

            Console.Write("- validating HGVS entries against gene models... ");
            HgvsValidator.Validate(hgvsEntries, idToTranscript, bases);
            Console.WriteLine("finished.");

            foreach ((string sampleId, HgvsProteinEntry[] sampleHgvsProteinEntries) in hgvsEntriesBySample)
            {
                string vcfPath = Path.Combine(outputVcfDirectory, sampleId + ".vcf");
                
                Console.Write($"- writing variants for {sampleId}... ");
                HashSet<SNV> snvs = HgvsConverter.Convert(sampleHgvsProteinEntries, idToTranscript, bases);
                (int numExactPositions, int numAmbiguousPositions) = CountExactPositions(snvs);
                WriteVcf(vcfPath, sampleId, snvs);
                Console.WriteLine($"{snvs.Count} SNVs written ({numExactPositions} exact, {numAmbiguousPositions} ambiguous).");
            }
        }

        private static HashSet<HgvsProteinEntry> GetAggregateEntries(Dictionary<string,HgvsProteinEntry[]> hgvsEntriesBySample)
        {
            var hgvsEntries = new HashSet<HgvsProteinEntry>();
            
            foreach (HgvsProteinEntry[] entries in hgvsEntriesBySample.Values)
            {
                foreach (HgvsProteinEntry entry in entries) hgvsEntries.Add(entry);
            }

            return hgvsEntries;
        }

        private static (int NumExactPositions, int NumAmbiguousPositions) CountExactPositions(HashSet<SNV> snvs)
        {
            var numExactPositions     = 0;
            var numAmbiguousPositions = 0;

            foreach (SNV snv in snvs)
            {
                if (snv.ExactMatch) numExactPositions++;
                else numAmbiguousPositions++;
            }

            return (numExactPositions, numAmbiguousPositions);
        }

        private static void WriteVcf(string outputVcfPath, string sampleId, HashSet<SNV> snvs)
        {
            using FileStream stream = FileUtilities.GetCreateStream(outputVcfPath);
            using var        writer = new VcfWriter(stream, sampleId);
            
            foreach (SNV snv in snvs.OrderBy(x => x.Position)) writer.Write(snv);
        }

        private static void DisplayHgvsEntries(HashSet<HgvsProteinEntry> hgvsEntries)
        {
            Console.WriteLine();
            ConsoleUtilities.Highlight("HGVS Protein Variants:");
            Console.WriteLine("===============================");

            foreach (HgvsProteinEntry entry in hgvsEntries.OrderBy(x => x.TranscriptName).ThenBy(x => x.Position))
            {
                string proteinId = Synonyms.ToProteinId[entry.TranscriptName.ToLower()];
                Console.WriteLine($"- {entry.GetHgvsString(proteinId)}");
            }
            
            Console.WriteLine();
        }

        private static (Dictionary<string, Transcript> IdToTranscript, string Bases) Load(string inputPath)
        {
            Dictionary<string, Transcript> idToTranscript;
            string                         bases;

            using (var stream = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new GenBankReader(stream))
            {
                (idToTranscript, bases) = reader.ParseEntry();
            }

            return (idToTranscript, bases);
        }
    }
}