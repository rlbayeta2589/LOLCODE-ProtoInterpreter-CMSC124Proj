
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
using Gtk;

namespace Proto
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();
		}
	}
}
