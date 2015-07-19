namespace StorageLogic.Exception
{
    public class DateConsistenceException : System.Exception
    {
        public DateConsistenceException(string format, params object[] args) : base(string.Format(format, args)) { }
    }
}
