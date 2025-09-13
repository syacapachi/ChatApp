using System;

public abstract partial class AuthManagerBase
{
    public abstract void SignUp(string email, string password, string username, Action<bool, string> callback);
    public abstract void SignIn(string email, string password, Action<bool, string> callback);
    public abstract void SignOut();

    public abstract string CurrentUserId { get; }
    public abstract string CurrentUserName { get; }

    // ✅ 静的アクセスポイント（UIからはここを呼ぶだけ）
    public static AuthManagerBase Instance { get; protected set; }
}
