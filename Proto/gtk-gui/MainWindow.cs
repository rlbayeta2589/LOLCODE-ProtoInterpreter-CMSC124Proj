
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.UIManager UIManager;
	
	private global::Gtk.Action CODEAction;
	
	private global::Gtk.Action AddFileAction;
	
	private global::Gtk.Action CODEAction1;
	
	private global::Gtk.Action AddFileAction1;
	
	private global::Gtk.Action CONSOLEAction;
	
	private global::Gtk.ToggleAction AutoHideAction;
	
	private global::Gtk.VBox vbox2;
	
	private global::Gtk.MenuBar menubar1;
	
	private global::Gtk.HBox UpperWindow;
	
	private global::Gtk.VBox vbox3;
	
	private global::Gtk.ScrolledWindow GtkScrolledWindow;
	
	private global::Gtk.TextView TextEditor;
	
	private global::Gtk.VBox vbox4;
	
	private global::Gtk.Entry entry3;
	
	private global::Gtk.ScrolledWindow GtkScrolledWindow2;
	
	private global::Gtk.TreeView LexTree;
	
	private global::Gtk.Entry entry4;
	
	private global::Gtk.ScrolledWindow GtkScrolledWindow1;
	
	private global::Gtk.TreeView SymbTree;
	
	private global::Gtk.Button ExecuteBtn;
	
	private global::Gtk.HBox hbox2;
	
	private global::Gtk.Label ConsoleLabel;
	
	private global::Gtk.HBox ConsoleWindow;
	
	private global::Gtk.ScrolledWindow Window2;
	
	private global::Gtk.TextView Console;

	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.UIManager = new global::Gtk.UIManager ();
		global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
		this.CODEAction = new global::Gtk.Action ("CODEAction", global::Mono.Unix.Catalog.GetString ("CODE"), null, null);
		this.CODEAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("CODE");
		w1.Add (this.CODEAction, null);
		this.AddFileAction = new global::Gtk.Action ("AddFileAction", global::Mono.Unix.Catalog.GetString ("Add File"), null, null);
		this.AddFileAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Add File");
		w1.Add (this.AddFileAction, null);
		this.CODEAction1 = new global::Gtk.Action ("CODEAction1", global::Mono.Unix.Catalog.GetString ("CODE"), null, null);
		this.CODEAction1.ShortLabel = global::Mono.Unix.Catalog.GetString ("CODE");
		w1.Add (this.CODEAction1, null);
		this.AddFileAction1 = new global::Gtk.Action ("AddFileAction1", global::Mono.Unix.Catalog.GetString ("Add File"), null, null);
		this.AddFileAction1.ShortLabel = global::Mono.Unix.Catalog.GetString ("Add File");
		w1.Add (this.AddFileAction1, null);
		this.CONSOLEAction = new global::Gtk.Action ("CONSOLEAction", global::Mono.Unix.Catalog.GetString ("CONSOLE"), null, null);
		this.CONSOLEAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("CONSOLE");
		w1.Add (this.CONSOLEAction, null);
		this.AutoHideAction = new global::Gtk.ToggleAction ("AutoHideAction", global::Mono.Unix.Catalog.GetString ("AutoHide"), null, null);
		this.AutoHideAction.Active = true;
		this.AutoHideAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("AutoHide");
		w1.Add (this.AutoHideAction, null);
		this.UIManager.InsertActionGroup (w1, 0);
		this.AddAccelGroup (this.UIManager.AccelGroup);
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("Team Seven Interpreter");
		this.Icon = global::Stetic.IconLoader.LoadIcon (this, "gtk-spell-check", global::Gtk.IconSize.Menu);
		this.WindowPosition = ((global::Gtk.WindowPosition)(3));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox2 = new global::Gtk.VBox ();
		this.vbox2.Name = "vbox2";
		this.vbox2.Spacing = 6;
		// Container child vbox2.Gtk.Box+BoxChild
		this.UIManager.AddUiFromString (@"<ui><menubar name='menubar1'><menu name='CODEAction1' action='CODEAction1'><menuitem name='AddFileAction1' action='AddFileAction1'/></menu><menu name='CONSOLEAction' action='CONSOLEAction'><menuitem name='AutoHideAction' action='AutoHideAction'/><separator/></menu></menubar></ui>");
		this.menubar1 = ((global::Gtk.MenuBar)(this.UIManager.GetWidget ("/menubar1")));
		this.menubar1.Name = "menubar1";
		this.vbox2.Add (this.menubar1);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.menubar1]));
		w2.Position = 0;
		w2.Expand = false;
		w2.Fill = false;
		// Container child vbox2.Gtk.Box+BoxChild
		this.UpperWindow = new global::Gtk.HBox ();
		this.UpperWindow.Name = "UpperWindow";
		this.UpperWindow.Spacing = 6;
		// Container child UpperWindow.Gtk.Box+BoxChild
		this.vbox3 = new global::Gtk.VBox ();
		this.vbox3.Name = "vbox3";
		this.vbox3.Spacing = 6;
		// Container child vbox3.Gtk.Box+BoxChild
		this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow.Name = "GtkScrolledWindow";
		this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
		this.TextEditor = new global::Gtk.TextView ();
		this.TextEditor.CanFocus = true;
		this.TextEditor.Name = "TextEditor";
		this.TextEditor.PixelsAboveLines = 5;
		this.TextEditor.LeftMargin = 10;
		this.TextEditor.RightMargin = 10;
		this.GtkScrolledWindow.Add (this.TextEditor);
		this.vbox3.Add (this.GtkScrolledWindow);
		global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.GtkScrolledWindow]));
		w4.Position = 0;
		this.UpperWindow.Add (this.vbox3);
		global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.UpperWindow [this.vbox3]));
		w5.Position = 0;
		// Container child UpperWindow.Gtk.Box+BoxChild
		this.vbox4 = new global::Gtk.VBox ();
		this.vbox4.Name = "vbox4";
		this.vbox4.Spacing = 6;
		// Container child vbox4.Gtk.Box+BoxChild
		this.entry3 = new global::Gtk.Entry ();
		this.entry3.WidthRequest = 10;
		this.entry3.CanFocus = true;
		this.entry3.Name = "entry3";
		this.entry3.Text = global::Mono.Unix.Catalog.GetString ("LEXEMES");
		this.entry3.IsEditable = false;
		this.entry3.InvisibleChar = '•';
		this.vbox4.Add (this.entry3);
		global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.entry3]));
		w6.Position = 0;
		w6.Expand = false;
		w6.Fill = false;
		// Container child vbox4.Gtk.Box+BoxChild
		this.GtkScrolledWindow2 = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow2.Name = "GtkScrolledWindow2";
		this.GtkScrolledWindow2.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow2.Gtk.Container+ContainerChild
		this.LexTree = new global::Gtk.TreeView ();
		this.LexTree.CanFocus = true;
		this.LexTree.Name = "LexTree";
		this.GtkScrolledWindow2.Add (this.LexTree);
		this.vbox4.Add (this.GtkScrolledWindow2);
		global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.GtkScrolledWindow2]));
		w8.Position = 1;
		// Container child vbox4.Gtk.Box+BoxChild
		this.entry4 = new global::Gtk.Entry ();
		this.entry4.WidthRequest = 10;
		this.entry4.Name = "entry4";
		this.entry4.Text = global::Mono.Unix.Catalog.GetString ("SYMBOL TABLE");
		this.entry4.IsEditable = false;
		this.entry4.InvisibleChar = '•';
		this.vbox4.Add (this.entry4);
		global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.entry4]));
		w9.Position = 2;
		w9.Expand = false;
		w9.Fill = false;
		// Container child vbox4.Gtk.Box+BoxChild
		this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
		this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
		this.SymbTree = new global::Gtk.TreeView ();
		this.SymbTree.CanFocus = true;
		this.SymbTree.Name = "SymbTree";
		this.GtkScrolledWindow1.Add (this.SymbTree);
		this.vbox4.Add (this.GtkScrolledWindow1);
		global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.GtkScrolledWindow1]));
		w11.Position = 3;
		this.UpperWindow.Add (this.vbox4);
		global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.UpperWindow [this.vbox4]));
		w12.Position = 1;
		this.vbox2.Add (this.UpperWindow);
		global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.UpperWindow]));
		w13.Position = 1;
		// Container child vbox2.Gtk.Box+BoxChild
		this.ExecuteBtn = new global::Gtk.Button ();
		this.ExecuteBtn.CanFocus = true;
		this.ExecuteBtn.Name = "ExecuteBtn";
		this.ExecuteBtn.UseUnderline = true;
		this.ExecuteBtn.Label = "EXECUTE";
		this.vbox2.Add (this.ExecuteBtn);
		global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.ExecuteBtn]));
		w14.Position = 2;
		w14.Expand = false;
		w14.Fill = false;
		// Container child vbox2.Gtk.Box+BoxChild
		this.hbox2 = new global::Gtk.HBox ();
		this.hbox2.Name = "hbox2";
		this.hbox2.Spacing = 6;
		// Container child hbox2.Gtk.Box+BoxChild
		this.ConsoleLabel = new global::Gtk.Label ();
		this.ConsoleLabel.Name = "ConsoleLabel";
		this.ConsoleLabel.Xpad = 10;
		this.ConsoleLabel.Xalign = 0F;
		this.ConsoleLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("Console");
		this.hbox2.Add (this.ConsoleLabel);
		global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.ConsoleLabel]));
		w15.Position = 0;
		w15.Expand = false;
		w15.Fill = false;
		this.vbox2.Add (this.hbox2);
		global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox2]));
		w16.Position = 3;
		w16.Expand = false;
		w16.Fill = false;
		// Container child vbox2.Gtk.Box+BoxChild
		this.ConsoleWindow = new global::Gtk.HBox ();
		this.ConsoleWindow.Name = "ConsoleWindow";
		this.ConsoleWindow.Spacing = 6;
		// Container child ConsoleWindow.Gtk.Box+BoxChild
		this.Window2 = new global::Gtk.ScrolledWindow ();
		this.Window2.Name = "Window2";
		this.Window2.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child Window2.Gtk.Container+ContainerChild
		this.Console = new global::Gtk.TextView ();
		this.Console.Name = "Console";
		this.Console.Editable = false;
		this.Console.CursorVisible = false;
		this.Console.AcceptsTab = false;
		this.Console.PixelsAboveLines = 5;
		this.Console.PixelsBelowLines = 5;
		this.Console.LeftMargin = 15;
		this.Console.RightMargin = 15;
		this.Window2.Add (this.Console);
		this.ConsoleWindow.Add (this.Window2);
		global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.ConsoleWindow [this.Window2]));
		w18.Position = 0;
		this.vbox2.Add (this.ConsoleWindow);
		global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.ConsoleWindow]));
		w19.Position = 4;
		this.Add (this.vbox2);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 734;
		this.DefaultHeight = 485;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		this.AddFileAction.Activated += new global::System.EventHandler (this.OnAddFileActionActivated);
		this.AddFileAction1.Activated += new global::System.EventHandler (this.OnAddFileActionActivated);
		this.AutoHideAction.Toggled += new global::System.EventHandler (this.OnAutoHideActionToggled);
		this.ExecuteBtn.Clicked += new global::System.EventHandler (this.OnExecuteBtnClicked);
	}
}
