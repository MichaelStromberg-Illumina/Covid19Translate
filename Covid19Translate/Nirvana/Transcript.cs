using System.IO;
using Covid19Translate.Utilities;

namespace Covid19Translate.Nirvana
{
    public sealed class Transcript
    {
        public readonly  string  Id;
        private readonly int     _start;
        private readonly int     _end;
        private readonly BioType _bioType;

        private readonly string _codingSequence;
        private readonly string _aminoAcidSequence;

        private readonly TranscriptRegion[] _transcriptRegions;

        public Transcript(string id, int start, int end, BioType bioType, string codingSequence,
            string aminoAcidSequence, TranscriptRegion[] transcriptRegions)
        {
            Id                 = id;
            _start              = start;
            _end                = end;
            _bioType            = bioType;
            _codingSequence    = codingSequence;
            _aminoAcidSequence = aminoAcidSequence;
            _transcriptRegions = transcriptRegions;
        }

        public override string ToString() =>
            $"transcript: {_start:N0} - {_end:N0}, ID: {Id}, biotype: {_bioType}, # of bases: {_codingSequence.Length}, # of AA: {_aminoAcidSequence.Length}";

        public string GetAminoAcid(in int aaPosition) => _aminoAcidSequence[aaPosition - 1].ToString();

        public string GetCodon(in int position) => _codingSequence.Substring(position - 1, 3);

        public static int AminoAcidToCdsPosition(in int aaPosition) => (aaPosition - 1) * 3 + 1;

        public int AminoAcidToGenomicPosition(in int aaPosition)
        {
            int cdsPosition = AminoAcidToCdsPosition(aaPosition);
            int index       = _transcriptRegions.Search(cdsPosition);

            if (index < 0)
                throw new InvalidDataException(
                    $"ERROR: Unable to identify the proper transcript region for AA position {aaPosition}");

            TranscriptRegion region = _transcriptRegions[index];

            int offset          = cdsPosition  - region.CdnaStart;
            int genomicPosition = region.Start + offset;

            return genomicPosition;
        }
    }
}