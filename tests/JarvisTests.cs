using System;
class JarvisTests {
 static int fails;
 static void Check(string name,Func<bool> test){try{if(!test())throw new Exception("assertion");Console.WriteLine("PASS "+name);}catch(Exception e){fails++;Console.WriteLine("FAIL "+name+": "+e.Message);}}
 static int Main(){
  var apps=new[]{new JarvisApp("Firefox","org.mozilla.firefox"),new JarvisApp("设置","com.android.settings"),new JarvisApp("游戏","com.game.a"),new JarvisApp("游戏","com.game.b")};
  Check("sleeping agent ignores ambient command",()=>new JarvisSession().Accept("打开Firefox",0,apps).Action==JarvisAction.None);
  Check("wake plus command",()=>new JarvisSession().Accept("贾维斯，帮我打开浏览器",0,apps).Action==JarvisAction.Browser);
  Check("separate wake then command",()=>{var s=new JarvisSession();return s.Accept("贾 维 斯",0,apps).Action==JarvisAction.Wake&&s.Accept("给我看看装了哪些软件",1,apps).Action==JarvisAction.Apps;});
  Check("wake times out",()=>{var s=new JarvisSession();s.Accept("贾维斯",0,apps);return s.Accept("打开Firefox",30,apps).Action==JarvisAction.None;});
  Check("known app package selected",()=>new JarvisSession().Accept("贾维斯打开Firefox",0,apps).Argument=="org.mozilla.firefox");
  Check("ambiguous labels show choices",()=>new JarvisSession().Accept("贾维斯打开游戏",0,apps).Action==JarvisAction.Apps);
  Check("network goes to manual settings",()=>new JarvisSession().Accept("贾维斯我要设置无线网络",0,apps).Action==JarvisAction.Wifi);
  Check("battery request",()=>new JarvisSession().Accept("贾维斯现在还有多少电",0,apps).Action==JarvisAction.Status);
  Check("same utterance is not executed twice",()=>{var s=new JarvisSession();s.Accept("贾维斯打开浏览器",0,apps);return s.Accept("打开浏览器",.3,apps).Action==JarvisAction.None;});
  Check("cancel disarms",()=>{var s=new JarvisSession();s.Accept("贾维斯",0,apps);s.Accept("取消",1,apps);return s.Accept("打开Firefox",2,apps).Action==JarvisAction.None;});
  Check("unlisted model tool denied",()=>!JarvisTools.TryMap("shell","rm -rf /",apps,out _));
  Check("model cannot open arbitrary package",()=>!JarvisTools.TryMap("open_app","not.installed",apps,out _));
  Check("observed homophone wake is normalized",()=>new JarvisSession().Accept("假 为师 打开 软件 列表",0,apps).Action==JarvisAction.Apps);
  Check("Chinese Firefox alias selects installed app",()=>new JarvisSession().Accept("贾维斯打开火狐",0,apps).Argument=="org.mozilla.firefox");
  Check("semantic app request delegates to configured AI",()=>new JarvisSession().Accept("贾维斯打开那个测试场景",0,apps).Action==JarvisAction.AskAi);
  Check("camera requires explicit local request",()=>new JarvisSession().Accept("贾维斯眼前有什么",0,apps).Action.ToString()=="Look"); Check("cloud cannot invoke camera",()=>!JarvisTools.TryMap("look","",apps,out _)); Check("look at battery remains local",()=>new JarvisSession().Accept("贾维斯帮我看一下电量",0,apps).Action==JarvisAction.Status); Check("look at apps remains local",()=>new JarvisSession().Accept("贾维斯看一下软件列表",0,apps).Action==JarvisAction.Apps); return fails==0?0:1;
 }
}