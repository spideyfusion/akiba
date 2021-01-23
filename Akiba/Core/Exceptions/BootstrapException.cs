namespace Akiba.Core.Exceptions
{
    using System;

    [Serializable]
    internal class BootstrapException : Exception
    {
        public override string Message => this.bootstrapError;

        private readonly string bootstrapError;

        public BootstrapException(string message) => this.bootstrapError = message;
    }
}
