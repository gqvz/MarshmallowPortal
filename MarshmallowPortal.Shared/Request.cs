namespace MarshmallowPortal.Shared;

public enum TokenType
{
    Google = 1,
    Github = 2,
    Discord = 3
}

public class Request
{
    public string Token { get; init; }
    public TokenType TokenType { get; init; }
}