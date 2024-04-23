namespace CmdRunner.Dtos
{
    /// <summary>
    /// Contains data about an exited process.
    /// </summary>
    public class ProcessResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessResult"/> class.
        /// </summary>
        /// <param name="exitCode">The process exit code.</param>
        /// <param name="stdout">The contents of the standard output stream.</param>
        /// <param name="stderr">The contents of the standard error stream.</param>
        /// <param name="timedOut">A value indicating whether the process has timed out.</param>
        public ProcessResult(int exitCode, string stdout, string stderr, bool timedOut)
        {
            ExitCode = exitCode;
            StandardOutput = stdout;
            StandardError = stderr;
            TimedOut = timedOut;
        }

        /// <summary>Gets the process exit code.</summary>
        public int ExitCode { get; }

        /// <summary>Gets the contents of the standard output stream.</summary>
        public string StandardOutput { get; }

        /// <summary>Gets the contents of the standard error stream.</summary>
        public string StandardError { get; }

        /// <summary>Gets a value indicating whether the process has timed out.</summary>
        public bool TimedOut { get; }
    }
}
