/*=======================================================================================================
 * 
 *   ### CMSC 124 Project ###
 * 
 *  Program Name :
 * 		Team Seven LOLCODE Interpreter
 * 
 *	Program Description:
 *		This program is a LOLCODE interpreter
 *
 *	Programming Language:
 *		Visiual C#
 *	
 *	Compiler:
 *		Monodevelop / Xamarin Studio
 *
 *	Authors:
 *		CMSC 124 B-3L
 *			TEAM SEVEN
 *				-Bayeta, Reynaldo III L.
 *				-Luis, Brent Ian
 *				-Sabino, Arthjoseph P.
 *
 *	Date Created: 
 *		October 29, 2015
 *
 *	Date Finshed:
 *		December 6, 2015
 *
 *========================================================================================================*/

using System;
using System.Text.RegularExpressions;

namespace Proto
{
	public partial class ScanDialog : Gtk.Dialog
	{
		public ScanDialog ()
		{
			this.Build ();
		}

		/*====================================================================================
		 * 
		 * 	OnButtonOkClicked ()
		 * 		- function that get the value of the scanner and destroy the scanner dialog
		 * 		- access MainWindow.type and MainWindow.string_IT for storing the type of the
		 *	 scanned literal and its value
		 *
		 *====================================================================================*/

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			string scan = Scanner.Text;
			MainWindow.type = "ERR";
			MainWindow.string_IT = scan;
			if (Regex.IsMatch (scan, @"^\s*-?\d+\s*$")) {
				MainWindow.IT = Convert.ToDouble (scan);
				MainWindow.type = "NUMBR";
			} else if (Regex.IsMatch (scan, @"^\s*-?\d+\.\d+\s*$")) {
				MainWindow.type = "NUMBAR";
				MainWindow.IT = Convert.ToDouble (scan);
			} else if (Regex.IsMatch (scan, @"^\s*(WIN|FAIL)\s*$")) {
				MainWindow.type = "TROOF";
			} else {
				if (scan == "" || scan==null || scan.Trim()=="") {
					MainWindow.type = "ERR";
				}else{
					MainWindow.type = "YARN";
				}
			}
			this.Destroy ();
		}
	}
}

