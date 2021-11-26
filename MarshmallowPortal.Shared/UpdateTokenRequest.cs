namespace MarshmallowPortal.Shared;

public class UpdateTokenRequest : Request
{
    public string OldToken { get; init; }
}