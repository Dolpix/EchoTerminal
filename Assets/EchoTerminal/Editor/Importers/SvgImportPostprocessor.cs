using UnityEditor;

namespace EchoTerminal.Editor.Importers
{
public class SvgImportPostprocessor : AssetPostprocessor
{
	private const int _uiToolkitVectorImage = 3;
	private const string _iconsFolder = "EchoTerminal/UI/Icons/";

	private void OnPreprocessAsset()
	{
		if (!assetPath.EndsWith(".svg") || !assetPath.Contains(_iconsFolder))
		{
			return;
		}

		var so = new SerializedObject(assetImporter);
		SerializedProperty svgType = so.FindProperty("svgType");

		if (svgType == null || svgType.intValue == _uiToolkitVectorImage)
		{
			return;
		}

		svgType.intValue = _uiToolkitVectorImage;
		so.ApplyModifiedPropertiesWithoutUndo();
	}
}
}