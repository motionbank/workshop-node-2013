#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


using VVVV.Core.Logging;
using System.Collections.Generic;

#endregion usings

namespace VVVV.Nodes
{
	
	[PluginInfo(Name = "JsonParser", Category = "JSON", Help = "parse json string", Tags = "")]
	public class JsonParser : IPluginEvaluate
	{
		#region fields & pins
		[Input("JSON", DefaultString = "hello")]
		IDiffSpread<string> FInput;
		
		[Input("Go", IsBang=true, IsSingle=true)]
		IDiffSpread<bool> FGo;
		
		[Output("Output json")]
		ISpread<JObject> FJOutput;
		
		[Output("status")]
		ISpread<string> FStatus;
		
		[Import()]
		ILogger FLogger;
		#endregion fields & pins
		//called when data for any output pin is requested
		
		public void Evaluate(int SpreadMax)
		{
			FJOutput.SliceCount = SpreadMax;
			FStatus.SliceCount = SpreadMax;
			
			if(FInput.SliceCount != 0 && FGo[0]==true)
			{
				for(int i=0; i< SpreadMax; i++)
				{
					try
					{
						FJOutput[i] = JObject.Parse(FInput[i]);
						FStatus[i] = "ok";
					}
					catch(Exception e)
					{
						FStatus[i] = "error";
					}
					
				}
			}
		}
	}
	
	[PluginInfo(Name = "SelectToken", Category = "JSON", Help = "select json token", Tags = "")]
	public class SelectToken : IPluginEvaluate
	{
		#region fields & pins
		[Input("JObject")]
		ISpread<JObject> FInput;
		
		[Input("path")]
		ISpread<string> FInputp;
		
		[Output("Output")]
		ISpread<string> FOutput;
		
		[Import()]
		ILogger FLogger;
		#endregion fields & pins
		//called when data for any output pin is requested
		
		public void Evaluate(int SpreadMax)
		{
			FOutput.SliceCount = SpreadMax;
			
			if( FInput[0]!=null){
				
				for (int i = 0; i < FInputp.SliceCount; i++)
				{
					if( FInput[0].SelectToken(FInputp[i])!= null && FInput[0]!=null)
					{
						FOutput[i]=  FInput[0].SelectToken(FInputp[i]).ToString();
					}
					else
					{
						FOutput[i]= "";
					}
				}
			}
		}
	}
	
	[PluginInfo(Name = "JsonArray", Category = "JSON", Help = "list json array content", Tags = "")]
	public class JsonArray : IPluginEvaluate
	{
		#region fields & pins
		[Input("Jobject")]
		ISpread<JObject> FInput;
		
		[Input("path", IsSingle=true)]
		ISpread<string> FInputPath;
		
		[Input("key", IsSingle=true)]
		ISpread<string> FInputKey;
		
		[Input("Debug", IsBang=true, IsSingle=true)]
		IDiffSpread<bool> FDebug;
		
		[Output("Output")]
		ISpread<string> FOutput;
		
		[Output("count")]
		ISpread<int> FcOutput;
		
		[Import()]
		ILogger FLogger;
		#endregion fields & pins
		//called when data for any output pin is requested
		
		public void Evaluate(int SpreadMax)
		{
			FcOutput.SliceCount = SpreadMax;
			FOutput.SliceCount = 0;
			
			if(FInput.SliceCount != 0)
			{
				int i = 0;
				int count = 0;
				
				for(int j=0; j < SpreadMax; j++)
				{
					if(FDebug[0])FLogger.Log(LogType.Debug, "go " + j );
					
					if( FInput[j] != null)
					{
						if(FDebug[0])FLogger.Log(LogType.Debug, "valid Json " + j);
						
						if( FInput[j].SelectToken(FInputPath[0]) != null  )
						{
							var results  = FInput[j].SelectToken(FInputPath[0]); 
							
							if(FDebug[0])FLogger.Log(LogType.Debug, "valid " + FInputPath[0]);
 							
							foreach (JToken child in results.Children())
							{
								// one more result
								FOutput.SliceCount += 1;
							
								if( child.SelectToken(FInputKey[0]) != null ) 
								{
									if(FDebug[0])FLogger.Log(LogType.Debug, (i*(j+1)) + " / " + count + " result child: " + i + " - " + child.SelectToken(FInputKey[0]).ToString().Substring(6));
									FOutput[count] = child.SelectToken(FInputKey[0]).ToString(); 
								}
								else
								{
									FOutput[count] = "empty";
								}
								i++;
								count++;
							}
							FcOutput[j] = i;
						}
						
						
					}
					else
					{
						FOutput.SliceCount = 1;
						FcOutput[0] = 0;
						FOutput[i]= "empt";
					}

					i = 0;
					if(FDebug[0])FLogger.Log(LogType.Debug, "----------------");
				}
			}
		}
	}
}
