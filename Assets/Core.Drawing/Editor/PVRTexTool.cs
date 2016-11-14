using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Debug = UnityEngine.Debug;


namespace CoreEditor.Drawing
{
	public static class PVRTexTool
	{
		public enum ResizeFilter {Nearest, Linear, Cubic}

		public class PVRTexToolParams
		{
			public string InputFile;
			public string OutputFile;
			public string PVROutputFile;
			public string EncodeFormat;
			public bool Dither;
			public int Width;
			public int Height;
			public ResizeFilter ResizeFilter = ResizeFilter.Linear;

			public override string ToString()
			{
				var sb = new StringBuilder( );
				if ( string.IsNullOrEmpty( InputFile ) == false )
					sb.AppendFormat( " -i \"{0}\"", InputFile );
				if ( string.IsNullOrEmpty( PVROutputFile ) == false )
					sb.AppendFormat( " -o \"{0}\"", PVROutputFile );
				if ( string.IsNullOrEmpty( OutputFile ) == false )
					sb.AppendFormat( " -d \"{0}\"", OutputFile );
				if ( string.IsNullOrEmpty( EncodeFormat ) == false )
					sb.AppendFormat( " -f {0}", EncodeFormat );
				if ( Dither )
					sb.Append( " -dither" );
				if ( Width > 0 && Height > 0 )
				{
					sb.AppendFormat( " -r {0},{1}", Width, Height );
					sb.AppendFormat( " -rfilter {0}", (ResizeFilter == ResizeFilter.Nearest ? "nearest" : (ResizeFilter == ResizeFilter.Linear ? "linear" : "cubic")) );
				}

				return sb.ToString();
			}
		}

		public static string Path
		{
			get
			{
				switch ( Application.platform )
				{
					case RuntimePlatform.OSXEditor:
						return Directory.GetParent(Application.dataPath) + "/ExternalTools/Mac/PVRTexToolCL";
					case RuntimePlatform.WindowsEditor:
						return Directory.GetParent(Application.dataPath) + @"\ExternalTools\Windows\PVRTexToolCL.exe";
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

		public static bool RunCommand( PVRTexToolParams paramObj )
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

			//if ( output.Count > 0 )
			//	Debug.Log( string.Join( "\n", output.ToArray() ) );
			if ( error.Count > 0 )
				Debug.LogWarning( string.Join( "\n", error.ToArray() ) );

			return proc.ExitCode == 0;
		}
	}
}