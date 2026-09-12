using MediatR;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Abstraction
{
    public class Messaging
    {
        // The ICommand interface extends IRequest<Result> from MediatR,
        // indicating that it represents a request that will return a Result object.
        // not returning any data, just a success/failure result.
        public interface ICommand : IRequest<Result>
        {
        }

        // This interface represents a command that expects a response of type TResponse (data).
        public interface ICommand<TResponse> : IRequest<Result<TResponse>>
        {
        }

        // This interface defines a handler for commands that do not return any data,
        // only a success/failure result.
        public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
            where TCommand : ICommand
        {
        }

        // This interface defines a handler for commands that return a response of type TResponse.
        public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
            where TCommand : ICommand<TResponse>
        {
        }
        // Query always returns data, so it requires TResponse
        public interface IQuery<TResponse> : IRequest<Result<TResponse>>
        {
        }
        // This interface defines a handler for queries that return a response of type TResponse.
        public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
            where TQuery : IQuery<TResponse>
        {
        }
    }
}
