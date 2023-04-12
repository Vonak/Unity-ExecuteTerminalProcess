using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ProcessHelper
{
    /// <summary>
    /// Helper class to Execute Terminal commands on Windows and Mac
    /// Mac uses /bin/bash, Windows uses powershell.exe
    /// </summary>
    public static class ExecuteTerminalProcess
    {
        /// <summary>
        /// The Class Logger, default is Unity's Debug.unityLogger
        /// </summary>
        private static ILogger _logger = Debug.unityLogger;

#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        private static string _fileName = "/bin/bash";
        private static string _arguments = " -c \"{0} \"";
#else
        private static string _fileName = "powershell.exe";
        private static string _arguments = "-NoProfile -ExecutionPolicy unrestricted {0}";
#endif
        
        /// <summary>
        /// Register an <see cref="ILogger"/> for this class.
        /// </summary>
        /// <param name="p_logger">the Logger to use</param>
        public static void SetLogger(ILogger p_logger)
        {
            _logger = p_logger;
        }
        
        /// <summary>
        /// Set a custom FileName for the process to execute
        /// default is /bin/bash on Mac and powershell.exe on Windows
        /// </summary>
        /// <param name="p_fileName">the fileName, <see cref="ProcessStartInfo.FileName"/></param>
        public static void SetCustomFileName(string p_fileName)
        {
            _fileName = p_fileName;
        }

        /// <summary>
        /// Set a custom Argument for the process to execute
        /// default is "-NoProfile -ExecutionPolicy unrestricted" on Windows, "-c" on Mac
        /// </summary>
        /// <param name="p_arguments">the fileName, <see cref="ProcessStartInfo.Arguments"/></param>
        public static void SetCustomArguments(string p_arguments)
        {
            _arguments = p_arguments;
        }

        /// <summary>
        /// Execute and wait for a Terminal command
        /// </summary>
        /// <param name="p_argument">the command to execute</param>
        /// <param name="p_workingDirectory">the Working Directory</param>
        /// <param name="p_createWindow">Does it create a window</param>
        /// <param name="p_redirectStandardError">Does it redirect StandardError</param>
        /// <param name="p_redirectStandardInput">Does it redirect Standard Input</param>
        /// <param name="p_redirectStandardOutput">Does it redirect Standard Output</param>
        /// <returns>a StreamReader object, null in case of Exceptions</returns>
        public static StreamReader ExecuteCommand(string p_argument, string p_workingDirectory, 
            bool p_createWindow = false, bool p_redirectStandardError = true, bool p_redirectStandardInput = true, bool p_redirectStandardOutput = true)
        {
            try
            {
                _logger?.Log($"============== Start Executing [{p_argument??""}] ===============");
                var startInfo = new ProcessStartInfo()
                {
                    FileName = _fileName,
                    Arguments = string.Format(_arguments, p_argument),
                    UseShellExecute = false,
                    RedirectStandardError = p_redirectStandardError,
                    RedirectStandardInput = p_redirectStandardInput,
                    RedirectStandardOutput = p_redirectStandardOutput,
                    WorkingDirectory = p_workingDirectory,
                    CreateNoWindow = p_createWindow,
                };
                Process myProcess = new Process
                {
                    StartInfo = startInfo
                };
                myProcess.Start();
                string output = myProcess.StandardOutput.ReadToEnd();
                UnityEngine.Debug.Log(output);
                myProcess.WaitForExit();
                _logger?.Log("============== End ===============");
 
                return myProcess.StandardOutput;
            }
            catch (Exception ex)
            {
                _logger?.LogError("[ExecuteTerminalProcess]", $"{p_argument??""} threw an Exception {ex}");
                return null;
            }
        }
    }
}
