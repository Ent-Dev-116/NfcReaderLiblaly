using System.Collections;
using UnityEngine;

public class TestPCSC : MonoBehaviour
{
    private bool isReading = false;

    void Start()
    {
        StartCoroutine(NfcLoop());
    }

    IEnumerator NfcLoop()
    {
        while (true)
        {
            // 多重実行防止
            if (!isReading)
            {
                isReading = true;

                try
                {
                    using (var nfc = new NfcReader())
                    {
                        var result = nfc.Connect();

                        if (result.state == 0)
                        {
                            var uid = nfc.ReadUID();

                            if (!string.IsNullOrEmpty(uid))
                            {
                                Debug.Log("NFC UID : " + uid);
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.Message);
                }

                isReading = false;
            }

            // CPU食い潰さないよう少し待機
            yield return new WaitForSeconds(0.2f);
        }
    }
}