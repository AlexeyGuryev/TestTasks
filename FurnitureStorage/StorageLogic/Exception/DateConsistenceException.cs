namespace StorageLogic.Exception
{
    public class DateConsistenceException : StorageLogicBaseException
    {
        public DateConsistenceException(string format, params object[] args) : base(format, args) { }
    }
}
