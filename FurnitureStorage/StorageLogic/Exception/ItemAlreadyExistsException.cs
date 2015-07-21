namespace StorageLogic.Exception
{
    public class ItemAlreadyExistsException : StorageLogicBaseException
    {
        public ItemAlreadyExistsException(string format, params object[] args) : base(format, args) { }
    }
}
