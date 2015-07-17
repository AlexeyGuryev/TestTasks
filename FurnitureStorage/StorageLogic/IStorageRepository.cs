using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageLogic
{
    public interface IStorageRepository
    {
        /// <summary>
        /// Создать комнату
        /// <param name="name">название комнаты</param>
        /// <param name="creationDate">дата, на которую создается комната</param>
        void CreateRoom(string name, DateTime creationDate);

        void RemoveRoom(string name, DateTime removeDate);
    }
}
