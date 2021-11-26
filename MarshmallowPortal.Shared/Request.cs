namespace MarshmallowPortal.Shared;

public enum TokenType
{
    Google = 1,
}

public class Request
{
    public string Token { get; init; }
    public TokenType TokenType { get; init; }
}