using System.Collections.Generic;
public enum WorkspacePage{Home,Apps,Settings,Wifi,Bluetooth,Browser,Ai,Answer,Pending,Audio}
public sealed class WorkspaceNavigation {
 readonly List<WorkspacePage> history=new List<WorkspacePage>();
 public WorkspacePage Current{get;private set;}=WorkspacePage.Home;
 public void Open(WorkspacePage page){if(page==WorkspacePage.Home){history.Clear();Current=page;return;}if(page==Current)return;if(history.Count==32)history.RemoveAt(0);if(Current!=WorkspacePage.Pending)history.Add(Current);Current=page;}
 public void Replace(WorkspacePage page){Current=page;}
 public WorkspacePage Back(){if(history.Count==0){Current=WorkspacePage.Home;return Current;}int last=history.Count-1;Current=history[last];history.RemoveAt(last);return Current;}
}
