namespace StorageLogic.Exception
{
    public class ItemNotFoundException : StorageLogicBaseException
    {
        public ItemNotFoundException(string format, params object[] args) : base(format, args) { }
    }
}
