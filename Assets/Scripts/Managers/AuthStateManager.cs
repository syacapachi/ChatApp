using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth; // Firebase Authenticationのインポート
using System; // EventHandlerのために必要

public class AuthStateManager : MonoBehaviour
{
    // FirebaseAuthのインスタンスを保持する変数
    private FirebaseAuth auth;

    void Awake()
    {
        //破棄されないようにする
        DontDestroyOnLoad(gameObject);
        // FirebaseAuthのデフォルトインスタンスを取得
        auth = FirebaseAuth.DefaultInstance;

        // StateChangedイベントにリスナーを登録
        // このリスナーは、認証状態が変わるたびにOnAuthStateChangedメソッドを呼び出す
        auth.StateChanged += OnAuthStateChanged;
        Debug.Log("FirebaseAuth.StateChanged リスナーを登録しました。");
    }

    // 認証状態が変更されたときに呼び出されるメソッド
    void OnAuthStateChanged(object sender, EventArgs eventArgs)
    {
        // 現在のユーザー情報を取得
        FirebaseUser user = auth.CurrentUser;

        if (user != null)
        {
            // ユーザーがログインしている場合
            Debug.Log($"ユーザーがログインしました: {user.DisplayName ?? "不明なユーザー名"} ({user.Email})");
            // 例: ログイン後の画面に遷移する、ログインUIを非表示にする
            if (SceneManager.GetActiveScene().name != "UserHomeScene")
            {
                SceneManager.LoadScene("UserHomeScene");
            }
            
        }
        else
        {
            // ユーザーがログアウトしている、またはログインしていない場合
            Debug.Log("ユーザーがログアウトしました、またはログインしていません。");
            // 例: ログイン画面を表示する、ゲームのメインメニューに戻す
            if (SceneManager.GetActiveScene().name != "LoginScene")
            {
                SceneManager.LoadScene("LoginScene");
            }
            
        }
    }

    void OnDestroy()
    {
        // オブジェクトが破棄される際に、メモリリークを防ぐためリスナーの登録を解除する
        if (auth != null)
        {
            auth.StateChanged -= OnAuthStateChanged;
            Debug.Log("FirebaseAuth.StateChanged リスナーを解除しました。");
        }
    }

    // 例として、ログインとログアウトのダミーメソッド
    // これらは本来、ログインボタンやログアウトボタンのクリックイベントなどで呼び出されます
    public void SignInAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("匿名ログインがキャンセルされました。");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"匿名ログインに失敗しました: {task.Exception}");
                return;
            }
            FirebaseUser newUser = task.Result.User;
            Debug.Log($"匿名ユーザーとしてログインしました: {newUser.UserId}");
        });
    }

    public void SignOut()
    {
        auth.SignOut();
        Debug.Log("ユーザーがログアウトしました。");
    }
}
