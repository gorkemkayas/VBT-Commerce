namespace BuildingBlocks.Application.Security;

public interface IRequireRole
{
    string[] AllowedRoles { get; }
}
