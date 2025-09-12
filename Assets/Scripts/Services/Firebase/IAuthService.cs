using System;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using Firebase.Firestore;
using System.Collections.Generic;


//���S�ȃA�N�Z�X�C���^�[�t�F�[�X
//���[�U�[�̑����ɉ����ăA�N�Z�X�\�ȃw�b�_�[���Ⴄ(��ʃ��[�U�[���Ǘ��Ҍ����ɃA�N�Z�X�ł��Ȃ��悤�ɂ�����)
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
            //.Result�ڃ\�b�g���g�����Ƃ�await�Ȃ��ŕ\���ł���
            currentUser = task.Result.User;
            
            callback(true, "�T�C���A�b�v����");

            //���[�U�[�����󔒏ꍇ�̃��[�UID�������ݒ�Ƃ���B
            if(userName == "")
            {
                userName = currentUser.UserId;
            }

            currentUser.UpdateUserProfileAsync(new Firebase.Auth.UserProfile { DisplayName = userName });
            //�f�[�^�x�[�X�Ƀ��[�U�[����o�^
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            var userDoc = new Dictionary<string, object>()
            {
                {"username",userName },
                { "email", email },
            }; 
            //users(root) - UserId - (name,email)�̍\��
            db.Collection("users").Document(CurrentUserId).SetAsync(userDoc);
            Debug.Log("���[�U�[���o�^����");
        }); 
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
            callback(true, "���[�U�[�f�[�^�폜����");
        });
    }
}

