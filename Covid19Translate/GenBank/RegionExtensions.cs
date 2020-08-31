using System.Text;
using Covid19Translate.Nirvana;

namespace Covid19Translate.GenBank
{
    public static class RegionExtensions
    {
        public static string GetBases(this Interval[] regions, string sequence)
        {
            var sb = new StringBuilder();

            foreach (Interval region in regions)
            {
                sb.Append(sequence.Substring(region.Start - 1, region.End - region.Start + 1));
            }

            return sb.ToString();
        }
    }
}