using Assets.Scripts.Foenn.ETL.Models;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.ETL.Extractors
{
    public abstract class Extractor
    {
        public abstract List<Datafield> ExtractHeaders();
        public abstract IEnumerable<string[]> ExtractContent(int headersCount);
    }
}