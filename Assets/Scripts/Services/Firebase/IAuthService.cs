using System;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using Firebase.Firestore;
using System.Collections.Generic;


//安全なアクセスインターフェース
//ユーザーの属性に応じてアクセス可能なヘッダーが違う(一般ユーザーが管理者権限にアクセスできないようにするやつ)
public interface IAuthService 
{
    string CurrentUserId { get; }
    string UserName { get; }
    void SignUp(string email, string password,string username, System.Action<bool, string> callback);
    void SignIn(string email, string password, System.Action<bool, string> callback);
    void SignOut();
}

public interface SettingsService : IAuthService
{
    FirebaseAuth settingAuth { get; }
    void CurrentUser();
    void DeleteUserData(System.Action<bool, string> callback);
}

public class FirebaseAuthService : IAuthService,SettingsService
{
    private FirebaseAuth auth;
    private FirebaseUser currentUser = null;
    
    public FirebaseAuthService()
    {
        auth = FirebaseAuth.DefaultInstance;
        //currentUser = Firebase.Auth.FirebaseUser
    }
    public string CurrentUserId => Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser?.UserId;

    public string UserName => Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.DisplayName;

    public FirebaseAuth settingAuth => auth;
    

    public void SignUp(string email, string password,string userName, System.Action<bool, string> callback)
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

            //ユーザー名が空白場合のユーザIDを初期設定とする。
            if(userName == "")
            {
                userName = currentUser.UserId;
            }

            currentUser.UpdateUserProfileAsync(new Firebase.Auth.UserProfile { DisplayName = userName });
            //データベースにユーザー名を登録
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            var userDoc = new Dictionary<string, object>()
            {
                {"username",userName },
                { "email", email },
            }; 
            //users(root) - UserId - (name,email)の構造
            db.Collection("users").Document(CurrentUserId).SetAsync(userDoc);
            Debug.Log("ユーザー名登録完了");
        }); 
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
        }); 
    }

    public void SignOut()
    {
        auth.SignOut();
        currentUser = null;
        //return Task.CompletedTask;
    }
    public void CurrentUser()
    {
        currentUser = auth.CurrentUser;
        Debug.Log("Get CurrentUser!");
    } 
    public void DeleteUserData(System.Action<bool, string> callback)
    {
        currentUser.DeleteAsync().ContinueWith(task =>
        {
            if (!task.IsFaulted)
            {
                callback(false, task.Exception.ToString());
                return;
            }
            if (task.IsCanceled)
            {
                callback(false, task.Exception.ToString());
                return;
            }
            callback(true, "ユーザーデータ削除成功");
        });
    }
}

