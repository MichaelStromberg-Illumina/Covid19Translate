using Covid19Translate.Nirvana;

namespace Covid19Translate.GenBank
{
    public interface IFeature
    {
        int        Start      { get; }
        int        End        { get; }
        string     GeneSymbol { get; }
        string     LocusTag   { get; }
        string     Product    { get; }
        string     ProteinId  { get; }
        Interval[] Regions    { get; }
        string     Note       { get; }
        Transcript ToTranscript(string bases);
    }
}