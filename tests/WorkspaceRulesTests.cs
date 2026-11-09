using System;
class WorkspaceRulesTests {
 static int failures;
 static void Check(string name,Func<bool> test){try{if(!test())throw new Exception("Assertion failed");Console.WriteLine("PASS "+name);}catch(Exception e){failures++;Console.WriteLine("FAIL "+name+": "+e.Message);}}
 static int Main(){
  Check("Gemini model applied to base URL",()=>WorkspaceRules.ResolveEndpoint("gemini","https://example.test/v1beta","models/gemini-2")=="https://example.test/v1beta/models/gemini-2:generateContent");
  Check("Gemini model replaces endpoint model",()=>WorkspaceRules.ResolveEndpoint("gemini","https://example.test/v1beta/models/old:generateContent","new")=="https://example.test/v1beta/models/new:generateContent");
  Check("Other provider endpoint retained",()=>WorkspaceRules.ResolveEndpoint("anthropic","https://example.test/v1/messages","model")=="https://example.test/v1/messages");
  Check("Page cannot overshoot end",()=>WorkspaceRules.ClampPage(100,720,360)==1);
  Check("Empty response page zero",()=>WorkspaceRules.ClampPage(10,0,360)==0);
  Check("Negative response page zero",()=>WorkspaceRules.ClampPage(-1,720,360)==0);
  Check("Hidden browser stays inactive after resume",()=>{var s=new BrowserPageState();s.Show();s.Pause(true);s.Hide();s.Pause(false);return !s.Active;});
  Check("Visible browser pauses and resumes",()=>{var s=new BrowserPageState();s.Show();bool a=s.Active;s.Pause(true);bool b=!s.Active;s.Pause(false);return a&&b&&s.Active;});
  return failures==0?0:1;
 }
}
