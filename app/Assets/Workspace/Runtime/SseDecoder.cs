using System;
using System.Text;

public sealed class AiRequestGeneration {
 int current;
 public int Next(){return ++current;}
 public bool IsCurrent(int ticket){return ticket==current;}
}

// Incremental UTF-8 and SSE framing, independent of the JSON/provider adapter.
public sealed class SseDecoder {
 readonly Decoder utf8=new UTF8Encoding(false,true).GetDecoder();
 readonly StringBuilder line=new StringBuilder(),data=new StringBuilder();
 readonly Action<string> onData;readonly int limit;bool cr;
 public bool Done{get;private set;}
 public SseDecoder(Action<string> callback,int maxRecordChars=32768){onData=callback;limit=maxRecordChars;}
 public void Feed(byte[] bytes,int count){
  if(Done)return;
  // Fixed chunks also keep adversarial receive sizes from allocating large buffers.
  var chars=new char[1024];int offset=0;
  while(offset<count&&!Done){int used,written;bool complete;utf8.Convert(bytes,offset,count-offset,chars,0,chars.Length,false,out used,out written,out complete);offset+=used;for(int i=0;i<written&&!Done;i++)Accept(chars[i]);}
 }
 void Accept(char c){
  if(c=='\n'&&cr){cr=false;return;}cr=false;
  if(c=='\r'||c=='\n'){EndLine();cr=c=='\r';return;}
  if(line.Length+data.Length>=limit)throw new InvalidOperationException("Stream record exceeded its limit.");line.Append(c);
 }
 void EndLine(){
  string value=line.ToString();line.Length=0;
  if(value.Length==0){Dispatch();return;}
  if(value.StartsWith("data:",StringComparison.Ordinal)){value=value.Substring(5);if(value.StartsWith(" "))value=value.Substring(1);if(data.Length+value.Length+1>limit)throw new InvalidOperationException("Stream record exceeded its limit.");data.Append(value).Append('\n');}
 }
 void Dispatch(){if(data.Length==0)return;string value=data.ToString(0,data.Length-1);data.Length=0;if(value.Trim()=="[DONE]"){Done=true;return;}onData(value);}
 public void Finish(){if(Done)return;var chars=new char[2];int count=utf8.GetChars(new byte[0],0,0,chars,0,true);for(int i=0;i<count;i++)Accept(chars[i]);if(line.Length>0)EndLine();Dispatch();}
}
