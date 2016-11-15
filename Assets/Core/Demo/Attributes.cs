using UnityEngine;
using CoreEngine;

[HelpBox("This is the HelpBox attribute")]
[HelpBox(
	"░░░░░░▄▄▄▄▀▀▀▀▀▀▀▀▄▄▄▄▄▄░░░░░░░",
	"░░░░░█░░░░▒▒▒▒▒▒▒▒▒▒▒▒░░▀▀▄░░░░",
	"░░░░█░░░▒▒▒▒▒▒░░░░░░░░▒▒▒░░█░░░",
	"░░░█░░░░░░▄██▀▄▄░░░░░▄▄▄░░░░█░░",
	"░▄▀▒▄▄▄▒░█▀▀▀▀▄▄█░░░██▄▄█░░░░█░",
	"█░▒█▒▄░▀▄▄▄▀░░░░░░░░█░░░▒▒▒▒▒░█",
	"█░▒█░█▀▄▄░░░░░█▀░░░░▀▄░░▄▀▀▀▄▒█",
	"░█░▀▄░█▄░█▀▄▄░▀░▀▀░▄▄▀░░░░█░░█░",
	"░░█░░░▀▄▀█▄▄░█▀▀▀▄▄▄▄▀▀█▀██░█░░",
	"░░░█░░░░██░░▀█▄▄▄█▄▄█▄████░█░░░",
	"░░░░█░░░░▀▀▄░█░░░█░█▀██████░█░░",
	"░░░░░▀▄░░░░░▀▀▄▄▄█▄█▄█▄█▄▀░░█░░",
	"░░░░░░░▀▄▄░▒▒▒▒░░░░░░░░░░▒░░░█░",
	"░░░░░░░░░░▀▀▄▄░▒▒▒▒▒▒▒▒▒▒░░░░█░",
	"░░░░░░░░░░░░░░▀▄▄▄▄▄░░░░░░░░█░░"
)]
[HelpBox("This script is used to show some attributes and the power of this Inspector's rewrite", Icon=HelpBoxIconType.Info)]
[HelpBox("Historically this system was created before Unity created that kind of Attributes.",
	"It was adapted later for compatibility.", Icon=HelpBoxIconType.Warning)]
[HelpBox("This is why some attributes has duplicates in Unity's Attributes. (Tooltip vs ToolTip, duh)", Icon=HelpBoxIconType.Error)]
public class Attributes : MonoBehaviour {

	[UnityEngine.Header("Comparison of Unity's [Tooltip] with [ToolTip]")]

	[UnityEngine.Tooltip("You knew it was there because I told you, don't you?")]
	public int unityToolTip = 0;

	[ToolTip("Most if not all Core attributes has '*' hint")]
	public int coreToolTip = 0;

	[UnityEngine.Header("Comparison of Unity's [Range] with [Slider] (Tooltip)")]

	[UnityEngine.Range(1,10)]
	public int range = 0;

	[Slider(1,10)]
	public int slider = 0;

	[UnityEngine.Header("Reorderable is awesome")]
	[ToolTip("Reorderable elements ironically does not support ExtendedInspector Attributes and UnityEngine.Header")]
	public string _;

	[Reorderable]
	public string[] thisArrayIsSortable = new string[]{};

	[System.Serializable]
	public class ArrayElementExample
	{
		public int someInt = 0;

		public AnimationCurve someCurve = new AnimationCurve();
	}

	[Reorderable]
	public ArrayElementExample[] itAlsoWorksWithComplexData = new ArrayElementExample[]{};


	[UnityEngine.Header("Other attributes")]

	[RegexPattern(@"^Foo\.Bar")]
	public string thisRegexShouldMatch = "Foo.Bar";

	[NonNull]
	public Object thisShouldNotBeNull = null;


	[TextureSizeCheck(32,32)]
	public Texture imageSizeCheck = null;

	[SortingLayer]
	public string sortingLayer = "";

	public enum TestEnum
	{
		Fine,
		[System.Obsolete]
		Warning,
		[System.Obsolete("",true)]
		Error
	}
	[NonObsolete]
	public TestEnum obsolete = TestEnum.Fine;
}
