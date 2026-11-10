using System;
using System.Text;
class StreamTests {
 static int fails;
 static void Check(string name,bool result){Console.WriteLine((result?"PASS ":"FAIL ")+name);if(!result)fails++;}
 static int Main(){
  var received="";var d=new SseDecoder(s=>received+=s+"|");
  var bytes=Encoding.UTF8.GetBytes(": keepalive\r\ndata: 你好😀\r\n\r\ndata: [DONE]\n\ndata: ignored\n\n");
  foreach(var b in bytes)d.Feed(new[]{b},1);d.Finish();
  Check("UTF8 boundaries, CRLF and DONE",received=="你好😀|"&&d.Done);
  received="";d=new SseDecoder(s=>received=s);bytes=Encoding.UTF8.GetBytes("data: first\ndata: second\n\n");d.Feed(bytes,bytes.Length);d.Finish();
  Check("multiline event",received=="first\nsecond");
  var epoch=new AiRequestGeneration();int old=epoch.Next();epoch.Next();Check("cancel invalidates old callbacks",!epoch.IsCurrent(old));int newer=epoch.Next();Check("restart keeps only newest callbacks",epoch.IsCurrent(newer)&&!epoch.IsCurrent(old));
  bool limited=false;try{d=new SseDecoder(s=>{},16);bytes=Encoding.UTF8.GetBytes("data: "+new string('x',30));d.Feed(bytes,bytes.Length);}catch(InvalidOperationException){limited=true;}Check("bounded event",limited);
  var doc=new HudScrollDocument();doc.Set("abcdefghij",3);Check("wrap retains every character",string.Join("",doc.Lines)=="abcdefghij");
  doc.Offset=1.25f;doc.Set("abcdefghijklmno",3);Check("append preserves reading offset",doc.Offset==1.25f);
  doc.Set("a\n\nb\n",3);Check("blank lines retained",doc.Lines.Count==4&&doc.Lines[1]==""&&doc.Lines[3]=="");
  return fails==0?0:1;
 }
}
