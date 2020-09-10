using System;
using System.IO;

namespace Covid19Translate.COVID19
{
    public class VcfWriter : IDisposable
    {
        private readonly string       _sampleId;
        private readonly StreamWriter _writer;

        public VcfWriter(Stream stream, string sampleId)
        {
            _sampleId = sampleId;
            _writer   = new StreamWriter(stream) {NewLine = "\n"};
            WriteHeader();
        }

        private void WriteHeader()
        {
            _writer.WriteLine("##fileformat=VCFv4.2");
            _writer.WriteLine("##FILTER=<ID=PASS,Description=\"All filters passed\">");
            _writer.WriteLine("##contig=<ID=NC_045512.2,length=29903>");
            _writer.WriteLine($"#CHROM\tPOS\tID\tREF\tALT\tQUAL\tFILTER\tINFO\tFORMAT\t{_sampleId}");
        }

        public void Write(SNV snv) => _writer.WriteLine(snv);
        public void Dispose()      => _writer.Dispose();
    }
}