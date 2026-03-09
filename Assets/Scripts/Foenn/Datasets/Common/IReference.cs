using Assets.Scripts.Foenn.Datasets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.Atlas.Datasets.Common {
    public class TableReference {
        public DataField ReferenceField { get }
        public ITable Table { get; }
    }
}
