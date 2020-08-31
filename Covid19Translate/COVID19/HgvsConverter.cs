using System.Collections.Generic;
using System.IO;
using System.Linq;
using Covid19Translate.Nirvana;
using Covid19Translate.Utilities;

namespace Covid19Translate.COVID19
{
    public static class HgvsConverter
    {
        public static HashSet<SNV> Convert(IEnumerable<HgvsProteinEntry> entries,
            Dictionary<string, Transcript> idToTranscript, string bases)
        {
            var snvs = new HashSet<SNV>();
            
            foreach (HgvsProteinEntry entry in entries)
            {
                if (!Synonyms.ToProteinId.TryGetValue(entry.TranscriptName.ToLower(), out string proteinId))
                    throw new InvalidDataException($"ERROR: Unable to find the protein ID for {entry.TranscriptName}");

                if (!idToTranscript.TryGetValue(proteinId, out Transcript transcript))
                    throw new InvalidDataException(
                        $"ERROR: Unable to find the transcript for protein ID ({proteinId})");

                int    genomicPosition = transcript.AminoAcidToGenomicPosition(entry.Position);
                string refCodon        = bases.Substring(genomicPosition - 1, 3);
                
                (char[] alleles1, char[] alleles2, char[] alleles3) = FindCodonDifferences(refCodon, entry.RefAminoAcid[0], entry.AltAminoAcid[0]);

                var numPositions = 0;
                if (alleles1.Length > 0) numPositions++;
                if (alleles2.Length > 0) numPositions++;
                if (alleles3.Length > 0) numPositions++;

                bool   exactPosition = numPositions == 1;
                string hgvs          = entry.GetHgvsString(proteinId);

                
                snvs.AddSnvs(genomicPosition,     refCodon[0], alleles1, exactPosition, hgvs);
                snvs.AddSnvs(genomicPosition + 1, refCodon[1], alleles2, exactPosition, hgvs);
                snvs.AddSnvs(genomicPosition + 2, refCodon[2], alleles3, exactPosition, hgvs);
            }
            
            return snvs;
        }

        private static void AddSnvs(this HashSet<SNV> snvs, in int position, char refAllele, char[] altAlleles,
            bool exactPosition, string hgvs)
        {
            if (altAlleles.Length == 0) return;
            
            foreach (char altAllele in altAlleles)
            {
                var snv = new SNV(position, refAllele.ToString(), altAllele.ToString(), hgvs, exactPosition);
                snvs.Add(snv);
            }
        }

        private static (char[] Alleles1, char[] Alleles2, char[] Alleles3) FindCodonDifferences(string refCodon,
            char refAminoAcid, char altAminoAcid)
        {
            string[] codons = AminoAcids.AminoAcidToCodons[altAminoAcid];

            var allelesByPosition                            = new HashSet<char>[3];
            for (var i = 0; i < 3; i++) allelesByPosition[i] = new HashSet<char>();

            foreach (string codon in codons)
            {
                for (var i = 0; i < 3; i++)
                {
                    if (codon[i] != refCodon[i])
                    {
                        allelesByPosition[i].TryAddAllele(refCodon, refAminoAcid, codon[i], i);
                    }
                }
            }

            return (allelesByPosition[0].ToArray(), allelesByPosition[1].ToArray(), allelesByPosition[2].ToArray());
        }

        private static void TryAddAllele(this HashSet<char> alleles, string refCodon, char refAminoAcid, char altAllele,
            in int offset)
        {
            char[] altCodonCharArray = refCodon.ToCharArray();
            altCodonCharArray[offset] = altAllele;
            var  altCodon = new string(altCodonCharArray);
            char aa       = AminoAcids.TranslateBases(altCodon)[0];
            if (aa == refAminoAcid) return;

            alleles.Add(altAllele);
        }
    }
}