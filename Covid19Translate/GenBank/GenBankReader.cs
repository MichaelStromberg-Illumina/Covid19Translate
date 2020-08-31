using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Covid19Translate.Nirvana;

namespace Covid19Translate.GenBank
{
    public sealed class GenBankReader : IDisposable
    {
        private readonly ReaderData _readerData;
        private readonly GenBankFeatureParser _parser;

        private const int SequenceTagLength = 10;

        private const string FeaturesTag = "FEATURES";

        private const string CdsFeatureTag           = "CDS";
        private const string FivePrimeFeatureTag     = "5'UTR";
        private const string GeneFeatureTag          = "gene";
        private const string MaturePeptideFeatureTag = "mat_peptide";
        private const string SourceFeatureTag        = "source";
        private const string StemLoopFeatureTag      = "stem_loop";
        private const string ThreePrimeFeatureTag    = "3'UTR";

        private CodingSequence _currentCds;

        public GenBankReader(Stream stream, bool leaveOpen = false)
        {
            var reader = new StreamReader(stream, Encoding.Default, true, 1024, leaveOpen);
            _readerData = new ReaderData(reader);
            _parser     = new GenBankFeatureParser(_readerData);
        }

        public (Dictionary<string, Transcript> IdToTranscript, string Bases) ParseEntry()
        {
            FindFeatures();

            var features = new List<IFeature>();
            while (!ParsingUtilities.FoundOrigin(_readerData.CurrentLine))
            {
                IFeature feature = GetNextFeature();
                if (feature == null) continue;
                features.Add(feature);
            }

            string bases = ParseSequence();

            var idToTranscript = new Dictionary<string, Transcript>();

            foreach (IFeature feature in features)
            {
                var transcript = feature.ToTranscript(bases);
                
                if (idToTranscript.ContainsKey(transcript.Id))
                    Console.WriteLine($"WARNING: Transcript dictionary already contains {transcript.Id}");
                
                idToTranscript[transcript.Id] = transcript;
            }

            return (idToTranscript, bases);
        }

        private string ParseSequence()
        {
            var sb = new StringBuilder();
            
            while (true)
            {
                _readerData.GetNextLine();
                if (_readerData.CurrentLine == null || ParsingUtilities.FoundEnd(_readerData.CurrentLine)) break;

                string line = _readerData.CurrentLine.Substring(SequenceTagLength).Replace(" ", "").ToUpper();
                sb.Append(line);
            }

            return sb.ToString();
        }

        private void FindFeatures()
        {
            while (true)
            {
                _readerData.GetNextLine();
                if (_readerData.CurrentLine == null) break;
                if (!_readerData.CurrentLine.StartsWith(FeaturesTag)) continue;
                break;
            }
            
            _readerData.GetNextLine();
        }

        private IFeature GetNextFeature()
        {
            string label = ParsingUtilities.GetLabel(_readerData.CurrentLine);
            if (string.IsNullOrEmpty(label))
                throw new InvalidDataException($"Found an empty state label: {_readerData.CurrentLine}");

            return label switch
            {
                SourceFeatureTag        => SkipFeature(),
                FivePrimeFeatureTag     => SkipFeature(),
                GeneFeatureTag          => SkipFeature(),
                CdsFeatureTag           => GetCds(),
                MaturePeptideFeatureTag => GetMaturePeptide(),
                StemLoopFeatureTag      => SkipFeature(),
                ThreePrimeFeatureTag    => SkipFeature(),
                _                       => throw new NotSupportedException($"Found an unsupported state: {label}")
            };
        }

        private IFeature SkipFeature()
        {
            _parser.GetFeature();
            return null;
        }

        private IFeature GetCds()
        {
            GenBankData data = _parser.GetFeature();
            var cds = new CodingSequence(data.Interval, data.GeneSymbol, data.LocusTag, data.GeneId, data.Regions,
                data.Note, data.CodonStart, data.Product, data.ProteinId, data.Translation + '*');
            _currentCds = cds;
            return cds;
        }

        private IFeature GetMaturePeptide()
        {
            GenBankData data = _parser.GetFeature();
            return new MaturePeptide(data.Interval, data.Regions, data.GeneSymbol, data.LocusTag, data.Product,
                data.Note, data.ProteinId, _currentCds);
        }

        public void Dispose() => _readerData.Dispose();
    }
}
