using System;
public static class ProviderRules {
 public static bool Validate(string provider,string endpoint,string model,string key,out string error){
  error="";
  if(provider!="openai-compatible"&&provider!="anthropic"&&provider!="gemini"){error="Unsupported provider adapter";return false;}
  Uri uri;if(!Uri.TryCreate(endpoint,UriKind.Absolute,out uri)||uri.Scheme!=Uri.UriSchemeHttps||string.IsNullOrEmpty(uri.Host)||!string.IsNullOrEmpty(uri.UserInfo)){error="Enter a valid HTTPS endpoint without URL credentials";return false;}
  if(string.IsNullOrWhiteSpace(model)){error="Model is required";return false;}
  if(string.IsNullOrWhiteSpace(key)||key.IndexOfAny(new[]{'\r','\n'})>=0){error="A valid API key is required";return false;}
  return true;
 }
}
