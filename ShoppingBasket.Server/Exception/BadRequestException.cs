//csharp ShoppingBasket.Server\Exceptions\BadRequestException.cs
using System;

namespace ShoppingBasket.Server
{
    /// <summary>
    /// Exception representing a client-side validation / bad request error.
    /// Service layer throws this; middleware maps it to HTTP 400.
    /// </summary>
    public class BadRequestException : Exception
    {
        public BadRequestException() : base() { }
        public BadRequestException(string message) : base(message) { }
        public BadRequestException(string message, Exception inner) : base(message, inner) { }
    }
}