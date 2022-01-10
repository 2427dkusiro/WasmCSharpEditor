# WebAssembly(Wasm) C#エディタ

This is japanese version of readme file. / これは日本語版のreadmeです。
Github上での表示がjavascriptのプロジェクトとなっている可能性がありますが、このプロジェクトの大半はC#コードです。

## 概要

ブラウザ内でコードのコンパイルと実行が完結する、ブラウザ上のC#開発環境。

## 詳細

WebAssemblyを利用して、C#コンパイラとユーザーコードをブラウザ上で実行します。
このプロジェクトの目標は、デスクトップアプリケーションとして公開されているC#開発環境と同等の開発環境を、インストールなどの作業なしに利用できるようにすることです。
競プロのようなジャッジシステムを簡易的に実装し、C#入門者向けのC#学習コースを用意することも計画しています。

## 使い方

### 動作環境

+ 最新のwebブラウザ。IEはサポートされていません。Google Chromeの最新版を推奨します。
+ Service Workerが有効な環境。firefoxのプライベートブラウジングモードではservice workerが使用できません。

### 使用方法

最新の開発版は [Github Pages](https://2427dkusiro.github.io/WasmCSharpEditor/) で公開しています。
ほぼすべてのC#機能がサポートされています。現在の言語バージョンはC#10です。
`Console.WriteLine` や `Console.ReadLine` などの標準入出力APIが、普段コンソールアプリケーションを書くときと同じように使えます。

## 進捗

### 実装済み

+ A code editor with syntax highlighting, using the CodeMirror library.
+ Compilation and execution of user-written code.
+ Compile and execute in a separate thread using the Web workers API.
+ To display the standard output in a virtual console implemented in HTML.
+ Implementation of `Console.ReadLine` based on synchronous waiting between Workers using Service Worker and XHR.

### 実装中

#### 優先度高
+ コードのコンパイルと実行の中断。
+ ジャッジシステムの実装。

#### 優先度中

+ コードの保存とエクスポート。
+ コードエディターのデフォルトコードをURLのクエリ文字列として受け付けること。
+ コードのフォーマット。
+ C#構文解析に基づくシンタックスハイライト。
+ コードの自動補完(intellisenseのようなもの)

#### 優先度低

+ コンパイルオプションの変更機能。
+ C#学習コースの作成。
+ コードのオンライン保存。

### 既知の不具合
+ SPA用の404回避策が正常に機能しない。
+ BlazorランタイムのキャッシュとService Workerのオフラインキャッシュが重複する。
+ `Console.Read` の実装が仕様と異なる。仕様を誤って解釈しているのが原因。
+ ユーザーコードが大量のメモリ確保を行ったり、コンソールに大量の書き込みを行ったりした場合、アプリケーションがランタイムごとクラッシュする。
+ ユーザーコードが無限再起関数呼び出しなどの原因でstack overflowした場合、.NET例外が発生するのではなくランタイムがクラッシュする。
+ DLLの読み込みがカルチャを反映しない。