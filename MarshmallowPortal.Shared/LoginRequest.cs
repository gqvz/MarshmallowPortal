namespace MarshmallowPortal.Shared;

public class LoginRequest : Request
{
    public int ExpireTime { get; init; }
}