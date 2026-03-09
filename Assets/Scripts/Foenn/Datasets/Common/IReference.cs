using Assets.Scripts.Foenn.Datasets;
using Assets.Scripts.Foenn.ETL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.Atlas.Datasets.Common {
    public class TableReference {
        public Datafield ReferenceField { get; }
        public ITable Table { get; }
    }
}
