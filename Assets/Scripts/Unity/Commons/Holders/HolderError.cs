using System;

namespace Assets.Scripts.Unity.Commons.Holders
{
    public class HolderError : Exception
    {
        public HolderError(string message) : base(message) { }
    }
}
