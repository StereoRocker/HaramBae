using System;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

namespace GameEngine
{
    public class Log
    {
        private bool toConsole;
        private StreamWriter writer;

        public Log(string logpath)
        {
            // Toggle console output depending on the build mode
#if DEBUG
            toConsole = true;
#else
            toConsole = false;
#endif

            // Now set up the file to output
            writer = new StreamWriter(new BufferedStream(File.OpenWrite(logpath)));
        }

        ~Log()
        {
            // Flush the writer and close the stream
            if (writer != null)
            {
                writer.Flush();
                writer.Close();
            }
            writer = null;
        }

        // Close function
        public void Close()
        {
            if (writer != null)
            {
                writer.Flush();
                writer.Close();
            }
            writer = null;
        }

        // Base logging function
        private void WriteLog(string file, int line, string level, string message)
        {
            string composed = String.Format("{0} {1}:{2} - {3}", level, file, line, message);

            if (toConsole)
                Console.WriteLine(composed);
            try {
                writer.WriteLine(composed);
            } catch (Exception e)
            {
                e.ToString();
            }
        }

        // Externally visible shortcuts
        public void Error(string message, [CallerFilePath] string file="", [CallerLineNumber] int line=0)
        {
            WriteLog(file, line, "Error", message);
        }

        public void Warning(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            WriteLog(file, line, "Warning", message);
        }

        public void Info(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            WriteLog(file, line, "Info", message);
        }

        public void Debug(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            WriteLog(file, line, "Debug", message);
        }
    }
}
