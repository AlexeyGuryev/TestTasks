namespace StorageLogic.Exception
{
    public class ItemNotFoundException : System.Exception
    {
        public ItemNotFoundException(string format, params object[] args) : base(string.Format(format, args)) { }
    }
}
