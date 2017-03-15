using UnityEngine.UI;
using System.Linq;
public static class ToggleGroupExtension
{
	public static Toggle GetActive(this ToggleGroup aGroup)
	{
		return aGroup.ActiveToggles().FirstOrDefault();
	}
}