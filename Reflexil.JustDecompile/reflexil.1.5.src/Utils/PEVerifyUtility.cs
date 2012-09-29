/* Reflexil Copyright (c) 2007-2012 Sebastien LEBRETON

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. */

#region " Imports "
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
#endregion

namespace Reflexil.Utils
{
    /// <summary>
    /// Wrapper for peverify.exe SDK utility
    /// </summary>
	static class PEVerifyUtility
    {

        #region " Constants "
        const string PV_FILENAME = "peverify.exe";
        #endregion

        #region " Properties "
        public static bool PEVerifyToolPresent
        {
            get
            {
                return File.Exists(PEVerifyToolFilename);
            }
        }

        private static string PEVerifyToolFilename
        {
            get
            {
                return SdkUtility.Locate(PV_FILENAME);
            }
        }
        #endregion

        #region " Methods "
        /// <summary>
        /// Call peverify.exe SDK utility
        /// </summary>
        /// <param name="arguments">Program arguments </param>
        /// <param name="show">Show utility window</param>
        /// <returns>True if successfull</returns>
        public static bool CallPEVerifyUtility(string arguments, bool show, Action<TextReader> outputhandler)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(PEVerifyToolFilename, arguments);
                startInfo.CreateNoWindow = !show;
                startInfo.RedirectStandardOutput = outputhandler != null;
                startInfo.UseShellExecute = false;
                Process pvProcess = Process.Start(startInfo);

                String lines = String.Empty;
                ThreadPool.QueueUserWorkItem((state) => lines = pvProcess.StandardOutput.ReadToEnd());

                pvProcess.WaitForExit();
                if (outputhandler != null)
                {
                    outputhandler(new StringReader(lines));
                }
                return pvProcess.ExitCode == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

	}
}