namespace CodeRunner
{
    /// <summary>
    /// コンパイラメッセージの種類を表現します。
    /// </summary>
    public enum ErrorTypes
    {
        /// <summary>
        /// Hidden(非表示)レベル。
        /// </summary>
        Hidden,
        /// <summary>
        /// Info(メッセージ)レベル。
        /// </summary>
        Info,
        /// <summary>
        /// Warning(警告)レベル。
        /// </summary>
        Warning,
        /// <summary>
        /// Error(エラー)レベル。
        /// </summary>
        Error,
    }
}
