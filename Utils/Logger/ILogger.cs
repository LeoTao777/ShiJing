using System;

namespace ShiJing.Utils.Log
{
    /// <summary>
    /// 日志接口：供外部模块调用，屏蔽底层实现细节。
    /// </summary>
    public interface ILogger
    {
        /// <summary>日志来源标记，通常是当前类名或模块名。</summary>
        string Source { get; }

        /// <summary>记录 Trace 级别日志。</summary>
        void Trace(string message, Exception? exception = null);

        /// <summary>记录 Debug 级别日志。</summary>
        void Debug(string message, Exception? exception = null);

        /// <summary>记录 Info 级别日志。</summary>
        void Info(string message, Exception? exception = null);

        /// <summary>记录 Warn 级别日志。</summary>
        void Warn(string message, Exception? exception = null);

        /// <summary>记录 Error 级别日志。</summary>
        void Error(string message, Exception? exception = null);

        /// <summary>记录 Fatal 级别日志。</summary>
        void Fatal(string message, Exception? exception = null);

        /// <summary>
        /// 创建一个绑定到指定来源标记的子 logger，便于区分日志出处。
        /// </summary>
        ILogger ForSource(string source);
    }
}
