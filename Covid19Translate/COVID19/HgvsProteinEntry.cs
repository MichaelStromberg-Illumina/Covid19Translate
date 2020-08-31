using System;

namespace Covid19Translate.COVID19
{
    public sealed class HgvsProteinEntry : IEquatable<HgvsProteinEntry>
    {
        public readonly string TranscriptName;
        public readonly string RefAminoAcid;
        public readonly int    Position;
        public readonly string AltAminoAcid;

        public HgvsProteinEntry(string transcriptName, string refAminoAcid, int position, string altAminoAcid)
        {
            TranscriptName = transcriptName;
            RefAminoAcid   = refAminoAcid;
            Position       = position;
            AltAminoAcid   = altAminoAcid;
        }

        public bool Equals(HgvsProteinEntry other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TranscriptName == other.TranscriptName && RefAminoAcid == other.RefAminoAcid &&
                   Position       == other.Position       && AltAminoAcid == other.AltAminoAcid;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = TranscriptName.GetHashCode();
                hashCode = (hashCode * 397) ^ RefAminoAcid.GetHashCode();
                hashCode = (hashCode * 397) ^ Position;
                hashCode = (hashCode * 397) ^ AltAminoAcid.GetHashCode();
                return hashCode;
            }
        }

        public string GetHgvsString(string proteinId) =>
            $"{proteinId}({TranscriptName}):p.{RefAminoAcid}{Position}{AltAminoAcid}";
    }
}