using System;
using Covid19Translate.Nirvana;

namespace Covid19Translate.GenBank
{
    public sealed class GenBankFeatureParser
    {
        private readonly ReaderData _readerData;
        
        private const string GeneIdKey = "GeneID";
        private const string TaxonKey  = "taxon";

        private const  string CodonStartKey        = "codon_start";
        private const  string CollectionDateKey    = "collection_date";
        private const  string CountryKey           = "country";
        private const  string DbXRefKey            = "db_xref";
        private const  string FunctionKey          = "function";
        private const  string GeneKey              = "gene";
        private const  string GeneSynonymKey       = "gene_synonym";
        private const  string HostKey              = "host";
        private const  string InferenceKey         = "inference";
        private const  string IsolateKey           = "isolate";
        private const  string LocusTagKey          = "locus_tag";
        private const  string MoleculeTypeKey      = "mol_type";
        private const  string NoteKey              = "note";
        private const  string OrganismKey          = "organism";
        private const  string ProductKey           = "product";
        private const  string ProteinIdKey         = "protein_id";
        private const  string RibosomalSlippageKey = "ribosomal_slippage";
        internal const string TranslationKey       = "translation";

        public GenBankFeatureParser(ReaderData readerData) => _readerData = readerData;

        public GenBankData GetFeature()
        {
            var data = new GenBankData { Regions = ParsingUtilities.GetRegions(_readerData.CurrentLine) };
            data.Interval = new Interval(data.Regions[0].Start, data.Regions[^1].End);
            
            while (true)
            {
                _readerData.GetNextLine();
                if (_readerData.CurrentLine == null                       ||
                    ParsingUtilities.FoundOrigin(_readerData.CurrentLine) ||
                    ParsingUtilities.HasLabel(_readerData.CurrentLine)) break;
                
                (string key, string value) = ParsingUtilities.GetKeyValuePair(_readerData);

                switch (key)
                {
                    case GeneSynonymKey:
                    case InferenceKey:
                    case RibosomalSlippageKey:
                        // skip
                        break;
                    case CodonStartKey:
                        data.CodonStart = int.Parse(value);
                        break;
                    case CollectionDateKey:
                        data.CollectionDate = ParsingUtilities.GetDate(value);
                        break;
                    case CountryKey:
                        data.Country = value;
                        break;
                    case FunctionKey:
                        data.Function = value;
                        break;
                    case GeneKey:
                        data.GeneSymbol = value;
                        break;
                    case HostKey:
                        data.Host = value;
                        break;
                    case IsolateKey:
                        data.Isolate = value;
                        break;
                    case LocusTagKey:
                        data.LocusTag = value;
                        break;
                    case MoleculeTypeKey:
                        data.MoleculeType = value;
                        break;
                    case NoteKey:
                        data.Note = value;
                        break;
                    case OrganismKey:
                        data.Organism = value;
                        break;
                    case ProductKey:
                        data.Product = value;
                        break;
                    case ProteinIdKey:
                        data.ProteinId = value;
                        break;
                    case DbXRefKey:
                        if (value.StartsWith(GeneIdKey))
                        {
                            data.GeneId = ParsingUtilities.GetValueFromColon(value, GeneIdKey);
                        }

                        if (value.StartsWith(TaxonKey))
                        {
                            data.TaxonomyId = ParsingUtilities.GetValueFromColon(value, TaxonKey);
                        }
                        break;
                    case TranslationKey:
                        data.Translation = value;
                        break;
                    default:
                        throw new NotSupportedException($"Found an unsupported key in the CDS feature: {_readerData.CurrentLine}");
                }
            }

            return data;
        }
    }
}