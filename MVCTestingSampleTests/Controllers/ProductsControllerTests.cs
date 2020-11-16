using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MVCTestingSample.Controllers;
using MVCTestingSample.Models;
using MVCTestingSample.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MVCTestingSample.Controllers.Tests
{
    [TestClass()]
    public class ProductsControllerTests
    {
        [TestMethod()]
        public async Task Index_ReturnsAViewResult_WithAListOfAllProducts()
        {
            // Arrange
            Mock<IProductRepository> mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(repo => repo.GetAllProductsAsync())
                    .ReturnsAsync(GetProducts());

            ProductsController prodController = new ProductsController(mockRepo.Object);

            // Act
            IActionResult result = await prodController.Index();

            // Assert
            // If view is returned
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = result as ViewResult;

            // List<> passed into view
            var model = viewResult.ViewData.Model;
            Assert.IsInstanceOfType(model, typeof(List<Product>));

            //Ensure all products are passed
            List<Product> pModel = model as List<Product>;
            Assert.AreEqual(3, pModel.Count);
        }

        private List<Product> GetProducts()
        {
            return new List<Product>()
            {
                new Product()
                {
                    ProductId = 1, Name = "Computer", Price = "199.99"
                },
                new Product()
                {
                    ProductId = 2, Name = "Shoe", Price = "599.99"
                },
                new Product()
                {
                    ProductId = 3, Name = "Desk", Price = "200"
                }
            };
        }

        [TestMethod()]
        public void Add_ReturnsViewResult()
        {
            Mock<IProductRepository> mockRepo = new Mock<IProductRepository>();
            ProductsController controller = new ProductsController(mockRepo.Object);

            var results = controller.Add();

            Assert.IsInstanceOfType(results, typeof(ViewResult));
        }


        [TestMethod]
        public async Task AddPost_ReturnsRedirectAndAddsProduct_WhenModelStateIsValid()
        {
            Mock<IProductRepository> mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(repo => repo.AddProductAsync(It.IsAny<Product>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

            var controller = new ProductsController(mockRepo.Object);
            Product p = new Product()
            {
                Name = "Test",
                Price = "100"
            };
            var result = await controller.Add(p);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            var redirectResult = result as RedirectToActionResult;
            Assert.IsNull(redirectResult.ControllerName, "Controller name should not be specified in the redirect");

            Assert.AreEqual("Index", redirectResult.ActionName);

            mockRepo.Verify();
        }

        [TestMethod]
        public async Task AddPost_ReturnsWiewWithModel_WhenModelStateIsInvalid()
        {
            Mock<IProductRepository> mockRepo = new Mock<IProductRepository>();
            var controller = new ProductsController(mockRepo.Object);
            var invalidProduct = new Product()
            {
                Name = null, // Name is Required to be valid
                Price = "3",
                ProductId = 1
            };
            // Mark ModelState as Invalid
            controller.ModelState.AddModelError("Name", "Required");

            IActionResult result = await controller.Add(invalidProduct);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(Product));

            // Ensure Invalid Product is passed back to view
            Product modelBoundProduct = viewResult.Model as Product;
            Assert.AreEqual(modelBoundProduct, invalidProduct, "Invalid product should be passed back to View");

        }
    }
}