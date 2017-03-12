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
using System.Text.RegularExpressions;
using System.Collections.Generic;

public partial class MainWindow: Gtk.Window
{	
	ListStore LexemeStore = new ListStore (typeof (string), typeof (string));	//stores model that will hold two strings for lexemes and classification
	ListStore SymbolStore = new ListStore (typeof (string), typeof (string)); 	//stores model that will hold two strings for identifier and value
	LinkedList<string> stack = new LinkedList<string> (); 						//stack used for multiple operations in one line and pairing of statements
	LinkedList<string> operations = new LinkedList<string> (); 					//use for operations
	LinkedList<string> operands = new LinkedList<string> (); 					//use for operands
	Dictionary<string, string[]> variables = new Dictionary<string, string[]>(); 
	public static double IT = 0;
	public static Boolean boolean_IT = false;
	public static string string_IT = "";										//Implicit IT variable catches the value of stand alone operations or variables
	public static string hidden_IT = "";  										//		or literals ex. SUM OF 1 AN 2 BTW hidden_IT = 3 
	public static string type = ""; 											
	public static string[] stringofkeywords = new string[]{						// array of strings that contains keywords
		"HAI","KTHXBYE","I","HAS","A","OBTW","TLDR","ITZ","NOOB","NUMBR","NUMBAR",
		"YARN","TROOF","SMOOSH","ALL","ANY","OF","NOT","SUM","DIFF","PRODUKT","QUOSHUNT",
		"MOD","BIGGR","SMALLR","BOTH","EITHER","WON","NOT","AN","BTW","MKAY","SAEM","DIFFRINT",
		"VISIBLE","GIMMEH","WIN","FAIL","R","O","RLY?","YA","NO","WAI","OIC","WTF?","OMG","OMGWTF",
		"IT","GTFO","MEBBE","OF","HOW","IZ","I","YR","IF","U","SAY","SO","IM","IN","TIL","WILE"};
	LinkedList<string> keywordLinkedList = new LinkedList<string> (stringofkeywords); //linked list that contains string of keywords
	Boolean error = false; 
	Boolean autohide = true;

	/*====================================================================================
	 * 
	 * 	MainWindow ()
	 * 		- create the main window of the program
	 *
	 *====================================================================================*/

	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{

		Build ();
		LexTreeViewInit();

		Console.ModifyBase(StateType.Normal, new Gdk.Color(0,0,0));				//change the bg color of the console
		Console.ModifyText(StateType.Normal, new Gdk.Color(0,255,0));			//change the text color of the console
		TextEditor.FocusInEvent += HideConsole;
		LexTree.FocusInEvent += HideConsole;									//adding events
		SymbTree.FocusInEvent += HideConsole;

	}

	/*====================================================================================
	 * 
	 * 	OnAutoHideActionToggled ()
	 * 		- function that will execute if the user click the autohide option in the 
	 *	menu found in the upper part of the program 
	 *
	 *====================================================================================*/

	protected void OnAutoHideActionToggled (object sender, EventArgs e)
	{

		autohide = autohide ? false : true;
		if (autohide) {
			ExecuteBtn.HeightRequest = 50;			//hide the console and
			ConsoleLabel.Hide ();					//	make the execute button
			ConsoleWindow.Hide ();					//  larger
		} else {
			ExecuteBtn.HeightRequest = 0;			//show the console and
			ConsoleLabel.Show ();					//	restoring the size 
			ConsoleWindow.Show ();					//	of the execute button
		}
	}

	/*====================================================================================
	 * 
	 * 	HideConsole ()
	 * 		- function that hide the console and make the execute button larger
	 *
	 *====================================================================================*/

	protected void HideConsole (object e, FocusInEventArgs f)
	{
		if (autohide) {
			ExecuteBtn.HeightRequest = 50;
			ConsoleLabel.Hide ();
			ConsoleWindow.Hide ();
		}
	}

	/*====================================================================================
	 * 
	 * 	ShowConsole ()
	 * 		- function that show the console and restore the size of the execute button
	 *
	 *====================================================================================*/

	protected void ShowConsole ()
	{
		ExecuteBtn.HeightRequest = 0;
		ConsoleLabel.Show ();
		ConsoleWindow.Show ();
	}

	/*====================================================================================
	 * 
	 * 	OnDeleteEvent ()
	 * 		- close the program and destory the window
	 *
	 *====================================================================================*/

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Gtk.Application.Quit ();
		a.RetVal = true;
	}

	/*====================================================================================
	 * 
	 * 	OnAddFileActionActivated ()
	 * 		- function that will execute if the user click the add file option in the 
	 *	  menu found in the upper part of the program 
	 *		- this function will load the text in the file and put it in the text editor
	 *	  in the program
	 *
	 *====================================================================================*/

	protected void OnAddFileActionActivated (object sender, EventArgs e)
	{
		// use for adding text file and loading the contents to the console
		 using(FileChooserDialog chooser = new FileChooserDialog (null,"Open File",
        	null, FileChooserAction.Open,"Cancel", ResponseType.Cancel, 
		    "Open", ResponseType.Accept)) {
            if (chooser.Run () == (int)ResponseType.Accept) {
	            System.IO.StreamReader file = System.IO.File.OpenText (chooser.Filename);
	            TextEditor.Buffer.Text = file.ReadToEnd ();
	            file.Close ();
				chooser.Destroy();
			}
     	}
	}

	/*====================================================================================
	* 
	* 	LexTreeViewInit ()
	* 		- function that  initializes the tree view that will be shown in the main   
	*	  window withcolumns "Lexeme" and "Classification" under the name of "Lexemes"
	*	 	- also initializes the tree view that will be shown in the main window with 
	*	  columns "Identifier" and "Value" under the name of "Symbol Table"
	*
	*====================================================================================*/

	public void LexTreeViewInit (){
		//initializes the tree views
		TreeViewColumn LexemeColumn = new TreeViewColumn (); //instantiates the column that will contain the Lexemes
		TreeViewColumn ClassColumn = new TreeViewColumn (); //instantiates the column that will contain the classification of the lexemes
    	LexemeColumn.Title = "Lexeme";
   		ClassColumn.Title = "Classification";

	    LexTree.AppendColumn (LexemeColumn);
	    LexTree.AppendColumn (ClassColumn);

        LexTree.Model = LexemeStore; //stores the data from LexemeStore to LexTree.Model
		CellRendererText LexCell = new Gtk.CellRendererText ();
		LexemeColumn.PackStart (LexCell, true);
		CellRendererText ClassCell = new Gtk.CellRendererText ();
		ClassColumn.PackStart (ClassCell, true);

		LexemeColumn.AddAttribute (LexCell, "text", 0);
		ClassColumn.AddAttribute (ClassCell, "text", 1);

		/*===========================================================*/

		TreeViewColumn IdentColumn = new TreeViewColumn (); //instantiates the column that will contain the identifiers
		TreeViewColumn ValueColumn = new TreeViewColumn (); //instantiates the column that will contain the value of the identifiers
    	IdentColumn.Title = "Identifier";
   		ValueColumn.Title = "Value";

	    SymbTree.AppendColumn (IdentColumn);
	    SymbTree.AppendColumn (ValueColumn);

		SymbTree.Model = SymbolStore; //stores the data from SymbolStore to SymbTree.Model
		CellRendererText IdentCell = new Gtk.CellRendererText ();
		IdentColumn.PackStart (IdentCell, true);
		CellRendererText ValueCell = new Gtk.CellRendererText ();
		ValueColumn.PackStart (ValueCell, true);

		IdentColumn.AddAttribute (IdentCell, "text", 0);
		ValueColumn.AddAttribute (ValueCell, "text", 1);
	}

	/*====================================================================================
	* 
	* 	getValue ()
	* 		-function that gets the value of variable or operands/literals in the operands
	* 	LinkedList and check if it is in type of NUMBR or NUMBAR
	*		- return the first value in the operands stack
	*
	*====================================================================================*/

	protected double getValue ()
	{
		if (variables.ContainsKey (operands.First.Value)) {	//check it is in the variables list
			return Convert.ToDouble (variables [operands.First.Value] [1]);	
		} else if (CheckLiteral (operands.First.Value) == "NUMBR" || CheckLiteral (operands.First.Value) == "NUMBAR") {
															//check if type NUMBAR or NUMBR
			return Convert.ToDouble (operands.First.Value);
		} else {
			Console.Buffer.Text += "\nERR : Unexpected statement ::- " + operations.First.Value;			
			error = true;
			return 0;
		}
	}

	/*====================================================================================
	* 
	* 	Compute ()
	* 		- function that decide whether what operation shall be executed
	* 		-computes for the operands/ literals  and variables that is stored in the 
	* 	operands LinkedList according to the operation LinkedList
	*		-removes the operands in the operands LinkedList that will be used in evaluation
	*	in the operation LinkedList
	*		-removes the operation in the operation LinkedList that is used in evaluation
	*	of the operands LinkedList
	*		-add the result of the operations to the 'operations' stack
	*		-no return value
	*
	*====================================================================================*/

	protected void Compute (string op)
	{
		//use for computing operands according to operations that is derived from EvaluateOperations ()
		double value = 0;
		operations.RemoveFirst ();
		value = getValue();
		operands.RemoveFirst();
		if(op=="+")value += getValue(); 									//for addition
		else if(op=="-")value -= getValue(); 								//for subtraction
		else if(op=="*")value *= getValue(); 								//for multiplication
		else if(op=="/")value /= getValue(); 								//for division
		else if(op=="%")value = value % getValue(); 						//for modulo
		else if(op==">")value = (value > getValue() ? value : getValue()); 	//for greater
		else if(op=="<")value = (value < getValue() ? value : getValue()); 	//for lesser

		operands.RemoveFirst();
		operations.AddFirst(value.ToString());
	}

	/*====================================================================================
	* 
	* 	EvaluateOperations ()
	*		-evaluates operands that involves NUMBR or NUMBAR depending on the operands
	*	LinkedList and operations LinkedList
	*		-the actual "operands" that is stored in operands LinkedList is computed using
	*	the "operations" that is specified in the operations LinkedList
	*		-returns the string stored in IT or the string "ERR" if it encounters an error
	*	in the program
	*
	*====================================================================================*/

	protected string EvaluateOperations ()
	{
		//use for evaluating operations that involves NUMBR or NUMBAR from operation keywords
		IT = 0;
		Boolean an = false;
		int count = operations.Count;
		string[] copyop = new string[count];
		foreach (string eval in operations) { 	//recreate the operation stack to a array of strings
			count--;							// for identifying lexemes
			copyop[count] = eval;
		}
		foreach (string eval in copyop) {
			if (variables.ContainsKey (eval)) {	//check if it is in the variables list
												// and check the type of the operand
				if(variables[eval][0]=="YARN" || variables[eval][0]=="TROOF" || variables[eval][0]=="NOOB" ){
					Console.Buffer.Text += "\nERR : Unexpected operand ::- " + eval;			
					error = true;
					return "ERR";
				}
			}
			// stores the lexemes
			if (eval == "SUM") { 												// for SUM or addition
				LexemeStore.AppendValues (eval+" OF", "Addition Operand");	
			} else if (eval == "DIFF") { 										// for DIFF or Subtraction
				LexemeStore.AppendValues (eval+" OF", "Subtraction Operand");	
			} else if (eval == "PRODUKT") {										// for PRODUKT or Multiplication
				LexemeStore.AppendValues (eval+" OF", "Multiplication Operand");	
			} else if (eval == "QUOSHUNT") { 									// for QUOSHUNT or Division
				LexemeStore.AppendValues (eval+" OF", "Division Operand");	
			} else if (eval == "MOD") { 										// for MOD or Modulo
				LexemeStore.AppendValues (eval+" OF", "Modulo Operand");	
			} else if (eval == "BIGGR") { 										// for BIGGR or Max or Greater of
				LexemeStore.AppendValues (eval+" OF", "Max Operand");	
			} else if (eval == "SMALLR") { 										// for SMALLR or Min or Smaller of
				LexemeStore.AppendValues (eval+" OF", "Min Operand");	
			}else if (CheckLiteral(eval)=="NUMBR") {							//stores the NUMBR and NUMBAR literal and the AN conjunction
				LexemeStore.AppendValues (eval, "Integer Literal");
				if(an){	LexemeStore.AppendValues ("AN", "Conjunction");}
				an = an ? false : true;
			} else if (CheckLiteral(eval)=="NUMBAR") {
				LexemeStore.AppendValues (eval, "Float Literal");
				if(an){	LexemeStore.AppendValues ("AN", "Conjunction");}
				an = an ? false : true;
			} else {
				Console.Buffer.Text += "\nERR : Unexpected operand  ::- " + eval;
				error = true;
				return "ERR";
			}

		}
		/*=========================================================================
		 * 
		 * 	Evaluating the operation stack
		 * 
		 * 		> Traverse the stack until one operand remains
		 * 		> store all the NUMBR or NUMBAR literals to another stack
		 * 			which is the operand stack
		 * 		> if a operation was detected (eg. SUM, PRODUKT etc) call the
		 * 			compute function
		 *		> the last operand that remains will be the answer
		 *
		 *=========================================================================*/

		while (operations.Count!=1) {
			if (CheckLiteral (operations.First.Value) == "NUMBR" || CheckLiteral (operations.First.Value) == "NUMBAR") {
				operands.AddFirst (operations.First.Value);
				operations.RemoveFirst ();
			}
			// computes the operands according to the correspomdence to the operations 
			if (operands.Count >= 2) {
				string eval = operations.First.Value;
				if (eval == "SUM") { 					//if operations is SUM then it will add the value of the head
					Compute ("+");						//		 in operands the push it to the operations list
				} else if (eval == "DIFF") {			//if operations is DIFF then it will subtract the value of the
					Compute ("-");						//		 head in operands the push it to the operations list
				} else if (eval == "PRODUKT") {			//if operations is PRODUKT then it will multiply the value of 
					Compute ("*");						// 		the head in operands the push it to the operations list
				} else if (eval == "QUOSHUNT") { 		//if operations is QUOSHUT then it will divide the value of the head
					Compute ("/");						//		 in operands the push it to the operations list
				} else if (eval == "MOD") { 			//if operations is MOD then it will divide the value of the head
					Compute ("%");						//		 in operands the push the remainder it to the operations list
				} else if (eval == "BIGGR") { 			//if operations is BIGGR then it will add the value of the head in 
					Compute (">");						//		operands the push the larger to the operations list
				} else if (eval == "SMALLR") { 			//if operations is SMALLR then it will add the value of the head in 
					Compute ("<");						//		operands the push the smallerr  to the operations list
				}
			}
		}
		IT = Convert.ToDouble(operations.First.Value);	//the answer
		operations.RemoveFirst ();

		if(error) return "ERR";
		return CheckLiteral(IT.ToString());
	}

	/*====================================================================================
	* 
	* 	getBool ()
	* 		-the same as the funcion getValue but this time for Boolean operands
	*		-gets the value of variable and operands/literals in the operands LinkedList
	*	that is of type TROOF and returns a boolean
	*		-checks if it is WIN or FAIL
	*
	*====================================================================================*/

	protected Boolean getBool ()
	{
		if (variables.ContainsKey (operands.First.Value)) { //check if it is in the variables list
			return variables [operands.First.Value] [1] == "WIN" ? true : false;
		} else if (CheckLiteral (operands.First.Value) == "TROOF") { //check if type TROOF
			return operands.First.Value == "WIN" ? true : false;
		} else {
			Console.Buffer.Text += "\nERR : Unexpected statement ::- " + operations.First.Value;			
			error = true;
			return false;
		}
	}

	/*====================================================================================
	* 
	* 	Analyze ()
	* 		-the same as the funcion Compute but this time for Boolean operands
	* 		-function that decide whether what operation shall be executed
	*		-gets the value of variable and operands/literals in the operands LinkedList
	*	that is of type TROOF and returns a boolean
	*		-checks if it is WIN or FAIL
	*		-no return value
	*
	*====================================================================================*/

	protected void Analyze (string op)
	{
		Boolean value = false;
		operations.RemoveFirst ();
		value = getBool ();
		operands.RemoveFirst ();
		if (op == "&&")											//for and operation
			value = (value && getBool ()) ? true : false;
		else if (op == "||")									//for or operation
			value = (value || getBool ()) ? true : false;
		else if (op == "x||")									//for xor operation
			value = (value && !getBool () || !value && getBool ()) ? true : false;
		
		operands.RemoveFirst ();
		operations.AddFirst (value ? "WIN" : "FAIL");
	}

	/*====================================================================================
	* 
	* 	EvaluateBoolean ()
	* 		-the same as the funcion EvaluateOperations but this time for Boolean
	* 	operations
	*		-evaluates operands that involves TROOF depending on the operands LinkedList
	*	and operations LinkedList
	*		-the actual "operands" that is stored in operands LinkedList is computed
	*	using the "operations" that is specified in the operations LinkedList
	*		-returns the string stored in IT or the string "ERR" if it encounters an error
	*	in the program
	*
	*====================================================================================*/

	protected string EvaluateBoolean ()
	{
		//handles the evaluation of lolcode TROOF (boolean) operations
		string_IT = "";
		Boolean an = false;
		int count = operations.Count;
		string[] copyop = new string[count];
		foreach (string eval in operations) {	//recreate the operation stack to a array of strings
			count--;							// for identifying lexemes
			copyop [count] = eval;
		}
		foreach (string eval in copyop) {
			if (variables.ContainsKey (eval)) {	//check if it is in the variables list
												// and check the type of the operand
				if (variables [eval] [0] != "TROOF") {
					Console.Buffer.Text += "\nERR : Unexpected operand ::- " + eval;			
					error = true;
					return "ERR";
				}
			}
			if (eval == "BOTH") {												// for AND operations
				LexemeStore.AppendValues (eval + " OF", "AND Operand");	
			} else if (eval == "EITHER") {										// for OR operations
				LexemeStore.AppendValues (eval + " OF", "OR Operand");	
			} else if (eval == "WON") {											// for XOR operations
				LexemeStore.AppendValues (eval + " OF", "XOR Operand");	
			} else if (eval == "NOT") {											// for NOT operations
				LexemeStore.AppendValues (eval, "NOT Operand");	
			} else if (eval == "ALL") {											// for Arity AND operations
				LexemeStore.AppendValues (eval + " OF", "Arity AND Operand");	
			} else if (eval == "ANY") {											// for Arity OR operations
				LexemeStore.AppendValues (eval + " OF", "Arity OR Operand");	
			} else if (eval == "MKAY") {
				LexemeStore.AppendValues (eval, "Closing for Arity");
			} else if (CheckLiteral (eval) == "TROOF") {
				if (an) {
					LexemeStore.AppendValues ("AN", "Conjunction");
				}
				an = an ? false : true;
				LexemeStore.AppendValues (eval, "Boolean Literal");
			} else {
				Console.Buffer.Text += "\nERR : Unexpected operand  ::- " + eval;
				error = true;
				return "ERR";
			}

		}

		/*=========================================================================
		 * 
		 * 	Evaluating the operation stack
		 * 
		 * 		> Traverse the stack until one operand remains
		 * 		> store all the TROOF literals to another stack
		 * 			which is the operand stack
		 * 		> if a operation was detected (eg. BOTH, EITHER etc) call the
		 * 			analyze function
		 *		> the last operand that remains will be the answer
		 *		> in case the last operand in the operations stack is not a
		 *			troof literal, it means the program encountered a nested
		 *			not operations (not operations take only one operands) another
		 *			checking is done after
		 *
		 *=========================================================================*/

		while (operations.Count!=1) {
			if (operations.First.Value == "MKAY") {		//check if MKAY
				operations.RemoveFirst ();
				continue;
			}
			if (operands.Count >= 1) {
				if (operations.First.Value == "NOT") {	// check if NOT operation then invert the value
					operands.First.Value = operands.First.Value == "WIN" ? "FAIL" : "WIN";
					operations.RemoveFirst ();
					continue;
				}
			}
			if (CheckLiteral (operations.First.Value) == "TROOF") {// adding the operands
				operands.AddFirst (operations.First.Value);
				operations.RemoveFirst ();
			}

			if (operands.Count >= 2) {
			
				string eval = operations.First.Value;
				if (eval == "BOTH") {					//if operations is BOTH then it will perform the AND operation
					Analyze ("&&");						//		and the result will be push to the operations list
				} else if (eval == "EITHER") {			//if operations is EITHER then it will perform the OR operation
					Analyze ("||");						//		and the result will be push to the operations list
				} else if (eval == "WON") {				//if operations is WON then it will perform the XOR operation
					Analyze ("x||");					//		and the result will be push to the operations list
				} else if (eval == "ALL") {				//if operations is ALL then it will perform the Arity AND operation
					operations.RemoveFirst ();			//		and the result will be push to the operations list
					Boolean checker = true;
					foreach (string boo in operands) {
						checker = (checker && (boo == "WIN" ? true : false)) ? true : false;
						if (!checker)
							break;
					}
					operations.AddFirst (checker ? "WIN" : "FAIL");
				} else if (eval == "ANY") {				//if operations is ANY then it will perform the Arity OR operation
					operations.RemoveFirst ();			//		and the result will be push to the operations list
					Boolean checker = false;
					foreach (string boo in operands) {
						checker = (checker || (boo == "WIN" ? true : false)) ? true : false;
						if (checker)
							break;
					}
					operations.AddFirst (checker ? "WIN" : "FAIL");
				}
			}
		}

		if (operations.First.Value != "WIN" && operations.First.Value != "FAIL") {

			string eval = operations.First.Value;
				if (eval == "NOT") {
					operands.First.Value = operands.First.Value == "WIN" ? "FAIL" : "WIN";
					operations.RemoveFirst ();
					operations.AddFirst(operands.First.Value);
					operands.RemoveFirst();
				} else if (eval == "BOTH") {			//if operations is BOTH then it will perform the AND operation
					Analyze ("&&");						//		and the result will be push to the operations list
				} else if (eval == "EITHER") {			//if operations is EITHER then it will perform the OR operation
					Analyze ("||");						//		and the result will be push to the operations list
				} else if (eval == "WON") {				//if operations is WON then it will perform the XOR operation
					Analyze ("x||");					//		and the result will be push to the operations list
				} else if (eval == "ALL") {				//if operations is ALL then it will perform the Arity AND operation
					operations.RemoveFirst ();			//		and the result will be push to the operations list
					Boolean checker = true;
					foreach (string boo in operands) {
						checker = (checker && (boo == "WIN" ? true : false)) ? true : false;
						if (!checker)
							break;
					}
					operations.AddFirst (checker ? "WIN" : "FAIL");
				} else if (eval == "ANY") {				//if operations is ANY then it will perform the Arity OR operation
					operations.RemoveFirst ();			//		and the result will be push to the operations list
					Boolean checker = false;
					foreach (string boo in operands) {
						checker = (checker || (boo == "WIN" ? true : false)) ? true : false;
						if (checker)
							break;
					}
					operations.AddFirst (checker ? "WIN" : "FAIL");
				}
		}
		string_IT = operations.First.Value;		//the answer
		operations.RemoveFirst ();

		if(error) return "ERR";
		return CheckLiteral(string_IT);
	}

	/*====================================================================================
	* 
	* 	OperationType ()
	*		-checks the operation type and classifies it, then it goes to the specified
	*	classification for correct evaluation
	*		-if it detects an operation keyword it will classify it according to what
	*	operation type it does
	*		-returns a string that will specify what kind of operation type is involved
	*		-else returns the string "ERR" if it encounters an error
	*
	*====================================================================================*/

	protected string OperationType (string[] expr)
	{
		foreach (string s in expr) {if(s=="SAEM"||s=="DIFFRINT") return "RELATIONAL";} 
		foreach (string s in expr) {if(s=="SMOOSH") return "CONCATINATION";}
		foreach (string s in expr) {if(s=="SUM"||s=="DIFF"||s=="PRODUKT"||s=="QUOSHUNT"||s=="MOD"||s=="BIGGR"||s=="SMALLR") return "ARITHMETIC";}
		foreach (string s in expr) {if(s=="BOTH"||s=="EITHER"||s=="WON"||s=="NOT"||s=="ALL"||s=="ANY") return "BOOLEAN";}
		return "ERR";
	}

	/*====================================================================================
	* 
	* 	CheckLiteral ()
	*		-the most useful function in this program :D
	*		-checks the literal and stores the value to string_IT and returns the type
	*		-this function accepts and evaluates one parameter which is a literal or
	*			a statement
	*
	*		Possible values of the parameter
	*			1)	NUMBR		Interger	12345
	*			2)	NUMBAR		Float		123.45
	*			3)	YARN		String		"HELLO"
	*			3)	TROOF		Boolean		WIN
	*			4)  OPERATION
	*					SMOOSH		>	String Concatination
	*					ARITHMETIC	>	Operation for Integers and Floats
	*					BOOLEAN		> 	Operations for Boolean
	*					RELATIONAL	>	Operations for Comparison
	*			5)	VARIABLE
	*
	*====================================================================================*/

	protected string CheckLiteral (string token)
	{
		if(error) return "ERR";
		if (Regex.IsMatch (token, @"^\s*-?\d+\s*$")) {
			//if it is an integer it will return the type of NUMBR and stores the converted double to IT
			string_IT = token.Trim();
			IT = Convert.ToDouble(token);
			return "NUMBR";
		}else if (Regex.IsMatch(token, @"^\s*-?\d+\.\d+\s*$")){
			//if it is a float it will return the type of NUMBAR and stores the converted double to IT
			string_IT = token.Trim();
			IT = Convert.ToDouble(token);
			return "NUMBAR";
		}else if (Regex.IsMatch(token, @"^\s*"+"\""+".*"+"\""+" $")){
			//if it is a string/char it will return the type of YARN and stores the string to IT
			string[] check = token.Trim().Split (new char[] { '\"' }, StringSplitOptions.RemoveEmptyEntries);
			if(check.Length==1){
				string_IT = check[0];
				return "YARN";
			}else{
				Console.Buffer.Text += "\nERR : Unexpected statements ::- " + token;
				error = true;
				return "ERR";
			}
		}else if (Regex.IsMatch(token, @"^\s*(WIN|FAIL)\s*$")){
			//if it detects WIN or FAIL it returns TROOF
			string_IT = token.Split (new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
			return "TROOF";
		}else{
			int checker=0;
			//splitting of the parameter token
			string[] lex = token.Trim().Split (new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if(lex.Length>1){	// if the parameter consist of greater then 1 word it means it is an
								//		expression

				//if it falls to the CONCATINATION classification
				if(OperationType(lex)=="CONCATINATION"){
					if (lex [0] == "SMOOSH") {
						//for concatinating 
						LexemeStore.AppendValues (lex[0], "String Concatination");
						string temp = token.Trim().Split (new string[] { "SMOOSH " }, StringSplitOptions.RemoveEmptyEntries)[0];
						string[] concat = temp.Split (new string[] { " AN " }, StringSplitOptions.RemoveEmptyEntries);
						for(int i=0; i<concat.Length; i++){
							if(concat[i].Trim().StartsWith("\"") && !(concat[i].EndsWith("\""))){
								concat[i] = concat[i] + " AN " + '\"';		//this is when there is the word ' AN ' inside the 
							}												//	string because this program split the operands
																			//	by AN
							if(concat[i].Trim().EndsWith("\"") && !(concat[i].StartsWith("\""))){
								concat[i] = '\"' + concat[i];
							}

						}
						for (int i=0; i<concat.Length; i++) {				//this is for putting the literals in the lexeme
							string s = concat[i].Trim();	
							if (CheckLiteral (" " + s + " ") == "YARN") {
								string[] chomp = s.Split (new char[] { '\"' }, StringSplitOptions.RemoveEmptyEntries);
								if(chomp.Length==1){
									operations.AddLast (string_IT);
									LexemeStore.AppendValues ("\"", "String Delimiter");
									LexemeStore.AppendValues (string_IT, "String Literal");
									LexemeStore.AppendValues ("\"", "String Delimiter");
									if(i!=concat.Length-1) LexemeStore.AppendValues ("AN", "Conjunction");
								}else{
									Console.Buffer.Text += "\nERR : Expected AN statement between operands";
									return "ERR";
								}
							} else if (CheckLiteral (" " + s + " ") != "ERR") {
								string type = CheckLiteral (" " + s + " ");
								if(type=="NUMBR")LexemeStore.AppendValues (s, "Integer Literal");
								if(type=="NUMBAR")LexemeStore.AppendValues (s, "Float Literal");
								if(type=="TROOF")LexemeStore.AppendValues (s, "Boolean Literal");
								if(i!=concat.Length-1)LexemeStore.AppendValues ("AN", "Conjunction");
								operations.AddLast (string_IT.ToString ());
							} else {
								Console.Buffer.Text += "\nERR : Failed to execute string concatination";
								error = true;
								return "ERR";
							}
						}
						string result = "";
						foreach (string s in operations) {	// actual concatination
							result += s;
						}
						operations.Clear ();
						string_IT = result;		//the answer
						return "YARN";
					} else {
						Console.Buffer.Text += "\nERR : Unexpected operators or operands";
						error = true;
						return "ERR";
					}
				}
				//if it falls to the ARITHMETIC classification
				else if(OperationType(lex)=="ARITHMETIC"){
					for(int i=lex.Length-1;i>=0;i--){
						string s = lex[i];				//adding the operations and operands to the operations stack
						if(s=="AN"||s=="OF")continue;
						if((s=="SUM"||s=="DIFF"||s=="PRODUKT"||s=="QUOSHUNT"||s=="MOD"||s=="BIGGR"||s=="SMALLR") && lex[i+1]=="OF"){
							operations.AddLast(lex[i]);
						}else if((s=="SUM"||s=="DIFF"||s=="PRODUKT"||s=="QUOSHUNT"||s=="MOD"||s=="BIGGR"||s=="SMALLR") && lex[i+1]!="OF"){
							Console.Buffer.Text += "\nERR : Expected 'OF' statement for " + s;
							return "ERR";
						}else if(CheckLiteral(s)=="NUMBR"||CheckLiteral(s)=="NUMBAR"){
							if(i!=lex.Length-1 && i!=0){
								if(lex[i+1]=="AN"||lex[i-1]=="AN"){		//check for syntax error
									operations.AddLast(lex[i]);
								}else{
									Console.Buffer.Text += "\nERR : Expected 'AN' statement near operand ::- " + s;
									return "ERR";
								}
							}else{
								if(i==lex.Length-1){
									if(lex[i-1]=="AN"){
										operations.AddLast(lex[i]);
									}else{
										Console.Buffer.Text += "\nERR : Expected 'AN' statement near " + s;
										return "ERR";
									}
								}else if(i==0){
									if(lex[i+1]=="AN"){
										operations.AddLast(lex[i]);
									}else{
										Console.Buffer.Text += "\nERR : Expected 'AN' statement near " + s;
										return "ERR";
									}
								}
							}
						}
					}

					//this is to check is the expression is valid
					//there should be one answer if you evaluate the expression
					foreach(string s in operations){
						if(s=="SUM"||s=="DIFF"||s=="PRODUKT"||s=="QUOSHUNT"||s=="MOD"||s=="BIGGR"||s=="SMALLR"){
							checker -= 1;
						}else{
							checker += 1;
						}
					}
					if(checker==1) return EvaluateOperations();
					else{
						Console.Buffer.Text += "\nERR : Unexpected operators or operands";
						error = true;
						return "ERR";
					}
				//if it falls to the BOOLEAN classification
				}else if(OperationType(lex)=="BOOLEAN"){
					if(lex.Length == 2){
						if(lex[0]=="NOT"){		// NOT operation handling
							LexemeStore.AppendValues(lex[0],"NOT Operand");
							if(CheckLiteral(lex[1])=="TROOF"){
								LexemeStore.AppendValues(string_IT,"Boolean Literal");
								string_IT = (string_IT=="WIN") ? "FAIL" : "WIN";
								return "TROOF";
							}else{
								Console.Buffer.Text += "\nERR : Unexpected operators or operands";
								error = true;
								return "ERR";
							}
						}else{
							Console.Buffer.Text += "\nERR : Unexpected operators or operands";
							error = true;
							return "ERR";
						}
					}
					Boolean arity = false, no_mkay = false, should_an = false;
					for(int i=0;i<lex.Length;i++){
						string s = lex[i];			//adding the operations and operands to the operations stack
						if((s=="BOTH"||s=="EITHER"||s=="WON"||s=="ALL"||s=="ANY")){
							should_an = true;
							break;
						}
					}
					for(int i=lex.Length-1;i>=0;i--){
						string s = lex[i];
						if(s=="AN"||s=="OF")continue;
						if(s=="NOT"||s=="MKAY"){
							operations.AddLast(lex[i]);					//adding the operands in the operations
							continue;									//	stack
						}
						if((s=="BOTH"||s=="EITHER"||s=="WON"||s=="ALL"||s=="ANY") && lex[i+1]=="OF"){
							operations.AddLast(lex[i]);
						}else if((s=="BOTH"||s=="EITHER"||s=="WON"||s=="ALL"||s=="ANY") && lex[i+1]!="OF"){
							Console.Buffer.Text += "\nERR : Expected 'OF' statement for " + s;
							return "ERR";
						}else if(CheckLiteral(s)=="TROOF"){
							if(!should_an){
								operations.AddLast(lex[i]);
								continue;
							}
							int left = 0;								//syntax error checking
							do{
								left++;
							}while(left!=i && lex[i-left]=="NOT");
							if(i!=lex.Length-1 && i!=0){
								if(lex[i+1]=="AN"||lex[i-left]=="AN"){
									operations.AddLast(lex[i]);
								}else{
									Console.Buffer.Text += "\nERR : Expected 'AN' statement near operand ::- " + s;
									return "ERR";
								}
							}else{
								if(i==lex.Length-1){
									if(lex[i-left]=="AN"){
										operations.AddLast(lex[i]);
									}else{
										Console.Buffer.Text += "\nERR : Expected 'AN' statement near " + s;
										return "ERR";
									}
								}else if(i==0){
									if(lex[i+1]=="AN"){
										operations.AddLast(lex[i]);
									}else{
										Console.Buffer.Text += "\nERR : Expected 'AN' statement near " + s;
										return "ERR";
									}
								}
							}
						}
					}
					if(operations.First.Value!="MKAY") no_mkay = true;		//airty checking
					if(!no_mkay){
						if(operations.Last.Value!="ANY" && operations.Last.Value!="ALL"){
						Console.Buffer.Text += "\nERR : Expected opening statement for MKAY";
						return "ERR";
						}
					}
					//this is to check is the expression is valid
					//there should be one answer if you evaluate the expression
					foreach(string s in operations){
						if(s=="ALL"||s=="ANY"){
							if(no_mkay){
								Console.Buffer.Text += "\nERR : Expected closing statement for " + s + " OF";
								return "ERR";
							}else arity = true;
						}
						if(s=="NOT"||s=="ALL"||s=="ANY"||s=="MKAY"){
							continue;
						}else if(s=="BOTH"||s=="EITHER"||s=="WON"){
							checker -= 1;
						}else{
							checker += 1;
						}
					}
					if(checker==1||arity)return EvaluateBoolean(); //string_IT holds the answer
					else{
						Console.Buffer.Text += "\nERR : Unexpected operators or operands";
						error = true;
						return "ERR";
					}
				}else if(OperationType(lex)=="RELATIONAL"){
					//if it falls to the RELATIONAL classification
					//use for evaluating comparison operations that involves equality
					if (lex [0] == "BOTH" && lex [1] == "SAEM" && lex [3] == "AN" && lex.Length==5) {
						string type1 = CheckLiteral (" "+ lex [2]+ " "); //returns type of operands and stores the value to string_IT
						var tempValue1 = string_IT;
						var tempValue1Type  = type1;
						string type2 = CheckLiteral (" "+lex [4]+" "); //returns type of operands and stores the value to string_IT
						var tempValue2 = string_IT;
						var tempValue2Type = type2;				
						if (tempValue1 == "" || tempValue2 == "") {
							return "ERR";
						}
						//immediate FAIL if the comparison operands are not equal in data type and in value
						if ((tempValue1 != tempValue2) || (tempValue1Type != tempValue2Type)) {
							string_IT = "FAIL";
							return"TROOF";
						} else if (tempValue1 == tempValue2)  {
							string_IT = "WIN";
							return "TROOF";
						} else
							return "ERR";
					}
					//evaluates if comparison operation is DIFFRINT
					else if (lex [0] == "DIFFRINT" && lex [2] == "AN" && lex.Length==4) {
						string type1 = CheckLiteral (" "+ lex [1]+" ");
						var tempValue1 = string_IT;
						var tempValue1Type  = type1;
						string type2 = CheckLiteral (" "+lex [3]+" ");
						var tempValue2 = string_IT;
						var tempValue2Type = type2;
						if (tempValue1 == "" || tempValue2 == "") {
							return "ERR";
						}
						if (tempValue1 == tempValue2) {
							string_IT = "FAIL";
							return "TROOF";
						} else if ((tempValue1 != tempValue2) || (tempValue1Type != tempValue2Type)) {
							string_IT = "WIN";
							return "TROOF";
						} else return "ERR";
					}
					//evaluates if comparison operation is BOTH SAEM
					else if(lex[0] == "BOTH" && lex[1] == "SAEM" && lex [3] =="AN" && (lex [4] == "BIGGR" || lex[4] == "SMALLR") && lex [5] == "OF" && lex[7] == "AN"){
						if(lex [2] != lex [6]){
							Console.Buffer.Text += "\nERR : Expected operand("+lex[2]+") and ("+lex[6]+") should be equal";
							error = true;
							return "ERR";
						}
						string type1 = CheckLiteral (" "+lex [6]+" ");
						var tempValue1 = string_IT;
						var tempValue1Type  = type1;
						string type2 = CheckLiteral (" "+lex [8]+" ");
						var tempValue2 = string_IT;
						var tempValue2Type = type2;
						//can only check greater than or lesser than of type NUMBR or NUMBAR
						if((tempValue1Type == "NUMBR" || tempValue1Type =="NUMBAR") && (tempValue2Type == "NUMBR" || tempValue2Type =="NUMBAR")){
							double dtempValue1 = double.Parse(tempValue1);
							double dtempValue2 = double.Parse(tempValue2);
							if (type1 == type2) {
								if (lex [4] == "BIGGR") {
									if (dtempValue1 >= dtempValue2) {
										string_IT = "WIN";
										return "TROOF";
									} else if (dtempValue1 <= dtempValue2) {
										string_IT = "FAIL";
										return "TROOF";
									}
								} else if (lex [4] == "SMALLR") {
									if (dtempValue1 <= dtempValue2) {
										string_IT = "WIN";
										return "TROOF";
									} else if (dtempValue1 >= dtempValue2) {
										string_IT = "FAIL";
										return "TROOF";
									}
								}
							} else {
								string_IT = "FAIL";
								return "TROOF";
							}
						}

					}
					//use for evaluating comparison operations that involves greater than or lesser than
					else if(lex[0] == "DIFFRINT" && lex [2] =="AN" && (lex [3] == "BIGGR" || lex[3] == "SMALLR") && lex [4] == "OF" && lex[6] == "AN"){
						if(lex [1] != lex [5]){
							Console.Buffer.Text += "\nERR : Expected operand("+lex[1]+") and ("+lex[5]+") should be equal";
							error = true;
							return "ERR";
						}
						string type1 = CheckLiteral (" "+ lex [5]+" ");
						var tempValue1 = string_IT;
						var tempValue1Type  = type1;
						string type2 = CheckLiteral (" "+lex [7]+" ");
						var tempValue2 = string_IT;
						var tempValue2Type = type2;
						//if the operands are of type NUMBR or NUMBAR
						if((tempValue1Type == "NUMBR" || tempValue1Type =="NUMBAR") && (tempValue2Type == "NUMBR" || tempValue2Type =="NUMBAR")){
							//converts them to double
							double dtempValue1 = Convert.ToDouble (tempValue1);
							double dtempValue2 = Convert.ToDouble (tempValue2);
							//actual evaluation of the operands according to comparison operations
							if (type1 == type2) {
								if (lex [3] == "BIGGR") {
									if (dtempValue1 < dtempValue2) {
										string_IT = "WIN";
										return "TROOF";
									} else if (dtempValue1 > dtempValue2) {
										string_IT = "FAIL";
										return "TROOF";
									}
								} else if (lex [3] == "SMALLR") {
									if (dtempValue1 > dtempValue2) {
										string_IT = "WIN";
										return "TROOF";
									} else if (dtempValue1 < dtempValue2) {
										string_IT = "FAIL";
										return "TROOF";
									}
								}
							} else {
								string_IT = "FAIL";
								return "TROOF";
							}
						}

					}else{
						Console.Buffer.Text += "\nERR : Unexpected operators or operands";
						string_IT = "";
						error = true;
						return "ERR";
					}
					string_IT = "";
					return "ERR";
				}else{
					Console.Buffer.Text += "\nERR : Unexpected operators or operands";
					error = true;
					return "ERR";
				}
			}else{ // if the parameter is only one word and not a LOLCODE literal
					// it should be a variable
				if(variables.ContainsKey(lex[0])){
					string_IT =  variables[lex[0]][1];
					return variables[lex[0]][0];
				}else{
					string_IT = "";
					Console.Buffer.Text += "\nERR : Undeclared Variable ::-" + (lex[0]);
					error = true;
					return "ERR";
				}
			}		
		}
	}

	/*====================================================================================
	* 
	* 	OnExecuteBtnClicked ()
	* 		-function that executes when the execute button was clicked
	* 		-get the text in the text editor and interpret it
	* 
	*====================================================================================*/

	protected void OnExecuteBtnClicked (object sender, EventArgs e)
	{
		ShowConsole(); 											//shows the console
		Boolean begin = false, end = false;						//used for checking the beginning and end statement of a lolcode
		Boolean comment = false;								//used for checking if there is/are any comment/s
		Boolean orly = false, condition_value = false, ifclause = false, elseclause = false;	//booleans for if-else statements
		Boolean wtf = false, case_value = false, omg = false, omgwtf = false, gtfo = false;		//booleans for switch statements
		string hidden_temp = ""; 
		error = false;
		string_IT = "";
		hidden_IT = "";									//*
		IT = 0;											//*
		boolean_IT = false;								//*
		MainWindow.type = ""; 										//*
		LexemeStore.Clear ();							//*		Resetting all the stacks, lists, variables,
		SymbolStore.Clear ();							//*			treeview and textview
		variables.Clear ();								//*
		operations.Clear();								//*
		operands.Clear();								//*
		stack.Clear ();
		stack.AddLast ("NO_DATA");
		Console.Buffer.Text = "";

		string[] lines = TextEditor.Buffer.Text.Split (new string[] { "\r\n", "\n" }, StringSplitOptions.None);

		if ((lines [0] == "" && lines.Length == 1) || TextEditor.Buffer.Text.Trim()=="") {	//if the text editor is empty
			Console.Buffer.Text += "\nERR : Interpreting an Empty Code";
			return;
		}
		for (int linecount=0; linecount<lines.Length; linecount++) {
			if (error) {
				end = true;						//if an error was encountered
				break;
			}
			if (lines [linecount] == "") {		//ignore when it is blank
				continue;
			}
			if (end) {							//if there are statements after the KTHXBYE
				Console.Buffer.Text += "\nERR : Invalid statements after the end of code at line ::- " + (linecount + 1);
				break;
			}
			string text = lines [linecount].Trim ();

			//contains the regex for LOLCODE 
			MatchCollection match = Regex.Matches (text, 
           		@"^\s*HAI |^\s*KTHXBYE\s*$|^\s*I HAS A |^\s*VISIBLE |^\s*SUM OF |^\s*DIFF OF |^\s*PRODUKT OF |^\s*QUOSHUNT OF |
				|^\s*MOD OF |^\s*BIGGR OF |^\s*SMALLR OF |^\s*GIMMEH |^\s*BOTH SAEM |^\s*DIFFRINT |^\s*BOTH OF |
				|^\s*EITHER OF |^\s*WON OF |^\s*NOT |^\s*ALL OF |^\s*ANY OF |\s*BTW|^\s*O RLY[?]|^\s*WTF[?]|^\s*OMGWTF|^\s*OMG|
				|^\s*YA RLY|^\s*NO WAI|^\s*OIC|^\s*GTFO|^\s*OBTW|^\s*SMOOSH |^\s*AN BIGGR OF|^\s+AN SMALLR OF|\s*R\s*|\s*TLDR|^.*$"
			);
			foreach (Match m in match) {
				/*================================ // for checking pairs (e.g) OBTW - TLDR  IF - OIC================================*/
				//uses stack and the stack concept detects the keywords that is involved in pair statements and checks if it has an
				//		appropriate start and finish in pairing

				if (stack.Last.Value != "NO_DATA") {  
					string[] lex = text.Split (new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (stack.Last.Value == "OBTW") { //start of Multiline comment
						comment = true; //checks if the stack's last value is OBTW and makes the comment "flag" true	
						if (lex.Length == 1) {
							if (m.Value == "TLDR") { //end of multiline comment
								LexemeStore.AppendValues (m.Value, "End of MultiLine Comment");
								stack.RemoveLast (); //if it detects the TLDR keyword it will remove the OBTW in top of stack
								comment = false; //makes the comment "flag" false, for detecting errors
								break;	
							}
						} else if (m.Value == "TLDR") { //if it detects the TLDR keyword without the OBTW
							Console.Buffer.Text += "\nERR : Expected no other statements beside TLDR at line ::- " + (linecount + 1);
							error = true;
							break;
						}
					} else if (stack.Last.Value == "O RLY?") { //start of condition statements
						if (lex.Length!=1 && m.Value == "OIC") { //assumption is no other statements beside OIC
							Console.Buffer.Text += "\nERR : Expected no other statements beside "+m.Value+" at line ::- " + (linecount + 1);
							error = true;
							break;
						}else if(lex.Length!=2 && (m.Value=="YA RLY" || m.Value=="NO WAI")){
							//assumption is no other statements beside YA RLY and NO WAI
							Console.Buffer.Text += "\nERR : Expected no other statements beside "+m.Value+" at line ::- " + (linecount + 1);
							error = true;
							break;
						}else if (m.Value == "OIC") { //end of condition statement
							LexemeStore.AppendValues (m.Value, "End of Condition Statement"); //stores in lexeme tree view
							stack.RemoveLast (); //if it detects the OIC keyword it will remove the O RLY? in top of stack
							elseclause = false; //YA RLY and NO WAI must be in the scope of O RLY? and OIC
							ifclause = false;	// ifclause "flag" and elseclause "flag" catches that error
							orly = false; // assumption is no nested condition statement
							condition_value = false;
							break;	
						} else if (m.Value == "YA RLY") { //start of if statement
							ifclause = true; //ifclause "flag" is true if it detects YA RLY
							LexemeStore.AppendValues (m.Value, "If Statement");
							stack.AddLast (m.Value); //stores YA RLY in top of stack
							break;
						}else if (m.Value == "NO WAI") { 
							Console.Buffer.Text += "\nERR : Expected YA RLY statement for NO WAI at line ::- " + (linecount + 1);
							error = true;
							break;
						} else if(m.Value != "BTW" && m.Value != "OBTW" && m.Value != " BTW"){
							Console.Buffer.Text += "\nERR : Unexpected statements at line ::- " + (linecount + 1);
							error = true;
							break;
						}

					} else if (stack.Last.Value == "YA RLY") { //start of if statement
						if (lex.Length!=1 && m.Value == "OIC") {
							Console.Buffer.Text += "\nERR : Expected no other statements beside "+m.Value+" at line ::- " + (linecount + 1);
							error = true;
							break;
						}else if(lex.Length!=2 && m.Value=="NO WAI"){
							Console.Buffer.Text += "\nERR : Expected no other statements beside "+m.Value+" at line ::- " + (linecount + 1);
							error = true;
							break;
						}
						if (m.Value == "NO WAI") { //end of if statement and start of else statement
							ifclause = false; //ends the if statement by making ifclause "flag" false
							elseclause = true; // and making the elseclause "flag" true
							LexemeStore.AppendValues (m.Value, "Else Statement"); //stores NO WAI in lexeme tree view
							stack.RemoveLast (); //removes the YA RLY in stack
							stack.AddLast(m.Value);	//NO WAI is now the top of stack
							break;
						}else if(m.Value=="OIC"){ //end of condition statement
							LexemeStore.AppendValues (m.Value, "End of Condition Statement"); //stores OIC in lexeme tree view
							stack.RemoveLast (); //removes NO WAI from top of stack
							stack.RemoveLast (); //removes O RLY? from top of stack
							elseclause = false; 
							ifclause = false;
							orly = false;
							condition_value = false;
							break;	
						}
					}else if(stack.Last.Value =="NO WAI"){ //end of if statement start of else statement
						if (lex.Length!=1 && m.Value == "OIC") {
							Console.Buffer.Text += "\nERR : Expected no other statements beside "+m.Value+" at line ::- " + (linecount + 1);
							error = true;
							break;
						}
						if(m.Value=="OIC"){ //ends the NO WAI (else) statement and O RLY?
							LexemeStore.AppendValues (m.Value, "End of Condition Statement");
							stack.RemoveLast (); //removes NO WAI from top of stack
							stack.RemoveLast (); //removes O RLY? from top of stack
							elseclause = false;
							ifclause = false;
							orly = false;
							condition_value = false;
							break;	
						}
					}else if(stack.Last.Value =="WTF?"){ //start of switch-case statement
						if (lex.Length!=1 && (m.Value == "OIC"||m.Value=="OMGWTF"||m.Value=="GTFO")) {
							Console.Buffer.Text += "\nERR : Expected no other statements beside "+m.Value+" at line ::- " + (linecount + 1);
							error = true;
							break;
						}else if(m.Value=="OIC"){ //ends of switch-case statement
							LexemeStore.AppendValues (m.Value, "End of Switch-Case Statement");
							stack.RemoveLast (); //removes WTF? from stack
							wtf = false; //wtf "flag" checks if it is in the scope of WTF?
							case_value = false;
							omg = false;
							omgwtf = false;
							gtfo = false;	
							break;
						}else if(m.Value=="OMG"){ //start of case
							LexemeStore.AppendValues (m.Value, "Start of Case");
							if(omgwtf){
								Console.Buffer.Text += "\nERR : Expected OMGWTF at the end of Swith-Case statement at line ::- " + (linecount + 1);
								error = true;
								break;
							}
							string val = "";
							string[] val_arr = text.Split (new string[] { "OMG" }, StringSplitOptions.RemoveEmptyEntries);
							if(val_arr.Length == 0){
								Console.Buffer.Text += "\nERR : Expected value for case at line ::- " + (linecount + 1);
								error = true;
								break;
							}else{
								val = val_arr[0].Trim();
							}
							if(val.Split (new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length!=1){
								Console.Buffer.Text += "\nERR : Unexpected statements '"+text+"' at line ::- " + (linecount + 1);
								error = true;
								break;
							}
							string type = CheckLiteral(" "+val+" ");
							if(type=="ERR"){
								Console.Buffer.Text += "\nERR : Unexpected Case Value in line ::- " + (linecount + 1);
								error = true;
								break;
							}else{
								if (type == "NUMBR") {
									LexemeStore.AppendValues (IT.ToString (), "Integer Literal");
								} else if (type == "NUMBAR") {
									LexemeStore.AppendValues (IT.ToString (), "Float Literal");
								} else if (type == "YARN") {
									LexemeStore.AppendValues ("\"", "String Delimiter");
									LexemeStore.AppendValues (string_IT, "String Literal");
									LexemeStore.AppendValues ("\"", "String Delimiter");
								} else if(type == "TROOF"){
									LexemeStore.AppendValues (string_IT, "Boolean Literal");
								}
							}

							if(string_IT == hidden_temp){
								case_value = true;
							}
							omg = true;
							break;
						}else if(m.Value=="GTFO"){ //breaks the case statement
							if(!omg){
								Console.Buffer.Text += "\nERR : Expected Case before GTFO at line ::- " + (linecount + 1);
								error = true;
								break;
							}
							LexemeStore.AppendValues (m.Value, "Break Statement");
							gtfo = case_value ? true : false;
							break;
						}else if(m.Value=="OMGWTF"){ //default case
							LexemeStore.AppendValues (m.Value, "Default Case");
							omg = false;
							omgwtf = true;
							break;
						}else if((!omg && !omgwtf) && m.Value != "OMG" && m.Value != "OMGWTF" && m.Value != "GTFO"){
							Console.Buffer.Text += "\nERR : Unexpected statements at line ::- " + (linecount + 1);
							error = true;
							break;
						}
					}
				} 

				if(comment) continue;
				if (m.Value == "O RLY?") { //start of condition statement
					if((hidden_IT != "WIN" && hidden_IT != "FAIL") || hidden_IT =="" || hidden_IT=="ERR"){
															//IT is an implicit variable in LOLCODE which is used in condition statement
						if(hidden_IT =="" || hidden_IT=="ERR")Console.Buffer.Text += "\nERR : Wild Card not found "; //IT must have a value of WIN or FAIL
						else Console.Buffer.Text += "\nERR : Wild Card cannot be casted to TROOF "; //IT must be of type TROOF
						Console.Buffer.Text += "\nERR : Failed to execute O RLY? statement at line ::- " + (linecount + 1);
						error = true;
						break;
					}
					string[] lex = text.Split (new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (lex.Length == 2) {
						LexemeStore.AppendValues (m.Value, "Start of Condition Statement");
						stack.AddLast (m.Value); //pushes O RLY? in stack
						orly = true; 
						condition_value = (string_IT=="WIN") ? true : false;//sets the condition_value depending on the value of string_IT
						break;
					} else {
						Console.Buffer.Text += "\nERR : Expected no other statements beside O RLY? at line ::- " + (linecount + 1);
						error = true;
						break;
					}
				} else if (m.Value == "OIC" || m.Value == "YA RLY" || m.Value == "NO WAI") {
					if(orly) continue; //continues the loop beecause it's still in O RLY? scope
					Console.Buffer.Text += "\nERR : Expected opening statement for "+m.Value+" at line ::- " + (linecount + 1); 
					error = true;
					break;
				} else if(m.Value == "WTF?"){
					if(hidden_IT =="" || hidden_IT=="ERR"){ //if hidden_IT is not of type TROOF with values WIN or FAIL throws error
						Console.Buffer.Text += "\nERR : Wild Card not found ";
						Console.Buffer.Text += "\nERR : Failed to execute WTF? statement at line ::- " + (linecount + 1);
						error = true;
						break;
					}
					string[] lex = text.Split (new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (lex.Length == 1) { //WTF? must be alone in its line
						LexemeStore.AppendValues (m.Value, "Start of Switch-Case Statement"); //stores the WTF? in lexeme tree view
						stack.AddLast(m.Value); //pushes WTF? in stack
						hidden_temp = hidden_IT;
						wtf = true; //wtf "flag" is true for the scope of WTF?
						break;
					} else {
						Console.Buffer.Text += "\nERR : Expected no other statements beside WTF? at line ::- " + (linecount + 1);
						error = true;
						break;
					}
				}else if (m.Value == "OIC" || m.Value == "OMGWTF" || m.Value == "OMG") {
					if(wtf) continue;
					Console.Buffer.Text += "\nERR : Expected opening statement for "+m.Value+" at line ::- " + (linecount + 1);
					error = true;
					break;
				}else if (m.Value == "BTW" || m.Value == " BTW") {
					LexemeStore.AppendValues ("BTW", "One Line Comment");
					break;
				} else if (m.Value == "OBTW") {
					string[] lex = text.Split (new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (lex.Length == 1) {
						LexemeStore.AppendValues (m.Value, "Start of MultiLine Comment");
						stack.AddLast (m.Value);
						break;
					} else {
						Console.Buffer.Text += "\nERR : Expected no other statements beside OBTW at line ::- " + (linecount + 1);
						error = true;
						break;
					}
				} else if (m.Value == "TLDR") {
					Console.Buffer.Text += "\nERR : Expected opening statement for TLDR at line ::- " + (linecount + 1);
					error = true;
					break;
				} else if (m.Value == "HAI " || m.Value == "HAI") { //checks if HAI is already there. throws error because of multiple opening statements
					if (begin) {
						Console.Buffer.Text += "\nERR : Multiple opening statements detected at line ::- " + (linecount + 1);
						error = true;
						break;
					}
					string[] lex = text.Split (new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (lex.Length == 1 || lex [1] == "BTW") { //checks if there is a BTW(comment)
						LexemeStore.AppendValues (m.Value, "LOLCODE Opening Statement");
						begin = true; //begin "flag" handles if there is the opening statement HAI
					} else {
							//assumption is only comments must be beside HAI statement
						Console.Buffer.Text += "\nERR : Unexpected statements at line ::- " + (linecount + 1);
						error = true;
						break;
					}
				} else if (!begin) {
					Console.Buffer.Text += "\nERR : Expected opening statement at line ::- " + (linecount + 1);
					error = true;
					break;
				} else if (m.Value == "KTHXBYE") {
					if(stack.Last.Value != "NO_DATA"){
						Console.Buffer.Text += "\nERR : Expected closing statement for ::- " + stack.Last.Value;
						error = true;
						break;
					}
					LexemeStore.AppendValues (m.Value, "LOLCODE Closing Statement");
					end = true;
				}
				//=============================================END of CHECKING PAIRS==============================================//


				else if (m.Value == "I HAS A ") { //start of variable declaration
					Boolean has_value = false;
					LexemeStore.AppendValues (m.Value, "Variable Declaration");

					string[] lex = text.Split (new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (keywordLinkedList.Contains (lex [3])) {	//check if the variable name is a LOLCODE keyword
						Console.Buffer.Text += "\nERR : Keyword '"+lex[3]+"' is used as a LOLCODE keyword";
						error = true;
						break;
					}

					if (lex.Length >= 4) {
						if (variables.ContainsKey (lex [3])) { //checks if the variable is already declared
							Console.Buffer.Text += "\nERR : Redeclaration of variable at line ::- " + (linecount + 1);
							error = true;
							break;
						}

						if (Regex.IsMatch (lex [3], @"^[A-Za-z][A-Za-z0-9/_]*$")) { //checks if the variable conforms to the naming convention
							LexemeStore.AppendValues (lex [3], "Variable Identifier");
						} else {
							Console.Buffer.Text += "\nERR : Invalid Variable Name at line ::- " + (linecount + 1) + " (" + lex [3] + ")";
							error = true;
							break;
						}

						if (lex.Length > 4) {
							if (lex [4] == "ITZ") { //initializes the declared variable with a specified value
								LexemeStore.AppendValues (lex [4], "Variable Initialization");
								if (lex.Length == 5) {
									Console.Buffer.Text += "\nERR : Failed to assign a value to a variable at line ::- " + (linecount + 1);
									error = true;
									break;
								} else {
									string new_string = "";
									has_value = true;
									string[] temp = text.Split (new char[] { '\"' }, StringSplitOptions.RemoveEmptyEntries);
									if (lex.Length > 6) {
										if (lex [5] == "SMOOSH") { //if value of initialize variable is to be concatenated
											new_string = text.Split (new string[] { " ITZ " }, StringSplitOptions.RemoveEmptyEntries) [1];
											string[] btwcheck = new_string.Split (new string[] { "BTW" }, StringSplitOptions.None);
											if (btwcheck.Length > 1) { //checks if there is a comment
												LexemeStore.AppendValues ("BTW", "One Line Comment");
												new_string = btwcheck [0];
											}
										} else if (temp.Length == 2) {
											new_string = " " + text.Split (new string[] { " ITZ " }, StringSplitOptions.RemoveEmptyEntries) [1] + " ";
										} else {
											for (int x = 5; x < lex.Length; x++) {
												if (lex [x] == "BTW") {
													LexemeStore.AppendValues (lex [x], "One Line Comment");
													new_string = " " + text.Split (new string[] { " ITZ " }, StringSplitOptions.RemoveEmptyEntries) [1] + " ";
													new_string = " " + new_string.Split (new string[] { "BTW" }, StringSplitOptions.None) [0].Trim () + " ";
													break;
												} else {
													new_string += " " + lex [x] + " ";
												}
											}
										}
									} else {
										new_string = " " + text.Split (new string[] { " ITZ " }, StringSplitOptions.RemoveEmptyEntries) [1] + " ";
									}
									string type = CheckLiteral (new_string); //get the value of the literal and assign to the variable
									if (type == "ERR") {
										Console.Buffer.Text += "\nERR : Failed to assign a value to a variable at line ::- " + (linecount + 1);
										error = true;
										break;
									} else {
										if (orly) {
											Console.Buffer.Text += "\nERR : Cannot declare a variable inside an O RLY? statement";
											error = true;
											break;
										}
										if (wtf) {
											Console.Buffer.Text += "\nERR : Cannot declare a variable inside an WTF? statement";
											error = true;
											break;
										}
										if (type == "NUMBR") {
												//checks if type is NUMBR for storing in lexeme tree  view
											LexemeStore.AppendValues (IT.ToString (), "Integer Literal");
											SymbolStore.AppendValues (lex [3], IT.ToString ());
											variables.Add (lex [3], new string[] { type, IT.ToString () });
										} else if (type == "NUMBAR") {
											//checks if type is NUMBAR for storing in lexeme tree  view
											LexemeStore.AppendValues (IT.ToString (), "Float Literal");
											SymbolStore.AppendValues (lex [3], IT.ToString ());
											variables.Add (lex [3], new string[] { type, IT.ToString () });
										} else if (type == "NOOB") {
											//checks if type is NOOB for storing in lexeme tree  view
											SymbolStore.AppendValues (lex [3], "-");
											variables.Add (lex [3], new string[] { type, null });
										} else if (type == "YARN") {
											//checks if type is YARN for storing in lexeme tree  view
											LexemeStore.AppendValues ("\"", "String Delimiter"); //stores the string delimiter
											LexemeStore.AppendValues (string_IT, "String Literal"); //stores the actual string
											LexemeStore.AppendValues ("\"", "String Delimiter");
											SymbolStore.AppendValues (lex [3], string_IT);
											variables.Add (lex [3], new string[] { type, string_IT });
										} else if (type == "TROOF") {
											LexemeStore.AppendValues (string_IT, "Boolean Literal");
											SymbolStore.AppendValues (lex [3], string_IT);
											variables.Add (lex [3], new string[] { type, string_IT });
										}
									}
								}
							} else if (lex [4] == "BTW") {
								has_value = false;
								LexemeStore.AppendValues (lex [4], "One Line Comment");
							} else {
								Console.Buffer.Text += "\nERR : Expected ITZ statement at line ::- " + (linecount + 1);
								Console.Buffer.Text += "\nERR : Failed to declare a variable at line ::- " + (linecount + 1);
								error = true;
								break;
							}
						}

						if (!has_value) {		// if the variable was declared without a value
							SymbolStore.AppendValues (lex [3], "-");
							variables.Add (lex [3], new string[] { "NOOB", null });
							break;
						}
					} else {
						Console.Buffer.Text += "\nERR : Variable name not specified at line ::- " + (linecount + 1);
						error = true;
						break;
					}
				break;
				} else if (m.Value == "VISIBLE ") { //print operator 
					Boolean newline = false;
					LexemeStore.AppendValues (m.Value, "Print Operator");
					string[] lex = text.Split (new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

					if(lex.Length==1){
						Console.Buffer.Text += "\nERR : Expected statements for VISIBLE at line ::- " + (linecount + 1);
						error = true;
						break;
					}
					if(text.EndsWith("!")){				//new line handler
						newline = true;
						text = text.TrimEnd('!');
					}
					string new_string = text.Split (new string[] { "VISIBLE " }, StringSplitOptions.RemoveEmptyEntries)[0];
					string[] btwcheck = new_string.Split (new string[] { "BTW" }, StringSplitOptions.None);
					if(btwcheck.Length>1){
						LexemeStore.AppendValues ("BTW", "One Line Comment");
						new_string = btwcheck[0].Trim();
					}
					string type = CheckLiteral(new_string+" "); //get the value of the literal to be printed and the type
																// for lexeme checking
					if(type=="NUMBR"){
						//checks if type is NUMBR for storing in lexeme tree  view
						LexemeStore.AppendValues (string_IT, "Integer Literal");
					}else if(type=="NUMBAR"){
						//checks if type is NUMBAR for storing in lexeme tree  view
						LexemeStore.AppendValues (string_IT, "Float Literal");
					}else if(type=="TROOF"){
						//checks if type is TROOF for storing in lexeme tree  view
						LexemeStore.AppendValues (string_IT, "Boolean Literal");
					}else if(type=="YARN"){
						//checks if type is YARN for storing in lexeme tree  view
						LexemeStore.AppendValues ("\"", "String Delimiter");
						LexemeStore.AppendValues (string_IT, "String Literal");
						LexemeStore.AppendValues ("\"", "String Delimiter");
					}else if(type=="NOOB"){
						//checks if type is NOOB for storing in lexeme tree  view
						Console.Buffer.Text += "\nERR : Printing an uninitialized variable at line ::- " + (linecount + 1);
						error = true;
						break;
					}
					if(!error){
						if(orly && !((ifclause && condition_value)||(elseclause && !condition_value))) break;
						if (gtfo || (wtf && omg && !case_value)) break; 
						if(newline){
							Console.Buffer.Text += string_IT + "\n"; // printing the value of string_IT in the console with new line
						}else{
							Console.Buffer.Text += string_IT;		// with out new line
						}
					}
					break;
				} else if (m.Value == "GIMMEH ") { //for scanning user input
					LexemeStore.AppendValues (m.Value, "Input Operator");
					string new_string = text.Split (new string[] { "GIMMEH " }, StringSplitOptions.RemoveEmptyEntries)[0];
					string[] btwcheck = new_string.Split (new string[] { "BTW" }, StringSplitOptions.None);
					if(btwcheck.Length>1){
						LexemeStore.AppendValues ("BTW", "One Line Comment");
						new_string = btwcheck[0].Trim();
					}
					if(new_string.Length!=0 && new_string!=""){
						if (variables.ContainsKey (new_string)) { 	//checks if the variable was previously declared
								if(orly && !((ifclause && condition_value)||(elseclause && !condition_value))) break;
								if (gtfo || (wtf && omg && !case_value)) break; 
							LexemeStore.AppendValues (new_string, "Variable Identifier");
								Proto.ScanDialog x = new Proto.ScanDialog (); //builds the scan dialog for getting user input
								ExecuteBtn.Hide ();
								x.Run ();
								ExecuteBtn.Show ();
								if(type=="NUMBR"||type=="NUMBAR"){ 
									variables [new_string] [1] = IT.ToString(); 	//changing the value of the variable and 
									variables [new_string] [0] = type;				// its type
								}else if(type=="TROOF"){
									variables [new_string] [1] = string_IT;
									variables [new_string] [0] = type;
								}else if(type=="YARN"){
									variables [new_string] [1] = string_IT;
									variables [new_string] [0] = type;
								}else{
									Console.Buffer.Text += "\nERR : Error in scanning ::- " + new_string;
									error = true;
									break;
								}
								break;
							} else {												// variable not found
								Console.Buffer.Text += "\nERR : Undeclared Variable ::- " + new_string;
								error = true;
								break;							
							}
					} else {
						Console.Buffer.Text += "\nERR : Variable not specified at line ::- " + (linecount + 1);
						error = true;
						break;		
					}
				} else if (Regex.IsMatch(text, @"^.*\s+R\s+.*$")) { // for assignment statement

					string[] lex = text.Split (new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

					if(lex.Length==2){
						Console.Buffer.Text += "\nERR : Failed to assign a value to a variable at line ::- " + (linecount + 1);
						error = true;
						break;
					}else if(lex.Length >= 3){
						if(variables.ContainsKey(lex [0])){
							LexemeStore.AppendValues (lex [0], "Variable Identifier");
							if(lex[1]=="R"){ // for assigning operation R
								LexemeStore.AppendValues (lex[1], "Assignment Operator");
								string[] temp = text.Split (new char[] { '\"' }, StringSplitOptions.RemoveEmptyEntries);
								string new_string = "";
								if (lex.Length > 3) {
									if(lex[2]=="SMOOSH"){ // if assigning value has concatenation operation
										new_string = text.Split (new string[] { " R " }, StringSplitOptions.RemoveEmptyEntries)[1];
										string[] btwcheck = new_string.Split (new string[] { "BTW" }, StringSplitOptions.None);
										if(btwcheck.Length>1){ //checks for comment
											LexemeStore.AppendValues ("BTW", "One Line Comment");
											new_string = btwcheck[0];
										}
									}else if(temp.Length==2){
										new_string = " "+ text.Split (new string[] { " R " }, StringSplitOptions.RemoveEmptyEntries)[1]+" ";
									}else{
										for (int x=2; x<lex.Length; x++) {
											if (lex [x] == "BTW") {
												LexemeStore.AppendValues (lex [x], "One Line Comment");
												new_string = " "+ text.Split (new string[] { " R " }, StringSplitOptions.RemoveEmptyEntries)[1]+" ";
												new_string = " "+new_string.Split (new string[] { "BTW" }, StringSplitOptions.None)[0].Trim()+" ";
												break;
											} else {
												new_string += " " + lex [x] + " ";
											}
										}
									}
								} else {
									new_string = " " +text.Split (new string[] { " R " }, StringSplitOptions.RemoveEmptyEntries)[1]+ " ";
								}
								// the code above just get the expression or literal to be assign to the variable
								string type = CheckLiteral (new_string); 	// the value is not in string_IT

								if(orly && !((ifclause && condition_value)||(elseclause && !condition_value))) break; 	// if handler
								if (gtfo || (wtf && omg && !case_value)) break; 										// switch handler

								if (type == "NUMBR") {
									LexemeStore.AppendValues (IT.ToString (), "Integer Literal");		//assigning values and changing its
									variables [lex [0]] [1] = IT.ToString ();							// type
									variables [lex [0]] [0] = type;
								} else if (type == "NUMBAR") {
									LexemeStore.AppendValues (IT.ToString (), "Float Literal");
									variables [lex [0]] [1] = IT.ToString ();
									variables [lex [0]] [0] = type;
								} else if (type == "NOOB") {
									Console.Buffer.Text += "\nERR : Assigning an uninitialized variable at line ::- " + (linecount + 1);
									error = true;
									break;
								} else if (type == "YARN") {
									LexemeStore.AppendValues ("\"", "String Delimiter");
									LexemeStore.AppendValues (string_IT, "String Literal");
									LexemeStore.AppendValues ("\"", "String Delimiter");
									variables [lex [0]] [1] = string_IT;
									variables [lex [0]] [0] = type;
								} else if (type == "TROOF") {
									LexemeStore.AppendValues (string_IT, "Boolean Literal");
									variables [lex [0]] [1] = string_IT;
									variables [lex [0]] [0] = type;
								} else if (type == "ERR") {
									Console.Buffer.Text += "\nERR : Failed to assign a value to a variable at line ::- " + (linecount + 1);
									error = true;
									break;
								}
								
							}else{
								Console.Buffer.Text += "\nERR : Unexpected statement at line ::- " + (linecount + 1);
								error = true;
								break;
							}
						}else{
							Console.Buffer.Text += "\nERR :  Undeclared variable "+lex[0]+" at line ::- " + (linecount + 1);
							error = true;
							break;
						}
					}
				// check if a stand alone expression
				}else if(m.Value == "SUM OF " || m.Value == "DIFF OF " || m.Value == "PRODUKT OF " || m.Value == "QUOSHUNT OF " ||
				         	m.Value == "MOD OF " || m.Value == "BIGGR OF " || m.Value == "SMALLR OF " || m.Value == "SMOOSH "||
				         	m.Value == "BOTH SAEM " || m.Value=="DIFFRINT "|| m.Value == "BOTH OF " || m.Value == "EITHER OF " ||
				         	m.Value == "WON OF " || m.Value == "NOT " || m.Value == "ALL OF " || m.Value == "ANY OF "){
					string new_string = text;
					string[] btwcheck = text.Split (new string[] { "BTW" }, StringSplitOptions.None);
					if(btwcheck.Length>1){	
						LexemeStore.AppendValues ("BTW", "One Line Comment");		//check for BTW
						new_string = btwcheck[0].Trim();
					}
					CheckLiteral (new_string);
					if(orly && !((ifclause && condition_value)||(elseclause && !condition_value))){break;} 	//if handler
					if (gtfo || (wtf && omg && !case_value)) break; 										//switch handler
					hidden_IT = string_IT; 	//hidden_IT is the IT of LOLCODE, it catches stand alone statements
					break;
				} else {
					string new_string = text;
					string[] btwcheck = text.Split (new string[] { "BTW" }, StringSplitOptions.None);
					if(btwcheck.Length>1){
						LexemeStore.AppendValues ("BTW", "One Line Comment");
						new_string = btwcheck[0].Trim();
					}
					string type = CheckLiteral (" "+new_string+" ");	//check if a stand alone literal

					if(type=="ERR"){
						Console.Buffer.Text += "\nERR : Unexpected statement " + m.Value + " at line ::- " + (linecount + 1);
						error = true;
						break;
					}

					if(orly && !((ifclause && condition_value)||(elseclause && !condition_value))){break;}	//if handler
					if (gtfo || (wtf && omg && !case_value)) break; 										//switch handler
					hidden_IT = string_IT;
					break;
				}
			}
		}	
		if (!end) {																				//if finished reading but dont 
			Console.Buffer.Text += "\nERR : Expected closing statement at the end";				//encounter the KTHXBYE statement
		} else if(!error) {
			SymbolStore.Clear ();
			SymbolStore.AppendValues ("IT",hidden_IT);											//put all the variables in the 
			foreach (string key in variables.Keys) {											// symbol table
				if(variables[key][0]=="NOOB") SymbolStore.AppendValues (key,"-");
				else SymbolStore.AppendValues (key,variables[key][1]);
			}
		}
	}

}

//========================================================================END OF CODE=============================================================================//