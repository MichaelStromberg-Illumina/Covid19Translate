using System.Collections.Generic;
using System.Linq;
using Covid19Translate.Nirvana;

namespace Covid19Translate.GenBank
{
    public sealed class MaturePeptide : IFeature
    {
        public int        Start      { get; }
        public int        End        { get; }
        public string     GeneSymbol { get; }
        public string     LocusTag   { get; }
        public string     Product    { get; }
        public string     ProteinId  { get; }
        public Interval[] Regions    { get; }
        public string     Note       { get; }

        private CodingSequence _cds;

        public MaturePeptide(Interval interval, Interval[] regions, string geneSymbol, string locusTag, string product,
            string note, string proteinId, CodingSequence cds)
        {
            Start      = interval.Start;
            End        = interval.End;
            Regions    = regions;
            GeneSymbol = geneSymbol;
            LocusTag   = locusTag;
            Product    = product;
            Note       = note;
            ProteinId  = proteinId;
            _cds       = cds;
        }

        public override string ToString()
        {
            var intervals = new List<string>(Regions.Length);
            intervals.AddRange(Regions.Select(region => $"{region.Start}-{region.End}"));
            string regions = string.Join(", ", intervals);

            return
                $"mat_peptide: {Start}-{End}, regions: {regions}, symbol: {GeneSymbol}, locus tag: {LocusTag}, note: {Note}, product: {Product}, protein ID: {ProteinId}";
        }

        public Transcript ToTranscript(string bases)
        {
            string codingSequence    = Regions.GetBases(bases);
            string aminoAcidSequence = AminoAcids.TranslateBases(codingSequence);

            TranscriptRegion[] transcriptRegions = TranscriptRegion.Create(Regions);

            return new Transcript(ProteinId, Start, End, BioType.MaturePeptide, codingSequence, aminoAcidSequence,
                transcriptRegions);
        }
    }
}