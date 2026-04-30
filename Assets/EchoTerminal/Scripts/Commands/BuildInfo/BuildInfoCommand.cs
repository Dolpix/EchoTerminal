using System.Text;
using EchoTerminal.Scripts.TerminalCore;
using EchoTerminal.Scripts.TerminalCore.Attributes;
using UnityEngine;

namespace EchoTerminal.Scripts.Commands
{
public static class BuildInfoCommand
{
	private const string _colLabel = "#C9D1D9";
	private const string _colValue = "#79C0FF";
	private const string _colTrue = "#3FB950";
	private const string _colFalse = "#F85149";
	private const string _colHeader = "#58A6FF";

	[TerminalCommand]
	[TerminalDescription("Show project build information")]
	private static void LogBuildInfo([Inject] Terminal terminal)
	{
		var sb = new StringBuilder();

		Header(sb, "Project");
		Row(sb, "Name", Application.productName);
		Row(sb, "Version", Application.version);
		Row(sb, "Company", Application.companyName);

		sb.AppendLine(" ");
		Header(sb, "Build");
		BoolRow(sb, "Dev Build", Debug.isDebugBuild);
		BoolRow(sb, "Editor", Application.isEditor);
		Row(sb, "Build GUID", string.IsNullOrEmpty(Application.buildGUID) ? "N/A (Editor)" : Application.buildGUID);
		Row(sb, "Platform", Application.platform.ToString());
		Row(sb, "Unity", Application.unityVersion);

		sb.AppendLine(" ");
		Header(sb, "System");
		Row(sb, "OS", SystemInfo.operatingSystem);
		Row(sb, "CPU", SystemInfo.processorType);
		Row(sb, "CPU Cores", SystemInfo.processorCount.ToString());
		Row(sb, "RAM", $"{SystemInfo.systemMemorySize} MB");
		Row(sb, "GPU", SystemInfo.graphicsDeviceName);
		Row(sb, "VRAM", $"{SystemInfo.graphicsMemorySize} MB");
		Row(sb, "Graphics API", SystemInfo.graphicsDeviceType.ToString());

		sb.AppendLine(" ");
		Header(sb, "Display");
		Row(sb, "Resolution", $"{Screen.width} x {Screen.height}");
		Row(sb, "Refresh Rate", $"{Screen.currentResolution.refreshRateRatio.value:F0} Hz");
		Row(sb, "Fullscreen", Screen.fullScreen ? "Yes" : "No");

		terminal.Log(sb.ToString().TrimEnd());
	}

	private static void Header(StringBuilder sb, string title)
	{
		sb.AppendLine($"<color={_colHeader}>{title}</color>");
	}

	private static void Row(StringBuilder sb, string label, string value)
	{
		sb.AppendLine($"  <color={_colLabel}>{label,-14}</color>  <color={_colValue}>{value}</color>");
	}

	private static void BoolRow(StringBuilder sb, string label, bool value)
	{
		string col = value ? _colTrue : _colFalse;
		sb.AppendLine($"  <color={_colLabel}>{label,-14}</color>  <color={col}>{value}</color>");
	}
}
}