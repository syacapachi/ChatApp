using UnityEngine;
using TMPro;
using System.Collections;

public static class TextWriter
{
    // �ÓI���\�b�h�F�^�C�s���O���ʕt���e�L�X�g�\��
    public static IEnumerator WriteText(TMP_Text textUI, string textToWrite, float speed = 0.05f)
    {
        textUI.text = ""; // �e�L�X�g���N���A

        // 1�������\��
        foreach (char character in textToWrite)
        {
            textUI.text += character;
            yield return new WaitForSeconds(speed);
        }
    }

    // �����Ƀe�L�X�g��\���i�^�C�s���O���ʂȂ��j
    public static void WriteTextInstant(TMP_Text textUI, string textToWrite)
    {
        textUI.text = textToWrite;
    }
}
