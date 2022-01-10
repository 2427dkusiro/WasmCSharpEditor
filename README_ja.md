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

#### high priority
+ Interruption of code execution and compilation.
+ Judge system implementation.

#### middle priority

+ Saving and exporting code.
+ Accepting the defaulted code of the code editor as the URL query string.
+ Code formatter implementation.
+ Better syntax highlighting. (Syntax highlighting based on parsing)
+ Auto-completion of code (like intellisense)

#### low priority

+ Changing compile option.
+ Creating learning cource.
+ Store the code online.

### 既知の不具合
+ 404 error workaround for SPA is not working.
+ The cache created by the Blazor runtime and the offline cache created by the service worker are duplicated.
+ The implementation of `Console.Read` is different from the specification. The specification is misinterpreted.
+ If the code written by the user allocates too much memory or writes too much to the console, the application will crash due to out of memory.
+ When user-written code causes stack overflow due to infinite recursive function calls, etc., the runtime crashes instead of raising a .NET exception.
+ The DLL to be loaded does not reflect the culture of the application.