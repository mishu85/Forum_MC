using System;

namespace ApplicationCore.Exeptions;

public class DuplicateException : Exception
{
    public DuplicateException(string message) : base(message)
    {

    }

}
