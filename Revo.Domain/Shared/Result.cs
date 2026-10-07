using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Revo.Domain.Shared
{
    public class Result
    {
        public bool IsSuccess { get;}
        public bool IsFailure => !IsSuccess;
        public Error Error { get;}
        [JsonConstructor] // this attribute is used to indicate that this constructor should be used for deserialization
        protected internal Result(bool isSuccess, Error error)
        {
            error ??= Error.None; // Ensure error is not null
            if (isSuccess && error != Error.None)
                throw new InvalidOperationException("Result is successful but has an error.");

            if (!isSuccess && error == Error.None)
                throw new InvalidOperationException("Result is failed but has no error.");

            IsSuccess = isSuccess;
            Error = error;
        }
        public static Result Success() => new(true, Error.None);
        public static Result Failure(Error error) => new(false, error);
    }
    public class Result<T> : Result
    {
        public T? Value { get; private set; }
        [JsonConstructor] // this attribute is used to indicate that this constructor should be used for deserialization 
        protected internal Result(bool isSuccess, T? value, Error error) 
            : base(isSuccess, error)
        {
            Value = value;
        }
        public static Result<T> Success(T value) => new(true, value, Error.None);
        public static new Result<T> Failure(Error error) => new(false, default(T), error);

    }
}
