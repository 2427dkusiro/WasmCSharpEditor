# WebAssembly C#コードエディタ

## 概要
ブラウザでコード入力から実行まで完結するC#開発環境。

## 説明
WebAssebmlyを用いて、C#コンパイラからユーザーコードまでをすべてブラウザ上で動作させます。<br>
デスクトップアプリとしてのコードエディタと同等のエクスペリエンスを、インストールやセットアップなしに得られるようにすることを目標としています。<br>
簡易ジャッジシステムを実装したり、コードエディタをインラインで再利用可能にすることでC#学習アプリを構築することも1つの目標です。

## 使い方
[Github Pages](https://2427dkusiro.github.io/WasmCSharpEditor/)に開発中のビルドを公開しています。

## 進捗

### 実装済み
+ CodeMirrorによる、シンタックスハイライト付きのコードエディタの使用。
+ ユーザーコードのコンパイルと実行。
+ 標準出力リダイレクトにより、Console.WriteLineなどの実行結果がブラウザ上に表示されます。
+ コンパイルと実行は、アプリ本体とは別のWorker(≒プロセス)で実行されます。
+ ServiceWorker+XMLHttpRequestにより標準入力が伝達され、Console.ReadLineなどが利用できます。

### 実装予定

#### 実装可能性　確定
+ コンパイルやコード実行を中断できるようにします。(ワーカー立てるコストを隠蔽する方策が要るのでそれなりに重い実装。)
+ ジャッジシステムを実装し、ユーザーコードを採点します。(同上。TREした場合のコストを隠蔽。またテストケース等のフォーマット策定もいる。結構実装重い。)

#### 実装可能性　高
+ UIを最適化し、画面に応じてコードエディタサイズを調整します。(Web UIわかんね)
+ ユーザーコードを保存したりエクスポートしたりできるようにします。(実装的には軽め)
+ コードエディタページで、クエリ文字列として初期コードを指定できるようにします。

#### 実装可能性　中
+ 自動インデントをサポートします。
+ ユーザーコードをフォーマットします。
+ C#文法に沿ったシンタックスハイライトを提供します。
+ 入力候補を表示します。(ここまで言語仕様絡みの問題、Roslynをうまく利用できるか+UI実装できるか、既存コードエディタのカスタマイズできるか、というUI実装の問題もある。)
+ コンパイル/実行のオプションへのアクセスを提供します。(実装は軽いが優先度が低い)
+ コードエディタとコンテンツを混在させるあり方を検討し、実装します。(まずコア部分が完成しないことにはどうしようもない)
+ DLLのロードを最適化し、トラフィックとロード速度を改善します。(優先度が低い)

#### 既知の不具合
+ ルート以外のページへのアクセスが404になる(解決策既知)。
+ ServiceWorker依存性のため、初回アクセスで不具合がある。
+ キャッシュが重複してる(規定+service worker?)
+ Console.Readの実装が違う、仕様を誤解してる
+ ユーザーコードが大量のメモリアロケーションを行ったり、コンソールに大量の書き込みを行うとアプリケーション全体のメモリが不足する問題。(一定のところでMREで落としたい)
+ 依存先ライブラリが恐らくカルチャ依存DLLに対応していないため、現在のカルチャーを非依存以外に設定できず、コンパイルエラーや例外のテキストは英語になる問題。(ライブラリ側改修で対応？)

