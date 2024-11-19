using AzubiWiki_AzureFunctions_Program.Backend.Contracts.Queries;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Services.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace AzubiWiki_AzureFunctions_Program.Backend.UnitTest.Validation.Test
{
    public class Garage
    {

        [Fact]
        public void Validate_GarageValidCredentials_True()
        {
            Validate validate = new();
            GarageQ garage = new GarageQ();

            garage.BelongsTo = "Someone";
            bool pass = validate.Garage(garage);

            Assert.True(pass);
        }


        [Fact]
        public void Validate_GarageBelongsToIsNullOrWhitespace_AssertionFailedException()
        {
            Validate validate = new();
            GarageQ garage = new GarageQ();

            try
            {
                validate.Garage(garage);
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);

                try
                {
                    garage.BelongsTo = "           ";
                    validate.Garage(garage);
                }
                catch (Exception exc)
                {
                    Assert.IsType<XunitException>(exc);
                }
            }
        }
    }
}
