using System;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;


//安全なアクセスインターフェース
//ユーザーの属性に応じてアクセス可能なヘッダーが違う(一般ユーザーが管理者権限にアクセスできないようにするやつ)
public interface IAuthService 
{
    string CurrentUserId { get; }
    void SignUp(string email, string password, System.Action<bool, string> callback);
    void SignIn(string email, string password, System.Action<bool, string> callback);
    void SignOut();
}

public interface AdministerService : IAuthService
{
    void DeleteUserData();
}

public class FirebaseAuthService : IAuthService,AdministerService
{
    private FirebaseAuth auth;
    private FirebaseUser currentUser = null;
    public FirebaseAuthService()
    {
        auth = FirebaseAuth.DefaultInstance;
    }
    public string CurrentUserId => Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
    

    public void SignUp(string email, string password,System.Action<bool, string> callback)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                callback(false, task.Exception.ToString());
                return;
            }
            //.Result目ソットを使うことでawaitなしで表現できる
            currentUser = task.Result.User;
            callback(true, "サインアップ成功");
        }); ;
        //await result.User.UpdateUserProfileAsync(new Firebase.Auth.UserProfile { DisplayName = displayName });
        // users/{uid} ドキュメント作成もここで
    }

    public void SignIn(string email, string password, System.Action<bool, string> callback)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                callback(false, task.Exception.ToString());
                return;
            }

            currentUser = task.Result.User;
            callback(true, "ログイン成功");
        }); ;
    }

    public void SignOut()
    {
        auth.SignOut();
        currentUser = null;
        //return Task.CompletedTask;
    }
    public void DeleteUserData()
    {
        currentUser.DeleteAsync();
    }
}

