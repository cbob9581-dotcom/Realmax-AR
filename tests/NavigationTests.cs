using System;
class NavigationTests {
 static int fails;static void Check(string n,Func<bool> f){if(f())Console.WriteLine("PASS "+n);else{fails++;Console.WriteLine("FAIL "+n);}}
 static int Main(){
 Check("root return stays home",()=>new WorkspaceNavigation().Back()==WorkspacePage.Home);
 Check("nested network returns settings then home",()=>{var n=new WorkspaceNavigation();n.Open(WorkspacePage.Settings);n.Open(WorkspacePage.Wifi);return n.Back()==WorkspacePage.Settings&&n.Back()==WorkspacePage.Home;});
 Check("pending answer returns original page",()=>{var n=new WorkspaceNavigation();n.Open(WorkspacePage.Apps);n.Open(WorkspacePage.Pending);n.Replace(WorkspacePage.Answer);return n.Back()==WorkspacePage.Apps;});
 Check("repeated same page does not trap back",()=>{var n=new WorkspaceNavigation();n.Open(WorkspacePage.Settings);n.Open(WorkspacePage.Wifi);n.Open(WorkspacePage.Wifi);return n.Back()==WorkspacePage.Settings;});
 Check("home clears history",()=>{var n=new WorkspaceNavigation();n.Open(WorkspacePage.Settings);n.Open(WorkspacePage.Wifi);n.Open(WorkspacePage.Home);return n.Back()==WorkspacePage.Home;});
 Check("answer can be revisited",()=>{var n=new WorkspaceNavigation();n.Open(WorkspacePage.Pending);n.Replace(WorkspacePage.Answer);n.Open(WorkspacePage.Settings);return n.Back()==WorkspacePage.Answer&&n.Back()==WorkspacePage.Home;});
 Check("leaving pending never returns to stale spinner",()=>{var n=new WorkspaceNavigation();n.Open(WorkspacePage.Apps);n.Open(WorkspacePage.Pending);n.Open(WorkspacePage.Settings);return n.Back()==WorkspacePage.Apps;}); return fails==0?0:1;
 }
}
