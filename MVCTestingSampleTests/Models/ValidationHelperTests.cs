using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCTestingSample.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MVCTestingSample.Models.Tests
{
    [TestClass()]
    public class ValidationHelperTests
    {
        [TestMethod()]
        [DataRow("9.99")]
        [DataRow("$20.00")]
        [DataRow(".99")] // Works with us currency
        [DataRow("$.09")]
        [DataRow("0")]
        [DataRow("100")]
        public void IsValidPrice_ValidPrice_ReturnsTrue(string input)
        {
            bool result = ValidationHelper.IsValidPrice(input);

            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("Five")]
        [DataRow("3 and 5 cents")]
        [DataRow("5 dollars")]
        [DataRow("50.00.1")]
        [DataRow("5.00$")]
        public void IsValidPrice_InValidPrice_ReturnsFalse(string input)
        {
            bool result = ValidationHelper.IsValidPrice(input);

            Assert.IsFalse(result);
        }
    }
}