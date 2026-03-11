namespace Assets.Scripts.Foenn.ETL
{
    using Assets.Scripts.Foenn.OLAP.Schema;
    using Mono.Data.Sqlite;
    using System.Collections.Generic;
    using System.Linq;

    public class FactTableLoader : SqliteTableLoader
    {
        public IFact Fact => (IFact)Table;

        private Dictionary<Reference, DimensionCache> _dimensionCaches;
        private Dictionary<Reference, int> _refParamIndex;
        private Dictionary<Reference, int> _refSourceCsvIndex;

        public FactTableLoader(IFact fact) : base(fact) { }

        public void StartStaging(SqliteConnection connection, SqliteTransaction transaction, string[] csvFieldNames, Dictionary<IDimension, DimensionCache> caches)
        {
            base.StartStaging(connection, transaction, csvFieldNames);

            _dimensionCaches = new Dictionary<Reference, DimensionCache>();
            _refParamIndex = new Dictionary<Reference, int>();
            _refSourceCsvIndex = new Dictionary<Reference, int>();

            var columnsWithoutPk = Fact.Columns.Where(c => !(c is PrimaryKey pk && pk.autoIncrement)).ToList();

            foreach (var reference in Fact.References)
            {
                if (caches.TryGetValue(reference.referencedDimension, out var cache))
                {
                    _dimensionCaches[reference] = cache;
                    _refSourceCsvIndex[reference] = FindCsvIndex(reference.sourceCsvColumn, csvFieldNames);

                    int colIndex = columnsWithoutPk.FindIndex(c => c.name == reference.name);
                    if (colIndex >= 0)
                    {
                        _refParamIndex[reference] = colIndex;
                        _csvToColumnMap[colIndex] = -1;
                    }
                }
            }
        }

        public override void StageLine(string[] csvLine)
        {
            var columnsWithoutPk = Fact.Columns.Where(c => !(c is PrimaryKey pk && pk.autoIncrement)).ToList();

            for (int i = 0; i < columnsWithoutPk.Count; i++)
            {
                var reference = Fact.References.FirstOrDefault(r => r.name == columnsWithoutPk[i].name);

                if (reference != null && _dimensionCaches.TryGetValue(reference, out var cache))
                {
                    int sourceIdx = _refSourceCsvIndex[reference];
                    string lookupValue = sourceIdx >= 0 ? csvLine[sourceIdx] : string.Empty;

                    if (cache.TryGetId(lookupValue, out int dimId))
                        _stageParams[i].Value = dimId;
                    else
                        _stageParams[i].Value = System.DBNull.Value;
                }
                else
                {
                    int csvIdx = _csvToColumnMap[i];
                    string rawValue = csvIdx >= 0 && csvIdx < csvLine.Length ? csvLine[csvIdx] : string.Empty;
                    _stageParams[i].Value = _converters[i](rawValue);
                }
            }

            _stageCommand.ExecuteNonQuery();
        }
    }
}
