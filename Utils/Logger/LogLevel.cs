namespace ShiJing.Utils.Log
{
    /// <summary>
    /// 日志等级。
    /// </summary>
    public enum LogLevel
    {
        /// <summary>跟踪：最详细的诊断信息，通常仅用于开发调试。</summary>
        Trace = 0,

        /// <summary>调试：用于开发阶段的辅助信息。</summary>
        Debug = 1,

        /// <summary>信息：常规运行流程信息，反映应用的正常状态。</summary>
        Info = 2,

        /// <summary>警告：非预期但可恢复的情况，提示潜在风险。</summary>
        Warn = 3,

        /// <summary>错误：运行时错误，影响部分功能但不导致应用崩溃。</summary>
        Error = 4,

        /// <summary>致命：严重错误，通常导致应用或核心功能不可用。</summary>
        Fatal = 5
    }
}
