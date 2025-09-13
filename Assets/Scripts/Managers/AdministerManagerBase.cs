using System;

public abstract class AdministerManagerBase
{
    
    public abstract void DeleteUser(Action<bool,string> callback);
    public static AdministerManagerBase Instance{ get; protected set; }
}
