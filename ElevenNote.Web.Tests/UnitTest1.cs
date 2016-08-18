using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using ElevenNote.Web.Models;

namespace ElevenNote.Web.Tests
{
    [TestClass]
    public class UnitTest1
    {
        public static bool ValidateObject(object o)
        {
            var results = new List<ValidationResult>();

            try
            {
                Validator.TryValidateObject(o, new ValidationContext(o), results, true /* validate all properties */);
                return results.Count == 0;
            }
            catch
            {
                return false;
            }

        }

        [TestMethod]
        public void EmailRequiredByModelWhenForgotten()
        {   //Arranged: create a variable that calls the class
            var model = new ForgotViewModel
            {
                Email = null
            };
            //Act: create a variable that tells us if our new class instance is a valid object or not.
            var result = ValidateObject(model);

            //Assert: "result is false" 
            Assert.IsFalse(result);  //invalid object and false assertion = true and test passes.
        }

        [TestMethod]
        public void PasswordRequiredforLogin()
        {
            var model = new LoginViewModel
            {
                Email = "e-mail@e-mail.com",
                Password = null
            };

            var result = ValidateObject(model);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EmailRequiredforLogin()
        {
            var model = new LoginViewModel()
            {
                Email = "test@test.com",  //e-mails should have reg. expression for e-mail formatting; it will accept any string as an e-mail.
                Password = "string"
            };

            var result = ValidateObject(model);

            Assert.IsFalse(result);
        }
    }
}
