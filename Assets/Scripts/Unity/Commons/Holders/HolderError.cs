namespace Assets.Scripts.Unity.Commons.Holders
{
    using System;

    public class HolderError : Exception
    {
        public HolderError(string message) : base(message)
        {
        }
    }
}
