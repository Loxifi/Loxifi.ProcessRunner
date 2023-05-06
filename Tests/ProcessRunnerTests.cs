using System.Diagnostics;
using System.Text;

namespace Loxifi.Tests
{
	[TestClass]
	public class ProcessRunnerTests
	{
		[TestMethod]
		public async Task TestWrite()
		{
			string testString = "12345";

			ProcessSettings processSettings = new("cmd");

			StringBuilder outBuilder = new();

			processSettings.StdOutWrite += (sender, args) =>
			{
				Debug.WriteLine(args);
				_ = outBuilder.Append(args);
			};

			RunningProcess runningProcess = ProcessRunner.StartAsync(processSettings);

			runningProcess.WriteLine("ECHO " + testString);
			runningProcess.WriteLine("Exit");

			uint exitCode = await runningProcess;

			bool resultContains = outBuilder.ToString().Contains(testString);	

			Assert.IsTrue(resultContains);
		}

		[TestMethod]
		public async Task TestExecute()
		{
			string testString = "12345";

			ProcessSettings processSettings = new("cmd");

			StringBuilder outBuilder = new();

			processSettings.StdOutWrite += (sender, args) =>
			{
				Debug.WriteLine(args);
				_ = outBuilder.Append(args);
			};

			RunningProcess runningProcess = ProcessRunner.StartAsync(processSettings);

			runningProcess.WriteLine("ECHO " + testString);
			runningProcess.WriteLine("Exit");

			uint exitCode = await runningProcess;

			bool resultContains = outBuilder.ToString().Contains(testString);

			Assert.IsTrue(resultContains);
		}
	}
	}
