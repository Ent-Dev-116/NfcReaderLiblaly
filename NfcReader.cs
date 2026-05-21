using MyPCSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Burst.CompilerServices;

/*
 * 備忘録
 * 4～129ページまでを0～125として扱う。
 */

public class NfcReader : IDisposable
{
    private IntPtr hContext = IntPtr.Zero;
    private IntPtr hCard = IntPtr.Zero;

    // ===== static共有 =====

    private static readonly IntPtr pci;
    private static readonly int pciLength =
        Marshal.SizeOf(typeof(Api.SCARD_IO_REQUEST));

    static NfcReader()
    {
        IntPtr handle = Api.LoadLibrary("Winscard.dll");
        pci = Api.GetProcAddress(handle, "g_rgSCardT1Pci");
    }

    // ====================

    /**
     * 戻り値 string
     * エラーの内容
     * 
     * 戻り値 int
     *  0 正常終了
     * -1 リーダー未接続
     * -2 NFC未接続
     */
    public (string errCode, int state) Connect()
    {
        uint ret = Api.SCardEstablishContext(
            MyPCSC.Constant.SCARD_SCOPE_USER,
            IntPtr.Zero,
            IntPtr.Zero,
            out hContext);

        if (ret != MyPCSC.Constant.SCARD_S_SUCCESS)
        {
            return ("Context生成失敗", -1);
        }

        uint pcchReaders = 0;

        Api.SCardListReaders(
            hContext,
            null,
            null,
            ref pcchReaders);

        if (pcchReaders <= 0)
        {
            return ("リーダーに接続できませんでした。", -1);
        }

        byte[] mszReaders = new byte[pcchReaders * 2];

        Api.SCardListReaders(
            hContext,
            null,
            mszReaders,
            ref pcchReaders);

        string readerName = Encoding.Unicode
            .GetString(mszReaders)
            .Split('\0')[0];

        IntPtr activeProtocol = IntPtr.Zero;

        ret = Api.SCardConnect(
            hContext,
            readerName,
            MyPCSC.Constant.SCARD_SHARE_SHARED,
            MyPCSC.Constant.SCARD_PROTOCOL_T1,
            ref hCard,
            ref activeProtocol);

        if (ret != MyPCSC.Constant.SCARD_S_SUCCESS)
        {
            return ("NFCを読み取ることができませんでした。", -2);
        }

        return ("正常に終了しました。", 0);
    }

    public string ReadUID()
    {
        byte[] send =
        {
            0xFF, 0xCA, 0x00, 0x00, 0x00
        };

        byte[] recv = new byte[256];
        int len = recv.Length;

        var io = new Api.SCARD_IO_REQUEST
        {
            cbPciLength = pciLength
        };

        uint ret = Api.SCardTransmit(
            hCard,
            pci,
            send,
            send.Length,
            io,
            recv,
            ref len);

        if (ret != MyPCSC.Constant.SCARD_S_SUCCESS)
        {
            throw new Exception("UID取得失敗");
        }

        return BitConverter.ToString(recv, 0, len - 2);
    }

    public byte[] ReadAllPages()
    {
        return ReadPage(0, 125);
    }

    public byte[] ReadPage(int stIndex, int enIndex)
    {
        return ReadPageMatrix(stIndex, enIndex)
            .SelectMany(x => x)
            .ToArray();
    }

    public List<byte[]> ReadPageMatrix(int stIndex, int enIndex)
    {
        // 特殊指定
        if (stIndex == -99 && enIndex == -99)
        {
            stIndex = -4;
            enIndex = 131;
        }
        else if (stIndex < 0 || enIndex > 125 || stIndex > enIndex)
        {
            throw new ArgumentOutOfRangeException();
        }

        stIndex += 4;
        enIndex += 4;

        List<byte[]> result =
            new List<byte[]>(enIndex - stIndex + 1);

        // ===== 使い回し =====

        byte[] recv = new byte[256];

        var io = new Api.SCARD_IO_REQUEST
        {
            cbPciLength = pciLength
        };

        // ===================

        for (int page = stIndex; page <= enIndex; page += 4)
        {
            byte[] send =
            {
                0xFF,
                0xB0,
                (byte)(page >> 8),
                (byte)(page & 0xFF),
                0x10
            };

            int len = recv.Length;

            uint ret = Api.SCardTransmit(
                hCard,
                pci,
                send,
                send.Length,
                io,
                recv,
                ref len);

            if (ret != MyPCSC.Constant.SCARD_S_SUCCESS ||
                recv[len - 2] != 0x90 ||
                recv[len - 1] != 0x00)
            {
                throw new Exception(
                    $"読み取り失敗 page={page}");
            }

            // 16byte → 4byte × 4ページ
            for (int i = 0; i < 16; i += 4)
            {
                int currentPage = page + (i / 4);

                if (currentPage > enIndex)
                {
                    break;
                }

                byte[] chunk = new byte[4];

                Buffer.BlockCopy(
                    recv,
                    i,
                    chunk,
                    0,
                    4);

                result.Add(chunk);
            }
        }

        return result;
    }

    public List<byte[]> ReadSystemMatrix()
    {
        return ReadPageMatrix(-99, -99);
    }

    public void StringWrite(byte[] data)
    {
        PageWrite(data, 0);
    }

    public void PageWrite(byte[] data, int st)
    {
        if (st < 0 || st > 125)
        {
            return;
        }

        int page = st + 4;

        byte[] recv = new byte[256];

        var io = new Api.SCARD_IO_REQUEST
        {
            cbPciLength = pciLength
        };

        for (int i = 0; i < data.Length; i += 4)
        {
            byte b0 = (i < data.Length)
                ? data[i]
                : (byte)0x00;

            byte b1 = (i + 1 < data.Length)
                ? data[i + 1]
                : (byte)0x00;

            byte b2 = (i + 2 < data.Length)
                ? data[i + 2]
                : (byte)0x00;

            byte b3 = (i + 3 < data.Length)
                ? data[i + 3]
                : (byte)0x00;

            byte[] send =
            {
                0xFF,
                0xD6,
                0x00,
                (byte)page,
                0x04,
                b0,
                b1,
                b2,
                b3
            };

            int len = recv.Length;

            uint ret = Api.SCardTransmit(
                hCard,
                pci,
                send,
                send.Length,
                io,
                recv,
                ref len);

            if (ret != MyPCSC.Constant.SCARD_S_SUCCESS ||
                recv[len - 2] != 0x90 ||
                recv[len - 1] != 0x00)
            {
                throw new Exception(
                    $"書き込み失敗 page={page}");
            }

            page++;
        }
    }

    public void Reset()
    {
        byte[] recv = new byte[256];

        var io = new Api.SCARD_IO_REQUEST
        {
            cbPciLength = pciLength
        };

        for (int page = 4; page < 130; page++)
        {
            byte[] send =
            {
                0xFF,
                0xD6,
                0x00,
                (byte)page,
                0x04,
                0x00,
                0x00,
                0x00,
                0x00
            };

            int len = recv.Length;

            uint ret = Api.SCardTransmit(
                hCard,
                pci,
                send,
                send.Length,
                io,
                recv,
                ref len);

            if (ret != MyPCSC.Constant.SCARD_S_SUCCESS ||
                recv[len - 2] != 0x90 ||
                recv[len - 1] != 0x00)
            {
                throw new Exception(
                    $"初期化失敗 page={page}");
            }
        }
    }

    public void Dispose()
    {
        if (hCard != IntPtr.Zero)
        {
            Api.SCardDisconnect(
                hCard,
                MyPCSC.Constant.SCARD_LEAVE_CARD);

            hCard = IntPtr.Zero;
        }

        if (hContext != IntPtr.Zero)
        {
            Api.SCardReleaseContext(hContext);

            hContext = IntPtr.Zero;
        }
    }
}