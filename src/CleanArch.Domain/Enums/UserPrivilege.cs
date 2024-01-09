namespace CleanArch.Domain.Enums;

[Flags]
public enum UserPrivilege
{
    Viewer = 0,
    Editor = 1,
    Administrator = 2,
}
