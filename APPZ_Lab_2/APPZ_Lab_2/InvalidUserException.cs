namespace APPZ_Lab_2
{
    using System;

    public class InvalidUserException : Exception
    {
        public InvalidUserException(string message) : base(message) { }
    }
}