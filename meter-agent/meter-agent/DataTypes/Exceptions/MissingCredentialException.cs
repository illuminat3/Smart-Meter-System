namespace meter_agent.DataTypes.Exceptions
{
    public class MissingCredentialException : Exception
    {
        public MissingCredentialException()
        {
        }

        public MissingCredentialException(string message)
            : base(message)
        {
        }

        public MissingCredentialException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
