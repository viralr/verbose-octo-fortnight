using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServerApp.Data.Interfaces;
using ServerApp.Models;
using ServerApp.Resources;
using ServerApp.Services;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        Mock<IOrderData> orderData = new Mock<IOrderData>();
        Mock<IWorkerData> workerData = new Mock<IWorkerData>();
        Mock<ILogger<OrderService>> logger = new Mock<ILogger<OrderService>>();
        private OrderService orderService;

        [TestInitialize]
        public void TestInitialize()
        {
            orderService = new OrderService(orderData.Object, workerData.Object, logger.Object);
        }

        [TestMethod]
        public void TestAddOrder()
        {
            orderService.AddOrders(new List<OrderResource>() { new OrderResource(){OrderId = "1",
                OrderItems = new List<OrderItemResource>() { new OrderItemResource(){ } } } });

            orderData.Verify(o => o.AddOrder(It.IsAny<Order>()), Times.Once);
        }
    }
}
