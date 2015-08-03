using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StorageLogic.Exception;
using StorageLogic.Model;
using StorageLogic.Service;

namespace StorageLogic.Test
{
    [TestClass]
    public class StorageServiceTest
    {
        private IStorageRepository repository = null;
        private StorageService service = null;
        private DateTime now = DateTime.Now;
        private Mock<IStorageRepository> mock = null;

        [TestInitialize]
        public void Init()
        {
            mock = new Mock<IStorageRepository>();
            mock.Setup(a => a.CreateRoom("Bathroom", now)).Returns(new Room()
            {
                CreationDate = now,
                Name = "Bathroom"
            });

            mock.Setup(a => a.CreateRoom("Bedroom", now)).Returns(new Room()
            {
                CreationDate = now,
                Name = "Bedroom"
            });

            mock.Setup(c => c.GetRoomByName("Bathroom")).Returns(new Room()
            {
                CreationDate = now,
                Name = "Bathroom"
            });

            repository = mock.Object;
            service = new StorageService(repository);

            //IStorageRepository repository1 = Mock.Of<IStorageRepository>(c =>
            //    c.CreateRoom("Bathroom", now).Equals(new Room() {CreationDate = now, Name = "Bathroom"})
            //    && c.GetRoomByName("Bathroom").Equals(new Room() {CreationDate = now, Name = "Bathroom"})
            //    );
        }

        [TestMethod]
        [ExpectedException(typeof(ItemAlreadyExistsException))]
        public void TestCreate()
        {
            service.CreateRoom("Bathroom", now);
        }

        [TestMethod]
        public void TestCreateCall()
        {
            mock.Setup(c => c.GetRoomByName(It.IsAny<string>()));
            service.CreateRoom("Bedroom", now);
            //mock.Verify(c => c.GetAllRoomStates(), Times.Once);
            mock.Verify();
        }
    }
}
