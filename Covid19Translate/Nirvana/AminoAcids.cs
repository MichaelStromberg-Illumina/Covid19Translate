using System.Collections.Generic;

namespace Covid19Translate.Nirvana
{
    public static class AminoAcids
    {
        private const string StopCodon = "*";

        private static readonly Dictionary<string, char> AminoAcidLookupTable;
        public static readonly  HashSet<string>          ValidAminoAcids;

        public static readonly Dictionary<char, string[]> AminoAcidToCodons; 

        static AminoAcids()
        {
            AminoAcidLookupTable = new Dictionary<string, char>
            {
                // 2nd base: T
                {"TTT", 'F'},
                {"TTC", 'F'},
                {"TTA", 'L'},
                {"TTG", 'L'},
                {"CTT", 'L'},
                {"CTC", 'L'},
                {"CTA", 'L'},
                {"CTG", 'L'},
                {"ATT", 'I'},
                {"ATC", 'I'},
                {"ATA", 'I'},
                {"ATG", 'M'},
                {"GTT", 'V'},
                {"GTC", 'V'},
                {"GTA", 'V'},
                {"GTG", 'V'},

                // 2nd base: C
                {"TCT", 'S'},
                {"TCC", 'S'},
                {"TCA", 'S'},
                {"TCG", 'S'},
                {"CCT", 'P'},
                {"CCC", 'P'},
                {"CCA", 'P'},
                {"CCG", 'P'},
                {"ACT", 'T'},
                {"ACC", 'T'},
                {"ACA", 'T'},
                {"ACG", 'T'},
                {"GCT", 'A'},
                {"GCC", 'A'},
                {"GCA", 'A'},
                {"GCG", 'A'},

                // 2nd base: A
                {"TAT", 'Y'},
                {"TAC", 'Y'},
                {"TAA", '*'},
                {"TAG", '*'},
                {"CAT", 'H'},
                {"CAC", 'H'},
                {"CAA", 'Q'},
                {"CAG", 'Q'},
                {"AAT", 'N'},
                {"AAC", 'N'},
                {"AAA", 'K'},
                {"AAG", 'K'},
                {"GAT", 'D'},
                {"GAC", 'D'},
                {"GAA", 'E'},
                {"GAG", 'E'},

                // 2nd base: G
                {"TGT", 'C'},
                {"TGC", 'C'},
                {"TGA", '*'},
                {"TGG", 'W'},
                {"CGT", 'R'},
                {"CGC", 'R'},
                {"CGA", 'R'},
                {"CGG", 'R'},
                {"AGT", 'S'},
                {"AGC", 'S'},
                {"AGA", 'R'},
                {"AGG", 'R'},
                {"GGT", 'G'},
                {"GGC", 'G'},
                {"GGA", 'G'},
                {"GGG", 'G'}
            };

            ValidAminoAcids = new HashSet<string>();
            const string validAminoAcids = "*ACDEFGHIKLMNPQRSTVWY";
            foreach (char aa in validAminoAcids) ValidAminoAcids.Add(aa.ToString());

            AminoAcidToCodons = new Dictionary<char, string[]>
            {
                ['*'] = new[] {"TAA", "TGA", "TAG"},
                ['A'] = new[] {"GCT", "GCC", "GCA", "GCG"},
                ['C'] = new[] {"TGT", "TGC"},
                ['D'] = new[] {"GAT", "GAC"},
                ['E'] = new[] {"GAA", "GAG"},
                ['F'] = new[] {"TTT", "TTC"},
                ['G'] = new[] {"GGT", "GGC", "GGA", "GGG"},
                ['H'] = new[] {"CAT", "CAC"},
                ['I'] = new[] {"ATT", "ATC", "ATA"},
                ['K'] = new[] {"AAA", "AAG"},
                ['L'] = new[] {"CTT", "CTC", "CTA", "CTG", "TTA", "TTG"},
                ['M'] = new[] {"ATG"},
                ['N'] = new[] {"AAT", "AAC"},
                ['P'] = new[] {"CCT", "CCC", "CCA", "CCG"},
                ['Q'] = new[] {"CAA", "CAG"},
                ['R'] = new[] {"CGT", "CGC", "CGA", "CGG", "AGA", "AGG"},
                ['S'] = new[] {"TCT", "TCC", "TCA", "TCG", "AGT", "AGC"},
                ['T'] = new[] {"ACT", "ACC", "ACA", "ACG"},
                ['V'] = new[] {"GTT", "GTC", "GTA", "GTG"},
                ['W'] = new[] {"TGG"},
                ['Y'] = new[] {"TAT", "TAC"}
            };
        }

        private static string AddUnknownAminoAcid(string aminoAcids) =>
            aminoAcids == StopCodon ? aminoAcids : aminoAcids + 'X';

        private static char ConvertTripletToAminoAcid(string triplet)
        {
            string upperTriplet = triplet.ToUpper();
            return AminoAcidLookupTable.TryGetValue(upperTriplet, out char aminoAcid) ? aminoAcid : 'X';
        }

        public static string TranslateBases(string bases)
        {
            // sanity check: handle the empty case
            if (bases == null) return null;

            int numAminoAcids = bases.Length / 3;

            // check if we have a non triplet case
            bool nonTriplet = numAminoAcids * 3 != bases.Length;

            // special case: single amino acid
            string aminoAcidString;
            if (numAminoAcids == 1)
            {
                aminoAcidString = ConvertTripletToAminoAcid(bases.Substring(0, 3)).ToString();
                return nonTriplet ? AddUnknownAminoAcid(aminoAcidString) : aminoAcidString;
            }

            // multiple amino acid case
            var aminoAcids = new char[numAminoAcids];
            for (var i = 0; i < numAminoAcids; i++)
            {
                aminoAcids[i] = ConvertTripletToAminoAcid(bases.Substring(i * 3, 3));
            }

            aminoAcidString = new string(aminoAcids);
            return nonTriplet ? AddUnknownAminoAcid(aminoAcidString) : aminoAcidString;
        }
    }
}