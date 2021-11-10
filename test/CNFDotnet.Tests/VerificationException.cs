using System;

namespace CNFDotnet.Tests
{
    public class VerifcationException : Exception
    {
        public VerifcationException(string message)
            : base(message)
        { }
    }
}
