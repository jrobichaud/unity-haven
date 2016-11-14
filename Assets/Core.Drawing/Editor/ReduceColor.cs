using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

namespace CoreEditor.Drawing
{
	public static class ReduceColor
	{
		public class ReduceColorParams
		{
			public string InputFile;
			public int NumberOfColors;

			public override string ToString()
			{
				return string.Format(@" --force --ext {0} --speed 1 {1} ""{2}""", System.IO.Path.GetExtension(InputFile), NumberOfColors, InputFile );
			}
		}

		public static string Path
		{
			get
			{
				switch ( Application.platform )
				{
					case RuntimePlatform.OSXEditor:
					return Directory.GetCurrentDirectory() + @"/ExternalTools/Mac/pngquant";
					case RuntimePlatform.WindowsEditor:
						return Directory.GetCurrentDirectory() + @"\ExternalTools\Windows\pngquant.exe";
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

		public static bool RunCommand( ReduceColorParams paramObj )
		{
			return RunCommand( paramObj.ToString() );
		}

		public static bool RunCommand( string command )
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

			if ( error.Count > 0 )
				Debug.LogWarning( string.Join( "\n", error.ToArray() ) );

			return proc.ExitCode == 0;
		}
	}
}