using Covid19Translate.Nirvana;

namespace Covid19Translate.Utilities
{
    public static class TranscriptRegionExtensions
    {
        public static int Search(this TranscriptRegion[] regions, int position)
        {
            var begin = 0;
            int end   = regions.Length - 1;

            while (begin <= end)
            {
                int index  = begin + (end - begin >> 1);
                TranscriptRegion region = regions[index];

                if (position >= region.CdnaStart && position <= region.CdnaEnd) return index;
                if (region.CdnaEnd < position) begin       = index + 1;
                else if (position  < region.CdnaStart) end = index - 1;
            }

            return ~begin;
        }
    }
}