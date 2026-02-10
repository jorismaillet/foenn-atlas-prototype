using Assets.Scripts.Foenn.ETL.Datasources;
using System.Data;
using System.Linq;

namespace Assets.Scripts.Foenn.ETL.Transformers
{
    public class Transformer
    {
        public Datasource datasource;

        public Transformer(Datasource datasource)
        {
            this.datasource = datasource;
        }

        public void Transform(Dataset dataset)
        {
            AddIdColumn(dataset);
            AddLineIdentifierColumn(dataset);
        }

        public void AddIdColumn(Dataset dataset)
        {
            dataset.fields.Insert(0, new Datafield("ID", Datatype.PRIMARY_KEY));
            dataset.lines.ForEach(line =>
            {
                line.Insert(0, "");
            });
        }

        public void AddLineIdentifierColumn(Dataset dataset)
        {
            var columnIndexes = dataset.fields.Select((f, i) => new { f.name, index = i }).ToDictionary(x => x.name, x => x.index);
            dataset.fields.Insert(1, new Datafield(datasource.InsertIdColumn(), Datatype.STRING));
            dataset.lines.ForEach(line =>
            {
                line.Insert(1, datasource.Identifier(columnIndexes, line));
            });
        }
    }
}