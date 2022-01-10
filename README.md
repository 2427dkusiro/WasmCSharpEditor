# WebAssembly(Wasm) CSharp Editor

日本語版のreadmeもあります。/ Japanese version of this document is available.

The majority of this project is C# code, although the display on Github may show it as a javascript project.

## Summary

A C# development environment that allows you to edit and execute code in a browser.

## Details

By using WebAssembly, the C# compiler and user code are executed in the browser.
The goal is to provide a development experience equivalent to that of existing development environments published as desktop applications, without the need for installation or other work.
I also plan to create a learning course for beginners who are beginning to learn C# by implementing a judging system similar to that used in competitive programming.

## How to use

### Requirements

+ Latest web browser; IE is not supported; the latest version of Google Chrome is recommended.
+ Service Worker enabled environment. firefox's private browsing mode will not work with Service Worker.

### Usage

You can try the latest development build at the [Github Pages](https://2427dkusiro.github.io/WasmCSharpEditor/)
Almost all C# features are supported. The language version is C#10.
You can use standard input/output APIs such as `Console.WriteLine` and `Console.ReadLine` as you normally do when writing console applications.

## Progress

### Implemented

+ A code editor with syntax highlighting, using the CodeMirror library.
+ Compilation and execution of user-written code.
+ Compile and execute in a separate thread using the Web workers API.
+ To display the standard output in a virtual console implemented in HTML.
+ Implementation of `Console.ReadLine` based on synchronous waiting between Workers using Service Worker and XHR.

### Implementing

#### High priority
+ Interruption of code execution and compilation.
+ Judge system implementation.

#### Middle priority

+ Saving and exporting code.
+ Accepting the defaulted code of the code editor as the URL query string.
+ Code formatter implementation.
+ Better syntax highlighting. (Syntax highlighting based on parsing)
+ Auto-completion of code (like intellisense)

#### Low priority

+ Changing compile option.
+ Creating learning cource.
+ Store the code online.

### Known bugs
+ 404 error workaround for SPA is not working.
+ The cache created by the Blazor runtime and the offline cache created by the service worker are duplicated.
+ The implementation of `Console.Read` is different from the specification. The specification is misinterpreted.
+ If the code written by the user allocates too much memory or writes too much to the console, the application will crash due to out of memory.
+ When user-written code causes stack overflow due to infinite recursive function calls, etc., the runtime crashes instead of raising a .NET exception.
+ The DLL to be loaded does not reflect the culture of the application.