namespace StorageLogic.Exception
{
    public class ItemAlreadyExistsException : System.Exception
    {
        public ItemAlreadyExistsException(string format, params object[] args) : base(string.Format(format, args)) { }
    }
}
