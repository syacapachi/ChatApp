using System;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;


//���S�ȃA�N�Z�X�C���^�[�t�F�[�X
//���[�U�[�̑����ɉ����ăA�N�Z�X�\�ȃw�b�_�[���Ⴄ(��ʃ��[�U�[���Ǘ��Ҍ����ɃA�N�Z�X�ł��Ȃ��悤�ɂ�����)
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
            //.Result�ڃ\�b�g���g�����Ƃ�await�Ȃ��ŕ\���ł���
            currentUser = task.Result.User;
            callback(true, "�T�C���A�b�v����");
        }); ;
        //await result.User.UpdateUserProfileAsync(new Firebase.Auth.UserProfile { DisplayName = displayName });
        // users/{uid} �h�L�������g�쐬��������
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
            callback(true, "���O�C������");
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

