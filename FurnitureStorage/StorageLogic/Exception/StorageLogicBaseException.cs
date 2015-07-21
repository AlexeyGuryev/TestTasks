namespace StorageLogic.Exception
{
    public class StorageLogicBaseException : System.Exception
    {
        public StorageLogicBaseException(string format, params object[] args) : base(string.Format(format, args)) { }
    }
}
