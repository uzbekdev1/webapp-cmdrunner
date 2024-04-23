using System.Diagnostics;
using CmdRunner.Dtos;

namespace CmdRunner.Helpers
{
    /// <summary>
    /// Provides methods for process execution and handling.
    /// </summary>
    public static class ProcessHelper
    {
        /// <summary>
        /// Executes a process with a timeout and reads all output.
        /// </summary>
        /// <param name="fileName">The application to execute.</param>
        /// <param name="args">Command-line arguments to use when starting the application.</param>
        /// <param name="timeout">The time to wait for the process to exit. When this time elapses, the process is killed.</param>
        /// <param name="stdin">Optional content to pass in to the process. If null, nothing is sent.</param>
        /// <param name="cancellationToken">Indicates that the wait for the completion should be aborted.</param>
        /// <returns>The process result.</returns>
        public static Task<ProcessResult> Execute(string fileName, IEnumerable<string> args, TimeSpan timeout, string stdin = "", CancellationToken cancellationToken = default)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            foreach (string arg in args)
            {
                startInfo.ArgumentList.Add(arg);
            }
            return Execute(startInfo, timeout, stdin, cancellationToken);
        }

        /// <summary>
        /// Executes a process with a timeout and reads all output.
        /// </summary>
        /// <param name="startInfo">The process start information. Any stream redirection is enabled automatically.</param>
        /// <param name="timeout">The time to wait for the process to exit. When this time elapses, the process is killed.</param>
        /// <param name="stdin">Optional content to pass in to the process. If null, nothing is sent.</param>
        /// <param name="cancellationToken">Indicates that the wait for the completion should be aborted.</param>
        /// <returns>The process result.</returns>
        public static async Task<ProcessResult> Execute(ProcessStartInfo startInfo, TimeSpan timeout, string stdin = "", CancellationToken cancellationToken = default)
        {
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = stdin != null;

            bool timedOut = false;
            using var process = Process.Start(startInfo);

            if (process == null)
            {
                return null;
            }

            if (stdin != null)
            {
                process.StandardInput.Write(stdin);
                process.StandardInput.Close();
            }

            // Read both streams asynchronously (in parallel) so that the process won't block on
            // writing to them when we're not reading yet and the buffer becomes full. This
            // ensures we're consuming the stream in a timely manner.
            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();

            // Wait for both streams to close, as an indication that the process should be
            // completed. At least we know that we have all the output there is. If this doesn't
            // happen in a certain time, the process is killed asynchronously, which should also
            // close both streams sooner or later. Before accessing ExitCode, the process must
            // really have exited, so we also wait for that (still covered by the timeout).
            using (var timeoutCts = new CancellationTokenSource(timeout))
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token))
            using (cts.Token.Register(() => { try { process.Kill(); timedOut = true; } catch { } }))
            {
                await Task.WhenAll(stdoutTask, stderrTask);
                await process.WaitForExitAsync(cancellationToken);
            }

            return new ProcessResult(process.ExitCode, await stdoutTask, await stderrTask, timedOut);
        }

        /// <summary>
        /// Returns a string that represents a single argument. If necessary, it is quoted.
        /// </summary>
        /// <param name="arg">The argument string.</param>
        /// <returns>The quoted argument string.</returns>
        public static string GetArgString(string arg) =>
            string.IsNullOrWhiteSpace(arg) || arg.Contains(' ') ? "\"" + arg + "\"" : arg;

        /// <summary>
        /// Returns a string that represents multiple arguments. If necessary, each is quoted.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The quoted arguments string.</returns>
        public static string GetArgsString(IEnumerable<string> args) =>
            string.Join(' ', args.Select(a => GetArgString(a)));
    }
}
