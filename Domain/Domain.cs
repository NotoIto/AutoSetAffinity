using System;
using Optional;

namespace Domain
{
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
}
