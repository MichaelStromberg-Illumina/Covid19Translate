namespace Covid19Translate.Nirvana
{
    public readonly struct Interval
    {
        public readonly int Start;
        public readonly int End;

        public Interval(int start, int end)
        {
            Start = start;
            End   = end;
        }
    }
}