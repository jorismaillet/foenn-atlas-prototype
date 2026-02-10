using Assets.Scripts.Foenn.ETL.Models;

namespace Assets.Scripts.Foenn.ETL.Extractors
{
    public abstract class Extractor
    {
        //TODO Should skip extraction if file already loaded (ex: table with file name of successful extraction and file md5)
        public abstract void Extract(Dataset dataset);
    }
}