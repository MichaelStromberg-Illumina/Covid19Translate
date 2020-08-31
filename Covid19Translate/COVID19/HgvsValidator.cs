using System;
using System.Collections.Generic;
using System.IO;
using Covid19Translate.Nirvana;
using Covid19Translate.Utilities;

namespace Covid19Translate.COVID19
{
    public static class HgvsValidator
    {
        public static void Validate(IEnumerable<HgvsProteinEntry> entries,
            Dictionary<string, Transcript> idToTranscript, string bases)
        {
            foreach (HgvsProteinEntry entry in entries)
            {
                if (!Synonyms.ToProteinId.TryGetValue(entry.TranscriptName.ToLower(), out string proteinId))
                    throw new InvalidDataException($"ERROR: Unable to find the protein ID for {entry.TranscriptName}");

                if (!idToTranscript.TryGetValue(proteinId, out Transcript transcript))
                    throw new InvalidDataException(
                        $"ERROR: Unable to find the transcript for protein ID ({proteinId})");

                Validate(entry, transcript, proteinId, bases);
            }
        }

        private static void Validate(HgvsProteinEntry entry, Transcript transcript, string proteinId, string bases)
        {
            string hgvs = entry.GetHgvsString(proteinId);

            ValidateTranscriptAminoAcids(entry, transcript, hgvs);
            ValidateTranscriptBases(entry, transcript, hgvs);
            ValidateGenomicPosition(entry, transcript, bases, hgvs);
        }

        private static void ValidateGenomicPosition(HgvsProteinEntry entry, Transcript transcript, string bases,
            string hgvs)
        {
            int    genomicPosition = transcript.AminoAcidToGenomicPosition(entry.Position);
            string codon           = bases.Substring(genomicPosition - 1, 3);
            string aa              = AminoAcids.TranslateBases(codon);

            if (aa != entry.RefAminoAcid)
                Console.WriteLine($"WARNING: {hgvs} (genomic position): expected {entry.RefAminoAcid}, observed: {aa}");
        }

        private static void ValidateTranscriptBases(HgvsProteinEntry entry, Transcript transcript, string hgvs)
        {
            int    basePosition = Transcript.AminoAcidToCdsPosition(entry.Position);
            string codon        = transcript.GetCodon(basePosition);
            string aa           = AminoAcids.TranslateBases(codon);

            if (aa != entry.RefAminoAcid)
                Console.WriteLine($"WARNING: {hgvs} (transcript bases): expected {entry.RefAminoAcid}, observed: {aa}");
        }

        private static void ValidateTranscriptAminoAcids(HgvsProteinEntry entry, Transcript transcript, string hgvs)
        {
            string aminoAcid = transcript.GetAminoAcid(entry.Position);

            if (aminoAcid != entry.RefAminoAcid)
                Console.WriteLine(
                    $"WARNING: {hgvs} (transcript AA): expected {entry.RefAminoAcid}, observed: {aminoAcid}");
        }
    }
}