using UnityEngine;

public class CaptureScreenshot  
{
	[UnityEditor.MenuItem("Edit/CaptureScreenshot")]
	static void Capture()
	{
		ScreenCapture.CaptureScreenshot("image.png", 2);
	}
}