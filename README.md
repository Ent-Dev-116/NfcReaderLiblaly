# NfcReaderLiblaly
このリポジトリ内のコードをすべてコピペすれば、ライブラリとして使えます。
不要なのは「TestPCSC.cs」だけです。

※以下、NfcReaderToolコピペ
## はじめに

<details><summary>このツールは、以下の条件において動きます。</summary>

### ハードウェア
>リーダー
>>[非接触ICカードリーダー／ライター PASORI RC-S300](https://www.amazon.co.jp/%E3%82%BD%E3%83%8B%E3%83%BC-NFC-FeliCa%E3%83%AA%E3%83%BC%E3%83%80%E3%83%BC-PaSoRi%EF%BC%88%E3%83%91%E3%82%BD%E3%83%AA%EF%BC%89%E6%A5%AD%E5%8B%99%E7%94%A8%E9%80%94%E5%B0%82%E7%94%A8%E3%83%A2%E3%83%87%E3%83%AB-RC-S300/dp/B0D66DGLVX/ref=sr_1_1?__mk_ja_JP=%E3%82%AB%E3%82%BF%E3%82%AB%E3%83%8A&crid=18T69W2EDS2K9&dib=eyJ2IjoiMSJ9.CcD7xj09aizlSJtD4CfgS5lbcs4bmajKfM1gYQ65THc8MCj_6D1eI-HwORHBxl7305qwvTJZhQpN9Y7hqpcOz5DxOCqEkjIHapbBspJjZnZEfWc027-nWHDTPO7cJV2ErPUMhP0inbxSMoCpPf8xueHArdmoWOZB9QLQX9ryL3Zgep68NZogEwa3j5PbkKuhIPZOTpc6ZC97qmj5FzjvvoGFIacoh_uvItIiv9JcjkRKHp9UP9Zs_iriBVx-Cm1Yglpywrch_M7LRJofIvKK22HgmdsiZi05hHn3lxkcKEU.8cz8fD_0Y7-LWG6P3JsH0MMnadxNGOMCMxYDogdfkfY&dib_tag=se&keywords=felica+s300&qid=1778066918&sprefix=felica+s300%2Caps%2C192&sr=8-1)

>nfcタグシール
>>[NFCタグシール NTAG215 １０枚セット](https://www.amazon.co.jp/NFC%E3%82%BF%E3%82%B0%E3%82%B7%E3%83%BC%E3%83%AB-%EF%BC%91%EF%BC%90%E6%9E%9A%E3%82%BB%E3%83%83%E3%83%88%E3%80%9050%E6%9E%9A%E3%81%BE%E3%81%A7%E9%81%B8%E3%81%B9%E3%82%8B%E6%9E%9A%E6%95%B0%E3%80%91%E7%99%BD%E7%84%A1%E5%9C%B0%E3%83%A9%E3%83%99%E3%83%AB-%E4%BD%BF%E3%81%84%E6%96%B9%E3%81%84%E3%82%8D%E3%81%84%E3%82%8D%EF%BC%81%E3%80%90%E3%83%86%E3%83%AC%E3%83%93%E3%81%A7%E7%B4%B9%E4%BB%8B%E3%81%95%E3%82%8C%E3%81%BE%E3%81%97%E3%81%9F%EF%BC%81%E3%80%91%E6%9B%B8%E3%81%8D%E6%8F%9B%E3%81%88%E5%8F%AF%E8%83%BD-NFC%E3%82%B9%E3%83%86%E3%83%83%E3%82%AB%E3%83%BC-NFC%E3%82%B7%E3%83%BC%E3%83%AB%E3%82%BF%E3%82%A4%E3%83%97/dp/B0FBRDNRLH/ref=sr_1_1_sspa?__mk_ja_JP=%E3%82%AB%E3%82%BF%E3%82%AB%E3%83%8A&crid=2O6FH8YXBUW92&dib=eyJ2IjoiMSJ9.KikXYQByyUiRHV2lUPWBGQNDo5Zt9zU87WHaun4jhHUzSR0cSI5lz0pTTDF0pxtNbv5Yn5mGwUOPbEXXkGXrOsBVYE4KOFpW3N8F-25dj4EWyoyhmEnDbEk-2RPIpVG4K3jeyX4PCOddPRC-eZv8LJAB-HcWArfRpsmtoqC_ZOxRN1Nj41S2DH1tn1BdGvuo5iviacXRWAATZrzVM0VDAljncbm-P1Wemi7aYAmipqqZZ-2H7WCDLj1TS9_P-SkXJsexKA9yAWhL2qGEdOTKa88NdCvISRRRLElcNWwwHTI.emGvufAt059gRWIFyuA4xrnhYybW7tndEb0zsUJzF0c&dib_tag=se&keywords=nfc%E3%82%BF%E3%82%B0+%E3%82%B7%E3%83%BC%E3%83%AB&qid=1778067120&sprefix=nfc%E3%82%BF%E3%82%B0+%E3%82%B7%E3%83%BC%E3%83%AB%2Caps%2C192&sr=8-1-spons&sp_csd=d2lkZ2V0TmFtZT1zcF9hdGY&psc=1)
### ソフトウェア
>環境
>>[VisualStudio](https://visualstudio.microsoft.com/ja/)

>言語、バージョン
>>[コンソール アプリ（C#）](https://learn.microsoft.com/ja-jp/visualstudio/get-started/csharp/tutorial-console?view=visualstudio)

>使用ライブラリ
>>[PCSC(7.0.1)](https://www.nuget.org/packages/PCSC)
>>[PCSC.Iso7816(7.0.1)](https://www.nuget.org/packages/PCSC.Iso7816)

</details>

ほかの環境で動かなくても責任は取れません。

## 要約
まず、VisualBasicで開いてください。
もしツールだけで使いたいのであれば、

NfcReaderTool/NFCTestApp/bin/Debug/

の中身をすべてダウンロードして、exeファイルを実行してください

### 構造

主に以下の要素で動いています。

- NFCTestApp
  - nfcapi
  - NfcReader
  - pcsc_const
  - Program.cs
  - WriteFrm
  - MainFrm

NFCを使用したい人が気にすべきは、**nfcapi**と**pcsc_const**と**NfcReader**の３つです。
とくに**NfcReader**が主な内容になっています。

## nfcapiとpcsc_const
これは、api定義と定数定義です。
それだけです。

## NfcReader
こちらがメインです。
しかし中身はごちゃごちゃしているので、使い方を説明します。

### Connect
引数はないです。
Nfcとの接続で、戻り値は「string,int」の二つです。サンプルコードにもありますが、「var」を用いて戻り値を受け取ってください。名前はそれぞれ「errCode,state」です。
**errCode**はエラーの内容の文字列で、**state**は、**0**が正常終了で、**-1**がContextの生成失敗、またはリーダー未接続で、**-2**がNFC読み取り失敗です。

### ReadUID
引数はないです。
使用すると**UIDが文字列として帰ってきます**。区切り文字はハイフンです。

### ReadAllPages
引数はないです。
使用すると**NFCタグの全データがバイト配列**で帰ってきます。

### ReadPage
引数は(int,int)で、最初と最後のページを指定して、その位置を読み取ります。
使用すると、**指定したページの読み取り結果**が帰ってきます。

### ReadAllPagesMatrix
引数はないです。
使用すると**NFCタグの全データがバイト配列リスト**で帰ってきます。
ページ毎のリスト１つ１つに、バイト配列が入っているイメージです。

### ReadPageMatrix
引数は(int,int)で、最初と最後のページを指定して、その位置を読み取ります。
使用すると、**指定したページの読み取り結果がバイト配列リスト**で帰ってきます。

### StringWrite
引数はバイト配列で、書き込むバイトデータを指定します。
使用すると、**0ページからデータを書き込んでいきます**。

### PageWrite
引数は(バイト配列,int)で、書き込むバイトデータと、書き込み始めるページを指定します。
使用すると、**指定したページからデータを書き込んでいきます**。

### Reset
引数はないです。
使用すると、**ユーザー範囲を初期化**します。

### Dispose
これはサンプルコードを見てもわかるように、Usingの外に出ると勝手に実行されるものです。
**気にしなくて大丈夫です。**
