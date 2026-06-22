namespace ShiJing.Utils.Log
{
    /// <summary>
    /// 日志输出目标的抽象基类。每个目标负责把一条 <see cref="LogEntry"/> 写入到具体介质
    /// （控制台、文件等）。子类只需实现 <see cref="Write"/>；生命周期由
    /// <see cref="LoggerService"/> 统一管理。
    /// </summary>
    public abstract class LogTarget : IDisposable
    {
        /// <summary>把一条日志写入目标介质。</summary>
        public abstract void Write(LogEntry entry);

        /// <summary>刷新缓冲（如文件流）。默认无操作。</summary>
        public virtual void Flush() { }

        /// <summary>释放目标持有的资源。默认无操作。</summary>
        public virtual void Dispose(bool disposing) { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
