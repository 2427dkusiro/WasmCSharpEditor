using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WasmCsTest.UiLogics;
using WasmCsTest.WorkerConnection;

namespace WasmCsTest.Pages
{
    /// <summary>
    /// コードエディタページのロジック。
    /// </summary>
    public partial class CodeEdit
    {
        /// <summary>
        /// <see cref="CodeEdit"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public CodeEdit()
        {
            selectedTab = new(0);
            tab0 = new TabImplementHelper(selectedTab, 0);
            tab1 = new TabImplementHelper(selectedTab, 1);
        }

        private string? userCode;
        private CodeEditorContext? CodeEditorContext;

        private bool IsCompiling => CodeEditorContext is not null && (CodeEditorContext.CompileState == CompileStatus.CompileWaiting || CodeEditorContext.CompileState == CompileStatus.Compiling);
        private bool IsRunning => CodeEditorContext is not null && (CodeEditorContext.RunCodeState == RunCodeStatus.RunWaiting || CodeEditorContext.RunCodeState == RunCodeStatus.Running);

        private readonly Ref<int> selectedTab;
        private readonly TabImplementHelper tab0;
        private readonly TabImplementHelper tab1;

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            CodeEditorContext = new CodeEditorContext(CompileQueue);
            CodeEditorContext.AddUpdateUiCallBack(async () =>
            {
                StateHasChanged();
                await Task.Yield();
            });
        }

        /// <inheritdoc />
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await CompileQueue.TryInitialize(JS, Http);
        }

        private async Task OnRunButtonClicked()
        {
            if (CodeEditorContext is not null && userCode is not null)
            {
                await CodeEditorContext.Compile(userCode);
                if (CodeEditorContext.CompileResult is null)
                {
                    throw new InvalidOperationException();
                }
                if (CodeEditorContext.CompileResult.IsSuccessed)
                {
                    this.selectedTab.Value = 1;
                    await Task.Yield();
                    await CodeEditorContext.RunCodeAsync();
                }
                else
                {
                    this.selectedTab.Value = 0;
                }
            }
        }
    }

    /// <summary>
    /// razorでのタブの実装を簡略化します。
    /// </summary>
    class TabImplementHelper
    {
        private readonly Ref<int> selectedIndex;
        private readonly int targetIndex;

        /// <summary>
        /// <see cref="TabImplementHelper"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="selectedIndex">選択されているタブを保持する参照。</param>
        /// <param name="targetIndex">実装するタブのインデックス。</param>
        public TabImplementHelper(Ref<int> selectedIndex, int targetIndex)
        {
            this.selectedIndex = selectedIndex;
            this.targetIndex = targetIndex;
        }

        /// <summary>
        /// タブのヘッダーのclass属性の値を取得します。
        /// </summary>
        public string HeaderClassString
        {
            get
            {
                if (selectedIndex == targetIndex)
                {
                    return "nav-link active tab-active";
                }
                else
                {
                    return "nav-link header-nonselected";
                }
            }
        }

        /// <summary>
        /// タブのコンテンツ領域のclass属性の値を取得します。
        /// </summary>
        public string ContentClassString
        {
            get
            {
                if (selectedIndex == targetIndex)
                {
                    return "tab-content tab-active visible";
                }
                else
                {
                    return "tab-content hidden";
                }
            }
        }

        /// <summary>
        /// タブのヘッダーがクリックされたときの処理を実行します。
        /// </summary>
        public void OnHeaderClick()
        {
            selectedIndex.Value = targetIndex;
            // this.StateHasChanged();
        }
    }

    /// <summary>
    /// 擬似的な値型への参照を提供します。
    /// </summary>
    /// <typeparam name="T">対象の型。</typeparam>
    class Ref<T> where T : struct
    {
        /// <summary>
        /// <see cref="Ref{T}"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="value"></param>
        public Ref(T value)
        {
            Value = value;
        }

        /// <summary>
        /// この参照が示す値を取得または設定します。 
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// この参照が示す値を取得します。<see cref="Value"/> プロパティを読み出すことと同じです。
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator T(Ref<T> value)
        {
            return value.Value;
        }
    }
}
