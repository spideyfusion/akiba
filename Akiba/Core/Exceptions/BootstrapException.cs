using System;

namespace Akiba.Core.Exceptions
{
    [Serializable]
    class BootstrapException : Exception
    {
        private string BootstrapError;

        public override string Message
        {
            get
            {
                return BootstrapError;
            }
        }

        public BootstrapException(string message)
        {
            this.BootstrapError = message;
        }
    }
}
