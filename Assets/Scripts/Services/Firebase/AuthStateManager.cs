using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth; // Firebase Authentication�̃C���|�[�g
using System; // EventHandler�̂��߂ɕK�v

public class AuthStateManager : MonoBehaviour
{
    // FirebaseAuth�̃C���X�^���X��ێ�����ϐ�
    private FirebaseAuth auth;

    void Awake()
    {
        //�j������Ȃ��悤�ɂ���
        DontDestroyOnLoad(gameObject);
        // FirebaseAuth�̃f�t�H���g�C���X�^���X���擾
        auth = FirebaseAuth.DefaultInstance;

        // StateChanged�C�x���g�Ƀ��X�i�[��o�^
        // ���̃��X�i�[�́A�F�؏�Ԃ��ς�邽�т�OnAuthStateChanged���\�b�h���Ăяo��
        auth.StateChanged += OnAuthStateChanged;
        Debug.Log("FirebaseAuth.StateChanged ���X�i�[��o�^���܂����B");
    }

    // �F�؏�Ԃ��ύX���ꂽ�Ƃ��ɌĂяo����郁�\�b�h
    void OnAuthStateChanged(object sender, EventArgs eventArgs)
    {
        // ���݂̃��[�U�[�����擾
        FirebaseUser user = auth.CurrentUser;

        if (user != null)
        {
            // ���[�U�[�����O�C�����Ă���ꍇ
            Debug.Log($"���[�U�[�����O�C�����܂���: {user.DisplayName ?? "�s���ȃ��[�U�[��"} ({user.Email})");
            // ��: ���O�C����̉�ʂɑJ�ڂ���A���O�C��UI���\���ɂ���
            if (SceneManager.GetActiveScene().name != "UserHomeScene")
            {
                SceneManager.LoadScene("UserHomeScene");
            }
            
        }
        else
        {
            // ���[�U�[�����O�A�E�g���Ă���A�܂��̓��O�C�����Ă��Ȃ��ꍇ
            Debug.Log("���[�U�[�����O�A�E�g���܂����A�܂��̓��O�C�����Ă��܂���B");
            // ��: ���O�C����ʂ�\������A�Q�[���̃��C�����j���[�ɖ߂�
            if (SceneManager.GetActiveScene().name != "LoginScene")
            {
                SceneManager.LoadScene("LoginScene");
            }
            
        }
    }

    void OnDestroy()
    {
        // �I�u�W�F�N�g���j�������ۂɁA���������[�N��h�����߃��X�i�[�̓o�^����������
        if (auth != null)
        {
            auth.StateChanged -= OnAuthStateChanged;
            Debug.Log("FirebaseAuth.StateChanged ���X�i�[���������܂����B");
        }
    }

    // ��Ƃ��āA���O�C���ƃ��O�A�E�g�̃_�~�[���\�b�h
    // �����͖{���A���O�C���{�^���⃍�O�A�E�g�{�^���̃N���b�N�C�x���g�ȂǂŌĂяo����܂�
    public void SignInAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("�������O�C�����L�����Z������܂����B");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"�������O�C���Ɏ��s���܂���: {task.Exception}");
                return;
            }
            FirebaseUser newUser = task.Result.User;
            Debug.Log($"�������[�U�[�Ƃ��ă��O�C�����܂���: {newUser.UserId}");
        });
    }

    public void SignOut()
    {
        auth.SignOut();
        Debug.Log("���[�U�[�����O�A�E�g���܂����B");
    }
}
