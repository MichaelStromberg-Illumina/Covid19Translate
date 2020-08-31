using System;

namespace Covid19Translate.COVID19
{
    public sealed class SNV : IEquatable<SNV>
    {
        public readonly  int    Position;
        private readonly string _refAllele;
        private readonly string _altAllele;
        private readonly string _hgvsProtein;
        public readonly  bool   ExactMatch;

        public SNV(int position, string refAllele, string altAllele, string hgvsProtein, bool exactMatch)
        {
            Position     = position;
            _refAllele   = refAllele;
            _altAllele   = altAllele;
            _hgvsProtein = hgvsProtein;
            ExactMatch   = exactMatch;
        }

        public bool Equals(SNV other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Position    == other.Position && _refAllele == other._refAllele && Position == other.Position &&
                   _altAllele == other._altAllele;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ _refAllele.GetHashCode();
                hashCode = (hashCode * 397) ^ _altAllele.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            string infoField = ExactMatch ? "EXACT_POSITION" : "AMBIGUOUS_POSITION";
            return $"NC_045512.2\t{Position}\t{_hgvsProtein}\t{_refAllele}\t{_altAllele}\t.\tPASS\t{infoField}\tGT\t0/1";
        }
    }
}