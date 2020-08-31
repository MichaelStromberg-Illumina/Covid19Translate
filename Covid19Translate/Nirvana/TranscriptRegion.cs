namespace Covid19Translate.Nirvana
{
    public sealed class TranscriptRegion
    {
        public readonly int Start;
        public readonly int End;
        public readonly int CdnaStart;
        public readonly int CdnaEnd;

        public TranscriptRegion(int start, int end, int cdnaStart, int cdnaEnd)
        {
            Start     = start;
            End       = end;
            CdnaStart = cdnaStart;
            CdnaEnd   = cdnaEnd;
        }

        public static TranscriptRegion[] Create(Interval[] regions)
        {
            var transcriptRegions = new TranscriptRegion[regions.Length];
            var cdnaStart         = 1;

            for (var i = 0; i < regions.Length; i++)
            {
                Interval region  = regions[i];
                int      offset  = region.End - region.Start;
                int      cdnaEnd = cdnaStart  + offset;

                transcriptRegions[i] = new TranscriptRegion(region.Start, region.End, cdnaStart, cdnaEnd);
                cdnaStart            = cdnaEnd + 1;
            }

            return transcriptRegions;
        }
    }
}