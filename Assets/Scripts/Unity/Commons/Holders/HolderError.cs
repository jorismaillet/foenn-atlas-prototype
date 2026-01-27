using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.Commons.Holders {
    public class HolderError : Exception {
        public HolderError(string message) : base(message) { }
    }
}
