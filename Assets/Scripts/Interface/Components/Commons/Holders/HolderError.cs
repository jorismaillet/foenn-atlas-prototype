using System;

namespace Assets.Scripts.Components.Commons.Holders
{
    public class HolderError : Exception
    {
        public HolderError(string message) : base(message)
        {
        }
    }
}
