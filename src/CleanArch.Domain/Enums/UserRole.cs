namespace CleanArch.Domain.Enums;

[Flags]
public enum UserRole
{
    None = 0,
    Viewer = 1,
    Editor = 2,
    Manager = 4,
    Administrator = 64,
}
