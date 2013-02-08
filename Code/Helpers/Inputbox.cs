/*
 * ajma.Utils.InputBox
 * Displays a prompt in a dialog box, waits for the user to input text or click a button, and then returns a string containing the contents of the text box.
 *  
 * Andrew J. Ma
 * ajmaonline@hotmail.com
 */

using System.Drawing;

namespace MediaFairy
{
	/// <summary>
	/// Displays a prompt in a dialog box, waits for the user to input text or click a button, and then returns a string containing the contents of the text box.
	/// </summary>
	public static class InputBox
	{
		/// <summary>
		/// Displays a prompt in a dialog box, waits for the user to input text or click a button, and then returns a string containing the contents of the text box.
		/// </summary>
		/// <param name="prompt">String expression displayed as the message in the dialog box.</param>
		/// <param name="title">String expression displayed in the title bar of the dialog box.</param>
		/// <returns>The value in the textbox is returned if the user clicks OK or presses the ENTER key. If the user clicks Cancel, a zero-length string is returned.</returns>
		public static string Show(string prompt, string title)
		{
			return Show(prompt, title, string.Empty, -1, -1);
		}

/*
		/// <summary>
		/// Displays a prompt in a dialog box, waits for the user to input text or click a button, and then returns a string containing the contents of the text box.
		/// </summary>
		/// <param name="prompt">String expression displayed as the message in the dialog box.</param>
		/// <param name="title">String expression displayed in the title bar of the dialog box.</param>
		/// <param name="defaultResponse">String expression displayed in the text box as the default response if no other input is provided. If you omit DefaultResponse, the displayed text box is empty.</param>
		/// <returns>The value in the textbox is returned if the user clicks OK or presses the ENTER key. If the user clicks Cancel, a zero-length string is returned.</returns>
		public static string Show(string prompt, string title, string defaultResponse)
		{
			return Show(prompt, title, defaultResponse, -1, -1);
		}
*/

		/// <summary>
		/// Displays a prompt in a dialog box, waits for the user to input text or click a button, and then returns a string containing the contents of the text box.
		/// </summary>
		/// <param name="prompt">String expression displayed as the message in the dialog box.</param>
		/// <param name="title">String expression displayed in the title bar of the dialog box.</param>
		/// <param name="defaultResponse">String expression displayed in the text box as the default response if no other input is provided. If you omit DefaultResponse, the displayed text box is empty.</param>
		/// <param name="xPos">Integer expression that specifies, in pixels, the distance of the left edge of the dialog box from the left edge of the screen.</param>
		/// <param name="yPos">Integer expression that specifies, in pixels, the distance of the upper edge of the dialog box from the top of the screen.</param>
		/// <returns>The value in the textbox is returned if the user clicks OK or presses the ENTER key. If the user clicks Cancel, a zero-length string is returned.</returns>
		private static string Show(string prompt, string title, string defaultResponse, int xPos, int yPos)
		{
			// Create a new input box dialog
// ReSharper disable UseObjectOrCollectionInitializer
			var frmInputBox = new InputBoxForm();
// ReSharper restore UseObjectOrCollectionInitializer
			frmInputBox.Title = title;
			frmInputBox.Prompt = prompt;
			frmInputBox.DefaultResponse = defaultResponse;
			if (xPos >= 0 && yPos >= 0)  frmInputBox.StartLocation = new Point(xPos, yPos);
			frmInputBox.ShowDialog();
			return frmInputBox.ReturnValue;
		}
	}
}
