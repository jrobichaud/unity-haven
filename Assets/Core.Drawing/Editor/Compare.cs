using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

namespace CoreEditor.Drawing
{
	public static class Compare
	{
		public class CompareParams
		{
			public string FileA;
			public string FileB;

			public override string ToString()
			{
				return string.Format(@"-metric RMSE ""{0}"" ""{1}"" NULL:",  FileA, FileB );
			}
		}

		public static string Path
		{
			get
			{
				switch ( Application.platform )
				{
					case RuntimePlatform.OSXEditor:
						return "/usr/local/bin/compare";
					case RuntimePlatform.WindowsEditor:
						return Directory.GetCurrentDirectory() + @"\ExternalTools\Windows\compare.exe";
					default:
						throw new System.PlatformNotSupportedException();
				}
			}
		}
		
		public static bool IsInstalled
		{
			get
			{
				return File.Exists( Path );
			}
		}
		
		public static bool IsIdentical( string imageA, string imageB, bool verbose = false )
		{
			return IsIdentical( new CompareParams(){FileA = imageA, FileB = imageB }, verbose );
		}

		public static bool IsIdentical( CompareParams paramObj, bool verbose = false )
		{
			return RunCommand( paramObj.ToString(), verbose );
		}

		public static bool RunCommand( string command, bool verbose = false )
		{
			if ( IsInstalled == false )
			{
				Debug.LogError( "Cannot find " + Path );
				return false;
			}
			
			var proc = new Process();
			proc.StartInfo.FileName = Path;
			proc.StartInfo.Arguments = command;
			proc.StartInfo.CreateNoWindow = true;
			proc.StartInfo.WorkingDirectory =  Directory.GetCurrentDirectory();
			proc.StartInfo.RedirectStandardOutput = true;
			proc.StartInfo.RedirectStandardError = true;
			proc.StartInfo.UseShellExecute = false;
			proc.StartInfo.EnvironmentVariables["PATH"] = System.Environment.GetEnvironmentVariable("PATH");
			proc.Start();

			var output = new List<string>();
			string line;
			while ( ( line = proc.StandardOutput.ReadLine() ) != null )
			{
				output.Add( line );
			}
			
			var error = new List<string>();
			while ( ( line = proc.StandardError.ReadLine() ) != null )
			{
				error.Add( line );
			}

			proc.WaitForExit();

			if ( verbose && error.Count > 0 )
				Debug.LogWarning( string.Join( "\n", error.ToArray() ) );

			return proc.ExitCode == 0;
		}
	}
}