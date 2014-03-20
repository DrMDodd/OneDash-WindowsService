using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace OneDash_WindowsService
{
	public partial class OneDashService : ServiceBase
	{
		private static Timer timer1 ;
		private static Timer timer2;
		private static Timer timer3;
		static string connectionString = "data source=localhost;initial catalog=onedash;user id=onedash;password=-OneD4sh-;";
		public delegate void updateLogFile(string text);

		protected override void OnStart(string[] args)
		{
			timer1 = new Timer(60 * 5 * 1000);
			timer2 = new Timer(60 * 5 * 1000);
			timer3 = new Timer(12 * 60 * 60 * 1000);
			timer1.Elapsed += timer1_Elapsed;
			timer2.Elapsed += timer2_Elapsed;
			timer3.Elapsed += timer3_Elapsed;
			
			//enabling the timer
			timer1.Enabled = true;
			System.Threading.Thread.Sleep(10000);
			//enabling the timer
			timer2.Enabled = true;
			System.Threading.Thread.Sleep(30000);
			//enabling the timer
			timer3.Enabled = true;
			
		}

		void timer3_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(OneDashService.connectionString))
				{
					SqlCommand command = new SqlCommand("spExportPAMSDevicesToOneDash", conn);
					conn.Open();
					command.ExecuteNonQuery();
					conn.Close();
					updateLogFile del = new updateLogFile(DoWork);
					del("PAMS Meters Updated");
				}
			}
			catch (Exception ex)
			{
				updateLogFile del = new updateLogFile(DoWork);
				del(ex.InnerException.ToString());
			}
		}

		void timer2_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(OneDashService.connectionString))
				{
					SqlCommand command = new SqlCommand("spExportPAMSServiceMetersToOneDash", conn);
					conn.Open();
					command.ExecuteNonQuery();
					conn.Close();
					updateLogFile del = new updateLogFile(DoWork);
					del("PAMS Meters Updated");
				}
			}
			catch (Exception ex)
			{
				updateLogFile del = new updateLogFile(DoWork);
				del(ex.InnerException.ToString());
			}
		}

		void timer1_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(OneDashService.connectionString))
				{
					SqlCommand command = new SqlCommand("spExportMineAPSReportDataToOneDash", conn);
					conn.Open();
					command.ExecuteNonQuery();
					conn.Close();
					updateLogFile del = new updateLogFile(DoWork);
					del("Mine APS Updated");
				}
			}
			catch (Exception ex)
			{
				updateLogFile del = new updateLogFile(DoWork);
				del(ex.InnerException.ToString());
			}
		}


		protected override void OnStop()
		{
			timer1.Enabled = false;
			timer2.Enabled = false;
			timer3.Enabled = false;
		}


		string windowsLogPath = @"c:\OneDashWS\WinServiceLog.txt";

		public OneDashService()
		{
			InitializeComponent();
			// Set up all the log paths
 			if(!Directory.Exists(@"c:\OneDashWS"))
			{
				Directory.CreateDirectory(@"c:\OneDashWS");
			}

			updateLogFile del = new updateLogFile(DoWork);
			del("hi");
		}


		void DoWork(string text)
		{
			using(StreamWriter sw = new StreamWriter(windowsLogPath, true))
			{
				sw.WriteLine(text.ToString());
			}
		}	
	}
}
