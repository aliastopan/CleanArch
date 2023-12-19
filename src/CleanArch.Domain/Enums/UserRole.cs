namespace CleanArch.Domain.Enums;

[Flags]
public enum UserRole
{
    Viewer = 0,
    Manager = 1,
    Administrator = 64,
}
