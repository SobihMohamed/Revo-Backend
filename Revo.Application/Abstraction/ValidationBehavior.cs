using FluentValidation;
using MediatR;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Revo.Application.Abstraction
{
    public sealed class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Result
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            this.validators = validators;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // check if there are no validators for the request
            if (!validators.Any())
               return await next();
            // 2 - validate the request using all registered validators in parallel
            // validation context mean : it contains the request object and any additional information needed for validation
            // it hold also the errors that may occur during validation
            var context = new ValidationContext<TRequest>(request);

            // 3 - run all validators in parallel and collect the validation results
            var validationResults = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            
            //4 - collect all validation failures from the results
            // and call the error class we have made in domain layer to create a list of errors
            // with code and message for each error
            var ValidationFailures = validationResults
                .Where(VR => !VR.IsValid)
                .SelectMany(VR => VR.Errors)
                .Select(validationFailure => new Domain.Shared.Error(
                    validationFailure.ErrorCode,
                    validationFailure.ErrorMessage))
               .ToList();

            //5 - if there are any validation failures, return a failed result with the list of errors
            if (ValidationFailures.Count != 0)
            {
                return CreateValidationResult(ValidationFailures.ToArray());
            }

            return await next();

        }
        private TResponse CreateValidationResult(Domain.Shared.Error[] errors)
        {
            // create a new instance of ValidationError with the list of errors
            var validationError = new ValidationError(errors);
            // check if the response type is Result,
            // if so return a failure result with the validation error
            // but make boxing and unboxing to avoid type mismatch
            if (typeof(TResponse) == typeof(Result))
                return (TResponse)(object)Result.Failure(validationError);

            // if the response type is Result<T>, we need to get the generic type T
            // and create a failure result of that type example guid 
            var resultType = typeof(TResponse).GetGenericArguments()[0];

            var failureMethod = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod(nameof(Result.Failure));

            return (TResponse)failureMethod!.Invoke(null, new object[] { validationError })!;
        }
    }
}
