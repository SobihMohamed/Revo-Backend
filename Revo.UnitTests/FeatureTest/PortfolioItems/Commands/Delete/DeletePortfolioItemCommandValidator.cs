using Revo.Application.Features.PortfolioItems.Commands.Delete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.PortfolioItems.Commands.Delete
{
    public class DeletePortfolioItemCommandValidator 
    {
        private readonly DeletePortofolioItemCommandValidator _validator;
        public DeletePortfolioItemCommandValidator()
        {
            _validator = new DeletePortofolioItemCommandValidator();
        }
        [Fact]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            var command = new DeletePortfolioItemCommand(Guid.Empty);
            var result = _validator.Validate(command);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Id" && e.ErrorMessage == "Portfolio item Id is required.");
        }
    }
}
