using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using Eksekusi;
using System.IO;

namespace LogonUI
{
	class Program
	{
		static void Main(string[] args)

		{
			string akun = "";
			string wlogon="SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon";
			string b = bacaRegistry(wlogon, "KunciViolaJones");
			string state = bacaRegistry(wlogon, "KunciViolaJonesState");
			string info=bacaRegistry(wlogon,"KunciViolaJonesInfo");
			String a = "";

			//pass argumen
			for (int i = 0; i < args.Length; i++)
			{

				a = a + " " + args[i];
			}

			//deteksi user logged on
			foreach (var item in Process.GetProcessesByName("explorer"))
			{
				akun = namaOwner(item.Id);
			}

			//kondisi saat pernah ada yang login
			var usernameP = akun.Split('\\');

			//pengunci kondisi on
			if (state != "2")
			{
				//kondisi shutdown /s atau /r atau /l
				if ((Process.GetProcessesByName("explorer").Length == 0))
				{
					setRegistry(wlogon, "KunciViolaJonesState", "0", RegistryValueKind.DWord);
					if ((Process.GetProcessesByName("ViolaJones").Length != 0))
					{
							mulai(b, "BootUP", true);
						
					}
					else
					{
						
						if ((info != "Unlock") && (info != "")&&(info=="SLogon")||(info=="Lock"))
						{
							mulai(b, "BootUP", true);
						}
					}
					Process p = new Process();
					try
					{
						p = Process.Start("LogonUIWindow.exe", a);
					}
					catch { }

					p.WaitForExit();
				}
				else
					if (usernameP[0] == Environment.MachineName)
					{
						
						//interactive
						Process p = new Process();
						try
						{
							p = Process.Start("LogonUIWindow.exe", a);
						}
						catch { }

						mulai(b, "Lock", true);
						p.WaitForExit();
					}
					else
					{
						try
						{
							Process p = new Process();
							p = Process.Start("LogonUIWindow.exe", a);
							mulai(b, "Lock2", true);
							p.WaitForExit();
						}
						catch { }
					}

			}
			else //pengunci kondisi off
			{
				try
				{

					Process p = Process.Start("LogonUIWindow.exe", a);
					p.WaitForExit();
				}
				catch { }
			}

			
		}

		private static void ExecuteOption(Param param)
		{
			if (param.PId != 0)
			{
				var process = Process.GetProcessById(param.PId);
				ExecuteOption(param, process);
				return;
			}
			if (param.Expression != null)
			{
				var list = Process.GetProcesses().Where(x => x.ProcessName.IndexOf(param.Expression, StringComparison.OrdinalIgnoreCase) >= 0);
				foreach (var process in list)
					ExecuteOption(param, process);
				return;
			}

		}

		private static void ExecuteOption(Param param, Process process)
		{
			Options option = param.Option;
			switch (option)
			{
				case Options.List:
					process.Print();
					break;
				case Options.Kill:
					process.Kill();
					break;
				case Options.Suspend:
					process.Suspend();
					break;
				case Options.Resume:
					process.Resume();
					break;
				default:
					throw new ArgumentException("");
			}
		}

		public static Param ProcessArgs(string[] args)
		{
			Param param;
			string commandLineParam2;
			if (args.Length < 1)
				throw new ArgumentException("");
			var option = ProcessOption(args[0]);
			if (option != Options.List && args.Length < 2)
				throw new ArgumentException("");
			if (args.Length < 2)
				commandLineParam2 = string.Empty;
			else
				commandLineParam2 = args[1];
			param = ProcessParam(commandLineParam2);
			param.Option = option;
			return param;
		}
		public static Param ProcessParam(string rawParam)
		{
			int result;
			var param = new Param();
			if (int.TryParse(rawParam, out result))
			{
				param.PId = result;
			}
			else
			{
				param.PId = 0;
				param.Expression = rawParam;
			}
			return param;
		}
		public static Options ProcessOption(string option)
		{
			Options progOptions;
			switch (option)
			{
				case "-l":
					progOptions = Options.List;
					break;
				case "-k":
					progOptions = Options.Kill;
					break;
				case "-s":
					progOptions = Options.Suspend;
					break;
				case "-r":
					progOptions = Options.Resume;
					break;
				default:
					throw new ArgumentException("");
			}
			return progOptions;
		}
		private static void setRegistry(string Registry, string value, string data, RegistryValueKind vk)
		{
			RegistryKey registry;
			if (Environment.Is64BitOperatingSystem)
			{
				registry = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
			}
			else
			{
				registry = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32);
			}
			try
			{
				RegistryKey regk = registry.CreateSubKey(Registry);
				regk.SetValue(value, data ,vk);
				regk.Close();
			}
			catch { }

		}

		private static string bacaRegistry(string Registry, string value)
		{	string a;
			RegistryKey registry;
			if (Environment.Is64BitOperatingSystem)
			{
				registry = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
			}
			else
			{
				registry = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32);
			}
			try
			{
				RegistryKey regk = registry.OpenSubKey(Registry, true);
				 a= regk.GetValue(value, null, RegistryValueOptions.None).ToString();
				regk.Close();
			}
			catch { return null; }
			return a;

		}
		public static string namaOwner(int PID)
		{
			var query = "select * from Win32_Process Where ProcessID= "+PID;
			ManagementObjectCollection PList;
			using (var cari = new ManagementObjectSearcher(query))
			{
				PList = cari.Get();

			}
			foreach (var item in PList.OfType<ManagementObject>())
			{
				object[] argList = { string.Empty, string.Empty };
				var returnv = Convert.ToInt32(item.InvokeMethod("GetOwner", argList));
				if (returnv==0)
				{
					return argList[1] + "\\" + argList[0];

				}
			}
			return "BlmAdaYangMenjalankanEx";

		}
		private static void mulai(string namaprogram, string argument, bool tunggu) {
			Process p=new Process();
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.Arguments = argument;
			p.StartInfo.FileName = namaprogram;
			p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			try
			{
				p.Start();
				if (tunggu) {
					p.WaitForExit();
				}
			}
			catch { }
		}
	}
}
