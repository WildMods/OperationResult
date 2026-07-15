using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationResult;

[Serializable]
public class ErrorStack : Exception
{
    private LinkedList<string> _context = [];
    private string? _stackTrace;
    public string? Stack => _stackTrace;
    
    private ErrorStack() {}

    public ErrorStack(string message, string? callSite = null) : base(message)
    {
        _stackTrace = callSite;
    }
    public static implicit operator ErrorStack(string message) => new(message);

    public ErrorStack(string message, Exception innerException, string? callSite = null) : base(message, innerException)
    {
        _stackTrace = callSite ?? innerException.StackTrace;
    }

    public ErrorStack Context(string message)
    {
        _context.AddFirst(message);
        return this;
    }

    public override string ToString()
    {
        StringBuilder str = new();
        if (Message != string.Empty)
        {
            str.AppendLine(Message);
        }
        else if (_context.Count != 0)
        {
            str.AppendLine(_context.Last!.Value);
        }
        else if (InnerException != null)
        {
            str.AppendLine($"{InnerException.GetType().ToString().Split(".").Last()}: {InnerException.Message}");
        }

        str.AppendLine().AppendLine("Caused by:");

        int i = -1;
        foreach (var value in _context)
        {
            str.AppendLine($"  {++i}. {value}");
        }

        if (Message != string.Empty)
        {
            str.AppendLine($"  {++i}. {Message}");
        }
        if (InnerException != null)
        {
            str.AppendLine()
                .AppendLine(
                    $"  {++i}. {InnerException.GetType().ToString().Split(".").Last()}: {InnerException.Message}"
                );
        }
        return str.ToString();
    }
}