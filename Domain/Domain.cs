using System;
using System.Linq;
using Optional;
using Optional.Collections;

namespace Domain
{
    public record Unit();
    public class DomainDefinedError : Exception {
        public DomainDefinedError(string message, Exception e) : base(message, e) { }
    }

    public class SystemError : DomainDefinedError {
        public SystemError(string message, Exception e) : base($"SystemError({message})", e) { }
    }

    public static class TryUtil    {
        public static Option<TResult, Exception> Try<TResult>(Func<TResult> tryFunction)
        {
            try
            {
                return Option.Some<TResult, Exception>(tryFunction());
            }
            catch (Exception e)
            {
                return Option.None<TResult, Exception>(e);
            }
        }

        public static Option<TResult, DomainDefinedError> ToOptionSystemError<TResult>(this Option<TResult, Exception> tryOption, string message) =>
            tryOption.Match(
                some: r => Option.Some<TResult, DomainDefinedError>(r),
                none: e => Option.None<TResult, DomainDefinedError>(new SystemError($"SystemError({message})", e))
            );
    }

    public static class ErrorUtil
    {
        public static Option<TResult[], DomainDefinedError> AggregateSystemError<TResult>(this Option<TResult, DomainDefinedError>[] results) =>
            results switch
            {
                Option<TResult, DomainDefinedError>[] r when r.Exceptions().ToArray().Length == 0 => Option.Some<TResult[], DomainDefinedError>(r.Values().ToArray()),
                Option<TResult, DomainDefinedError>[] r => Option.None<TResult[], DomainDefinedError>(new SystemError($"SystemError({r.Exceptions().Aggregate("", (es, e) => $"{es}, {e.Message}")}", new Exception()))
            };
    }
}
